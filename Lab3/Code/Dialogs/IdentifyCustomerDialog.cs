using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleEchoBot.Controllers;
using SimpleEchoBot.Dynamics;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SimpleEchoBot.Models;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class IdentifyCustomerDialog : LuisDialog<object>
    {
        private DynamicsContextController customerContext;

        public IdentifyCustomerDialog(DynamicsContextController customerContext) : base(new LuisService(new LuisModelAttribute(
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
                    "Are you sure you want to skip identification?",
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
                await context.PostAsync($"Can you give me your customer ID or emailaddress?");
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
            try
            {
                // If customer is already identified, make sure we don't ask it again.
                if (customerContext.CustomerIdentified)
                {
                    context.Done(customerContext);
                    return;
                }

                // TODO: Identify based on skype username.

                var emailAddress = result.Entities.FirstOrDefault(x => x.Type == "builtin.email")?.Entity;
                var customerNumber = result.Entities.FirstOrDefault(x => x.Type == "builtin.number")?.Entity;

                var requestContactsUri = string.Empty;
                var searchType = "emailaddress";
                var inverseSearchType = "customer number";
                if (!string.IsNullOrWhiteSpace(emailAddress))
                {
                    searchType = "emailaddress";
                    inverseSearchType = "customer number";
                    requestContactsUri = $"contacts?$select=fullname,contactid,firstname&$top=5&$filter=(emailaddress2 eq '{emailAddress}')";
                }
                else if (!string.IsNullOrWhiteSpace(customerNumber))
                {
                    // TODO: customer number is not yet a valid field in Dynamics CRM. Therefor only the emailaddress verification will work.
                    searchType = "customer number";
                    inverseSearchType = "emailaddress";
                    requestContactsUri = $"contacts?$select=fullname,contactid,firstname&$top=5&$filter=(customernumber eq '{customerNumber}')";
                }
                else
                {
                    await context.PostAsync("Can you provide your customer number or emailaddress?");
                    context.Wait(MessageReceived);
                    return;
                }

                await context.PostAsync("Thanks, checking our system...");
                var response = await DynamicsHelper<Contact>.HttpClient.GetAsync(requestContactsUri, HttpCompletionOption.ResponseHeadersRead);
                JObject repsonseAccounts = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    if (repsonseAccounts["value"].Count() <= 0)
                    {
                        await context.PostAsync($"Sorry, couldn't find a contact with this {searchType}.");
                        context.Wait(MessageReceived);
                        return;
                    }

                    if (repsonseAccounts["value"].Count() > 1)
                    {
                        await context.PostAsync($"Sorry, there are multiple users with this {searchType}. Try entering your {inverseSearchType}.");
                        context.Wait(MessageReceived);
                        return;
                    }

                    foreach (var item in repsonseAccounts["value"])
                    {
                        customerContext.FirstName = item["firstname"].ToString();
                        customerContext.CustomerId = Guid.Parse(item["contactid"].ToString());

                        context.Done(customerContext);
                        return;
                    }
                }
                else
                {
                    await context.PostAsync("Sorry, I can't connect to the server at this moment. Please try again later.");
                    context.Wait(MessageReceived);
                }
            }
            catch (Exception e)
            {
                await context.PostAsync("Can you give me your customer number or email?");
                context.Wait(MessageReceived);
            }
        }
    }
}