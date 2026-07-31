
=========================================================================================================================================================

Añadi esto por best practice, pero la validacion ya la hace indirectamente cuando :


 public async Task<(bool Success, string? ErrorMessage)> CreateAsync(
            DailyCheckFormViewModel model)
        {
            //Added as best practice, not fully needed
            model.DailyCheckWeek = model.DailyCheckWeek.Trim();
            model.DailyCheckDay = model.DailyCheckDay.Trim();

            /*
             * Business Validation | Daily Check Week (best practice)
             * Confirms that the submitted week belongs to the values
             * supported by the Daily Check module.
             */
            if (!ValidDailyCheckWeeks.Contains(model.DailyCheckWeek))
            {
                return (
                    false,
                    "La semana de control seleccionada no es válida.");
            }


        private static readonly string[] ValidDailyCheckWeeks =
        {
            "Semana 1",
            "Semana 2",
            "Semana 3",
            "Semana 4",
            "Semana 5",
            "Semana 6",
            "Semana 7"
        };


=========================================================================================================================================================











