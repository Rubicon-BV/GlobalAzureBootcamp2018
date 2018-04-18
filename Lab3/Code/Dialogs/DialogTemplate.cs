namespace SimpleEchoBot.Dialogs
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using SimpleEchoBot.Controllers;

    [Serializable]
    public class DialogTemplate : LuisDialog<object>
    {
        private DynamicsContextController customerContext;

        public DialogTemplate(DynamicsContextController customerContext) : base(new LuisService(new LuisModelAttribute(
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
            PromptDialog.Confirm(context, AfterResetAsync, "Are you sure ?", "Didn't get that!", promptStyle: PromptStyle.Auto);
        }

        [LuisIntent("None")]
        [LuisIntent("TemplateIntent")]
        public async Task TemplateIntent(IDialogContext context, LuisResult result)
        {
            // TODO
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
                await context.PostAsync($"TODO");
            }

            context.Wait(MessageReceived);
        }
    }
}