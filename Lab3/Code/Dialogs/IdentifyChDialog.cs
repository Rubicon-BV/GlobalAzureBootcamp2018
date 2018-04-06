

namespace SimpleEchoBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using SimpleEchoBot.Controllers;
    using System.Configuration;
    using System.Threading.Tasks;
    using SimpleEchoBot.Dynamics;
    using SimpleEchoBot.Models;
    using SimpleEchoBot.Resources;
    using System.Linq;

    [System.Serializable]
    public class IdentifyChDialog : LuisDialog<object>
    {
        private DynamicsContextController customerContext;

        public IdentifyChDialog(DynamicsContextController customerContext) : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
            this.customerContext = customerContext;
        }

        // Always make sure to have a way out of a certain dialog pattern.
        [LuisIntent("Stop")]
        public async Task StopIntent(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to skip boiler identification?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                context.Done(customerContext);
                return;
            }
            else
            {
                await context.PostAsync($"Can you give me the name of your boiler?");
            }

            context.Wait(MessageReceived);
        }

        // We route all intents to the same function, as we have one purpose. Identify the customer.
        [LuisIntent("None")]
        [LuisIntent("BrokenBoiler")]
        [LuisIntent("ErrorCode")]
        [LuisIntent("PlanDate")]
        [LuisIntent("Authenticate")]
        public async Task AuthenticateIntent(IDialogContext context, LuisResult result)
        {
            var fetchXml = string.Format(ContactInfo.RetrieveOrderByContact, this.customerContext.CustomerId);
            var salesOrderDetailsList = await DynamicsHelper<SalesOrderDetail>.GetFromCrmFetchXml(fetchXml, "salesorderdetails");

            if (salesOrderDetailsList == null || salesOrderDetailsList.Count() <= 0)
            {
                // No CH known
                await context.PostAsync("Sorry, no boiler known in our system.");
            }
            else if (salesOrderDetailsList.Count() == 1)
            {
                // 1 CH known
                this.customerContext.CustomerCh = salesOrderDetailsList.FirstOrDefault();
                context.Done(this.customerContext);
                return;
            }
            else
            {
                // More then 1 CH known
                PromptDialog.Choice(
                    context: context,
                    resume: this.ResumeAfterChChoise,
                    options: salesOrderDetailsList,
                    prompt: "We have multiple CH's, about wich one do you have a question?",
                    retry: "Sorry, didn't understand, can you select the right CH.",
                    promptStyle: PromptStyle.Auto);

                return;
            }

            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterChChoise(IDialogContext context, IAwaitable<SalesOrderDetail> result)
        {
            var CustomerCh = await result;
            if (CustomerCh != null)
            {
                this.customerContext.CustomerCh = CustomerCh;
                context.Done(this.customerContext);
                return;
            }

            // Still waiting for CH's?
            await context.PostAsync("Sorry, didn't understand, can you select the right boiler.");
            context.Wait(MessageReceived);
        }
    }
}