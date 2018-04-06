using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using SimpleEchoBot.Controllers;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private DynamicsContextController customerContext;

        private string lastMessage;

        private bool shouldContinueWithLastMessage;
 
        public RootDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
            customerContext = new DynamicsContextController();
        }

        protected override Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            this.shouldContinueWithLastMessage = false;
            return base.MessageReceived(context, item);
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Welcome to RubiHeat, how may I be of assistance?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("BrokenBoiler")]
        public async Task BrokenBoilerIntent(IDialogContext context, LuisResult result)
        {
            if (!customerContext.CustomerIdentified)
            {
                await context.PostAsync("I'm sorry to hear that!");
                this.shouldContinueWithLastMessage = true;
                this.lastMessage = result.Query;
                await context.Forward(new IdentifyCustomerDialog(customerContext), this.ResumeAfterCustomerIdentification, context.Activity, CancellationToken.None);
                return;
            }
            else if (customerContext.CustomerCv == null)
            {
                await context.Forward(new IdentifyCvDialog(customerContext), this.ResumeAfterCvIdentification, context.Activity, CancellationToken.None);
                return;
            }

            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("ErrorCode")]
        public async Task ErrorCodeIntent(IDialogContext context, LuisResult result)
        {
            if (!customerContext.CustomerIdentified)
            {
                await context.PostAsync("I'm sorry to hear that!");
                this.shouldContinueWithLastMessage = true;
                this.lastMessage = result.Query;
                await context.Forward(new IdentifyCustomerDialog(customerContext), this.ResumeAfterCustomerIdentification, context.Activity, CancellationToken.None);
                return;
            }
            else if (customerContext.CustomerCv == null)
            {
                this.shouldContinueWithLastMessage = true;
                this.lastMessage = result.Query;
                await context.Forward(new IdentifyCvDialog(customerContext), this.ResumeAfterCvIdentification, context.Activity, CancellationToken.None);
                return;
            }

            var errorCode = result.Entities?.FirstOrDefault(x => x.Type == "ErrorCode")?.Entity;
            if (string.IsNullOrWhiteSpace(errorCode) && !this.customerContext.ErrorCodeAsked)
            {
                this.customerContext.ErrorCodeAsked = true;
                await context.PostAsync($"Can you provide the error code?");
                context.Wait(MessageReceived);
                return;

            }
            else if(string.IsNullOrWhiteSpace(errorCode) && this.customerContext.ErrorCodeAsked)
            {
                errorCode = result.Query;
                await context.PostAsync($"Ok, I will check the Q&A for error code: {errorCode}");
                await context.PostAsync($"Q&A not implemented yet");
            }
            else
            {
                await context.PostAsync($"Ok, I will check the Q&A for error code: {errorCode}");
                await context.PostAsync($"Q&A not implemented yet");
            }

            this.customerContext.ErrorCode = errorCode;
            PromptDialog.Confirm(context, this.ResumeAfterMechanicPrompt, "Do you need a mechanic?");
        }

        [LuisIntent("PlanDate")]
        public async Task PlanDateIntent(IDialogContext context, LuisResult result)
        {
            await context.Forward(new PlanAppointment(customerContext), this.ResumeAfterPlanDate, context.Activity, CancellationToken.None);
        }

        [LuisIntent("Authenticate")]
        public async Task AuthenticateIntent(IDialogContext context, LuisResult result)
        {
            await context.Forward(new IdentifyCustomerDialog(customerContext), this.ResumeAfterCustomerIdentification, context.Activity, CancellationToken.None);
        }

        [LuisIntent("Stop")]
        public async Task StopIntent(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to skip identification?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.customerContext = new DynamicsContextController();
                return;
            }
            else
            {
                await context.PostAsync($"Ok, no reset, where were we?");
            }

            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterCustomerIdentification(IDialogContext context, IAwaitable<object> result)
        {
            var newContext = await result as DynamicsContextController;
            this.customerContext = newContext;
            if (this.customerContext.CustomerId.HasValue) // Customer identification successful.
            {
                await context.PostAsync($"Welcome {this.customerContext.FirstName}");
                if(this.shouldContinueWithLastMessage)
                {
                    var newMessage = context.MakeMessage();
                    newMessage.Text = this.lastMessage;

                    await this.MessageReceived(context, Awaitable.FromItem(newMessage));
                    return;
                }
            }

            context.Wait(this.MessageReceived);
        }

        private async Task ResumeAfterCvIdentification(IDialogContext context, IAwaitable<object> result)
        {
            var newContext = await result as DynamicsContextController;
            this.customerContext = newContext;

            if (this.shouldContinueWithLastMessage)
            {
                var newMessage = context.MakeMessage();
                newMessage.Text = this.lastMessage;

                await context.PostAsync($"We see that you have a {this.customerContext.CustomerCv.ProductName}.");
                await this.MessageReceived(context, Awaitable.FromItem(newMessage));
                return;
            }

            await context.PostAsync($"We see that you have a {this.customerContext.CustomerCv.ProductName}. Does the boiler show a error code?");
            this.customerContext.ErrorCodeAsked = true;
            context.Wait(this.MessageReceived);
        }

        private async Task ResumeAfterPlanDate(IDialogContext context, IAwaitable<object> result)
        {
            var newContext = await result as DynamicsContextController;
            this.customerContext = newContext;

            if (this.shouldContinueWithLastMessage)
            {
                var newMessage = context.MakeMessage();
                newMessage.Text = this.lastMessage;

                await this.MessageReceived(context, Awaitable.FromItem(newMessage));
                return;
            }

            context.Wait(this.MessageReceived);
        }

        public async Task ResumeAfterMechanicPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            var realResult = await result;
            if(!realResult)
            {
                await context.PostAsync($"Ok, good luck!");
                context.Wait(MessageReceived);
                return;
            }
            else
            {
                await context.PostAsync($"Ok, let's make an appointment.");
                await context.Forward(new PlanAppointment(customerContext), this.ResumeAfterPlanDate, context.Activity, CancellationToken.None);
                return;
            }
        }
    }
}