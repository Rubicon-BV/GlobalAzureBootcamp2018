[LuisIntent("Warranty")]
        [LuisIntent("BoilerAge")]
        public async Task WarrantyIntent(IDialogContext context, LuisResult result)
        {
            if (!_hasWarranty)
            {
                _hasWarranty = true;
                
                await context.PostAsync($"Hello, when did you purchase the device?");

                context.Wait(MessageReceived);
                return;
            }

            if (_hasWarranty)
            {
                var boilerAge = result.Entities.FirstOrDefault(x => x.Type == "boilerAge")?.Entity;
                var boilerBuildYear = result.Entities.FirstOrDefault(x => x.Type == "boilerBuildYear")?.Entity;

                int age;
                if (!int.TryParse(boilerAge, out age))
                {
                    await context.PostAsync("I'm a very simple bot, I only understand numbers... Please improve me!!!");
                    return;
                }

                if (age <= 2)
                {
                    await context.PostAsync($"Good news, your device has warranty!");
                }
                else
                {
                    await context.PostAsync($"We are sorry, but you warranty has expired. The warranty is 2 years.");
                }

            }
        }
