namespace SimpleEchoBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using SimpleEchoBot.Controllers;
    using System.Configuration;
    using System.Linq;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class PlanAppointment : LuisDialog<object>
    {
        private DynamicsContextController customerContext;

        private DateTime? suggestedDate;

        public PlanAppointment(DynamicsContextController customerContext) : base(new LuisService(new LuisModelAttribute(
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
                    "Are you sure you want to stop planning an appointment?",
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
                await context.PostAsync($"TODO: Continue planning... ?!");
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
            object formattedValues = null;
            var entity = result.Entities.FirstOrDefault(x =>
                                x.Type == "builtin.datetimeV2.date" ||
                                x.Type == "builtin.datetimeV2.daterange" ||
                                x.Type == "builtin.datetimeV2.timerange" ||
                                x.Type == "builtin.datetimeV2.time" ||
                                x.Type == "builtin.datetimeV2.datetime" ||
                                x.Type == "builtin.datetimeV2.datetimerange");
             entity?
                .Resolution?
                .TryGetValue("values", out formattedValues);

            // If no date indication, please ask for a date.
            if(entity == null)
            {
                await context.PostAsync($"When are you home to receive a mechanic?");
                context.Wait(MessageReceived);
                return;
            }

            // Als date time --> plan dan afspraak in in dynamics.
            // Als date time in verleden, zeg dan dat het lastig wordt om langs te komen.
            // Als geen date time en wel vraag gesteld, stel dan zelf een datum voor (random).

            Chronic.Parser parser = new Chronic.Parser();
            EntityRecommendation date = new EntityRecommendation();
            Chronic.Span resultSpan = null;
            
            if (entity.Type == "builtin.datetimeV2.daterange")
            {
                //var resolutionValues = (IList<object>)entity.Resolution["values"];
                //foreach (var value in resolutionValues)
                //{
                //    this.startDate = Convert.ToDateTime(((IDictionary<string, object>)value)["start"]);
                //    this.endDate = Convert.ToDateTime(((IDictionary<string, object>)value)["end"]);
                //}
                result.TryFindEntity("builtin.datetimeV2.daterange", out date);
                resultSpan = parser.Parse(date.Entity);
            }
            else if (entity.Type == "builtin.datetimeV2.date")
            {
                //var resolutionValues = (IList<object>)entity.Resolution["values"];
                //foreach (var value in resolutionValues)
                //{
                //    this.startDate = Convert.ToDateTime(((IDictionary<string, object>)value)["value"]);
                //}
                //result.TryFindEntity("builtin.datetimeV2.timerange", out date);
                //resultSpan = parser.Parse(date.Entity);
                result.TryFindEntity("builtin.datetimeV2.date", out date);
                resultSpan = parser.Parse(date.Entity);
            }
            else if(entity.Type == "builtin.datetimeV2.timerange")
            {
                result.TryFindEntity("builtin.datetimeV2.timerange", out date);
                resultSpan = parser.Parse(date.Entity);
            }
            else if(entity.Type == "builtin.datetimeV2.time")
            {
                result.TryFindEntity("builtin.datetimeV2.time", out date);
                resultSpan = parser.Parse(date.Entity);
            }
            else if (entity.Type == "builtin.datetimeV2.datetime")
            {
                result.TryFindEntity("builtin.datetimeV2.datetime", out date);
                resultSpan = parser.Parse(date.Entity);
            }

            // If we would have used a scheduling service, we could just feed it the dates and it would come up with a suggestion...
            if (!resultSpan.Start.HasValue && !resultSpan.End.HasValue)
            {
                await context.PostAsync($"When are you home to receive a mechanic?");
                context.Wait(MessageReceived);
                return;
            }
            else if(resultSpan.Start.Value <= DateTime.Now && resultSpan.End.Value <= DateTime.Now)
            {
                await context.PostAsync($"I understand you want this issue to be resolved already, but we can't go back in time to send a mechanic.");
                context.Wait(MessageReceived);
                return;
            }
            else if(resultSpan.Start.Value.Date != resultSpan.End.Value.Date)
            {
                // We got a date range, let's pick a random date (and time).
                await context.PostAsync($"Date range, let's pick a random date");
                context.Wait(MessageReceived);
                return;
            }
            else if(resultSpan.Start.Value.TimeOfDay.Hours == 0)
            {
                // Midnight, assume that we didn't recieve a time frame and suggest some random times a day.
                // GIVE TWO OPTIONS
                await context.PostAsync($"Single time on a day.");
                context.Wait(MessageReceived);
                return;
            }
            else if(resultSpan.Start.Value.TimeOfDay.Hours == resultSpan.Start.Value.TimeOfDay.Hours)
            {
                // We got a single time on that day
                await context.PostAsync($"Sorry, there is no mechanic available at " + resultSpan.Start.Value.TimeOfDay.Hours + ".");
                this.suggestedDate = resultSpan.Start.Value.AddHours(1);
                PromptDialog.Confirm(context, this.ConfirmMechanicDateTime, $"There is one available at " + (resultSpan.Start.Value.TimeOfDay.Hours + 1) + ", should I schedule that for you?");
                return;
            }
            else if(resultSpan.Start.Value.TimeOfDay.Hours != resultSpan.Start.Value.TimeOfDay.Hours)
            {
                // Just have some fun, pick a random time on this day
                await context.PostAsync($"Timeframe on a day, pick a random time");
                context.Wait(MessageReceived);
                return;
            }
            else
            {
                // Yeah, what else?
                await context.PostAsync($"Yeah, what else? =(");
                context.Wait(MessageReceived);
                return;
            }
        }

        public async Task ConfirmMechanicDateTime(IDialogContext context, IAwaitable<bool> result)
        {
            var realResult = await result;
            if (!realResult)
            {
                var newMessage = context.MakeMessage();
                await context.PostAsync("Ok, let's try to find a moment that works better.");
                await context.PostAsync("When is a good time for you?");
                return;
            }
            else
            {
                var scheduledDate = this.suggestedDate.Value.ToShortDateString();
                var scheduledTime = this.suggestedDate.Value.ToShortTimeString();

                await context.PostAsync($"Ok, scheduled for " + scheduledDate + " at " + scheduledTime + ".");
                this.customerContext.AppointmentScheduledOn = this.suggestedDate;
                context.Done(this.customerContext);
                return;
            }
        }
    }
}