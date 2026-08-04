using DB_GranjaLaFlor.Data.Context;
using Microsoft.EntityFrameworkCore;
using ProjectGranjaLaFlor.Models.ViewModels.ExpectedValue;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Architecture Decision | Service Layer: Business logic and database access are implemented inside Services.
     * Controllers should coordinate HTTP requests and delegate data operations to the Service layer.
     */
    public class ExpectedValueService
    {
        private readonly ApplicationDbContext _context;

        public ExpectedValueService(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Data Query | Expected Values: Retrieves the six fixed Expected Value records required by the Index view.
         * Records are ordered from Semana 1 through Semana 6.
         */
        public async Task<List<ExpectedValueListViewModel>> GetAllAsync()
        {
            return await _context.ExpectedValues
                .AsNoTracking()
                .OrderBy(expectedValue =>
                    expectedValue.ExpectedValueId)
                .Select(expectedValue =>
                    new ExpectedValueListViewModel
                    {
                        ExpectedValueId = expectedValue.ExpectedValueId,

                        ExpectedValueWeek = expectedValue.ExpectedValueWeek,

                        ExpectedConsumption = expectedValue.ExpectedConsumption,

                        ExpectedWeight =expectedValue.ExpectedWeight,

                        ExpectedConversion = expectedValue.ExpectedConversion,

                        ExpectedMortality = expectedValue.ExpectedMortality
                    })
                .ToListAsync();
        }

        /*
         * Data Query | Expected Value Form by Identifier: Retrieves the Expected Value record required by the Edit view.
         * The record is projected into ExpectedValueFormViewModel so the view does not depend directly on the entity.
         */
        public async Task<ExpectedValueFormViewModel?> GetFormByIdAsync(int id)
        {
            return await _context.ExpectedValues
                .AsNoTracking()
                .Where(expectedValue =>
                    expectedValue.ExpectedValueId == id)
                .Select(expectedValue =>
                    new ExpectedValueFormViewModel
                    {
                        ExpectedValueId = expectedValue.ExpectedValueId,

                        ExpectedValueWeek = expectedValue.ExpectedValueWeek,

                        ExpectedConsumption = expectedValue.ExpectedConsumption,

                        ExpectedWeight = expectedValue.ExpectedWeight,

                        ExpectedConversion = expectedValue.ExpectedConversion,

                        ExpectedMortality = expectedValue.ExpectedMortality
                    })
                .FirstOrDefaultAsync();
        }

        /*
         * Business Operation | Update Expected Value: Validates and updates the editable values associated with the selected fixed weekly record.
         *
         * ExpectedValueWeek is not updated because the week identifies the fixed catalog record.
         */
        public async Task UpdateAsync(ExpectedValueFormViewModel model)
        {
            /*
             * Business Validation | Existing Expected Value: Confirms that the selected Expected Value record exists.
             */
            var existingExpectedValue =
                await _context.ExpectedValues
                    .FirstOrDefaultAsync(expectedValue =>
                        expectedValue.ExpectedValueId ==
                            model.ExpectedValueId);

            if (existingExpectedValue == null)
            {
                throw new InvalidOperationException("El valor esperado seleccionado no existe.");
            }

            /*
             * Business Validation | Expected Consumption: Confirms that the expected consumption is greater than zero.
             */
            if (model.ExpectedConsumption <= 0)
            {
                throw new InvalidOperationException("El consumo esperado debe ser mayor que cero.");
            }

            /*
             * Business Validation | Expected Weight: Confirms that the expected weight is greater than zero.
             */
            if (model.ExpectedWeight <= 0)
            {
                throw new InvalidOperationException("El peso esperado debe ser mayor que cero.");
            }

            /*
             * Business Validation | Expected Conversion: Confirms that the expected conversion is greater than zero.
             */
            if (model.ExpectedConversion <= 0)
            {
                throw new InvalidOperationException("La conversión esperada debe ser mayor que cero.");
            }

            /*
             * Business Validation | Expected Mortality: Confirms that the expected mortality percentage remains within the supported range.
             */
            if (model.ExpectedMortality < 0 ||
                model.ExpectedMortality > 100)
            {
                throw new InvalidOperationException("La mortalidad esperada debe encontrarse entre 0 y 100.");
            }

            /*
             * Entity Mapping | Expected Value: Updates only the editable values of the record.
             *
             * ExpectedValueWeek remains unchanged because the user cannot reassign the fixed week.
             */
            existingExpectedValue.ExpectedConsumption = model.ExpectedConsumption;

            existingExpectedValue.ExpectedWeight = model.ExpectedWeight;

            existingExpectedValue.ExpectedConversion = model.ExpectedConversion;

            existingExpectedValue.ExpectedMortality = model.ExpectedMortality;

            /*
             * Database Operation | Save Expected Value: Persists the validated changes in the database.
             */
            await _context.SaveChangesAsync();
        }
    }
}