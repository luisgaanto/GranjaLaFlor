using ProjectGranjaLaFlor.Models.ViewModels.BroodReport;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DB_GranjaLaFlor.Services
{
    /*
     * Service Layer | Brood Report PDF
     *
     * Generates the printable PDF representation of a
     * historical Brood Report snapshot.
     *
     * The PDF is generated entirely in memory and is based
     * only on the historical information stored in the
     * Brood Report snapshot.
     */
    public class BroodReportPdfService
    {
        /*
         * PDF Generation | Brood Report
         *
         * Generates the PDF document in memory and returns
         * the resulting byte array to the Controller.
         */
        public byte[] GeneratePdf(
            BroodReportGetByIdViewModel model)
        {
            var document =
                Document.Create(container =>
                {
                    container.Page(page =>
                    {

                        /*
                         * PDF Page Configuration | Brood Report: Keeps the same horizontal width used by an A4 Landscape document while allowing the page 
                         * height to grow automatically according to the complete Brood Report content. This reproduces the long-sheet format of the original
                         * production control document without increasing the horizontal width of the report.
                         */
                        page.ContinuousSize(
                            PageSizes.A4.Landscape().Width);

                        /*
                         * Defines the space between the PDF content
                         * and the edges of the continuous page.
                         */
                        page.Margin(20);

                        /*
                         * Defines the default font size used throughout
                         * the Brood Report unless a specific section overrides it.
                         */
                        page.DefaultTextStyle(
                            text =>
                                text.FontSize(8));

                        /*
                         * PDF Content
                         *
                         * Defines the vertical structure of the
                         * printable Brood Report.
                         */
                        page.Content()
                            .Column(column =>
                            {
                                /*
                                 * PDF Section | Header
                                 */
                                ComposeHeader(
                                    column,
                                    model);

                                /*
                                 * PDF Section | Daily and Weekly Control
                                 *
                                 * Daily and Weekly information belongs
                                 * to the same operational table because
                                 * each Weekly Check is displayed beside
                                 * its corresponding seven Daily Checks.
                                 */
                                ComposeDailyControl(
                                    column,
                                    model);

                                /*
                                 * PDF Section | Footer
                                 */
                                ComposeFooter(
                                    column);
                            });
                    });
                });

            return document.GeneratePdf();
        }

        /*
 * PDF Section | Header
 *
 * Generates the original Brood Report header following
 * the visual distribution of the physical control sheet.
 *
 * Company information is centered, document information
 * is aligned to the right, production reference fields
 * are displayed as label-and-line fields, and the report
 * number is positioned beside them.
 */
        private void ComposeHeader(
            ColumnDescriptor column,
            BroodReportGetByIdViewModel model)
        {
            var header =
                model.Snapshot.Header;


            /*
             * =========================================================
             * Header Row | Company and Document Information
             * =========================================================
             *
             * Original visual distribution:
             *
             *                  Ganadera Cariblanco AJ Limitada
             *                  Cédula Jurídica ...
             *
             *                                      Tarjeta de Control...
             *                                      RE-PE-003 | VERSIÓN 04
             */
            column.Item()
                .Row(row =>
                {
                    /*
                     * Left visual space.
                     *
                     * The original printed document contains a logo
                     * in this area. The space is preserved even though
                     * no logo is currently generated by the application.
                     */
                    row.RelativeItem(1.0f);


                    /*
                     * Center | Purchasing Company Information
                     */
                    row.RelativeItem(1.8f)
                        .AlignCenter()
                        .Column(companyColumn =>
                        {
                            companyColumn.Item()
                                .AlignCenter()
                                .Text(
                                    "Ganadera Cariblanco AJ Limitada")
                                .Bold()
                                .FontSize(14);

                            companyColumn.Item()
                                .AlignCenter()
                                .Text(
                                    "Cédula Jurídica 3-102-712633")
                                .FontSize(9);
                        });


                    /*
                     * Right | Original Document Information
                     */
                    row.RelativeItem(1.2f)
                        .AlignRight()
                        .Column(documentColumn =>
                        {
                            documentColumn.Item()
                                .AlignRight()
                                .Text(
                                    "Tarjeta de Control de Aves")
                                .Bold()
                                .FontSize(11);

                            documentColumn.Item()
                                .AlignRight()
                                .Text(
                                    "RE-PE-003 | VERSIÓN 04")
                                .FontSize(8);
                        });
                });

            /*
             * =========================================================
             * Production Reference Information + Report Number
             * =========================================================
             *
             * Positions the QQ, production reference and report number
             * fields as one compact group near the center-right area
             * of the original printed header.
             *
             * A fixed left spacer is used instead of PaddingLeft so
             * the horizontal position can be controlled explicitly.
             */
            column.Item()
                .PaddingTop(10)
                .Row(row =>
                {
                    /*
                     * Horizontal Position
                     *
                     * Creates an intentional empty area before the
                     * production reference group.
                     *
                     * Increase this value to move the complete group
                     * farther to the right.
                     */
                    row.ConstantItem(235);


                    /*
                     * Feed References
                     */
                    row.ConstantItem(190)
                        .PaddingRight(6)
                        .Column(feedColumn =>
                        {
                            AddHeaderBoxField(
                                feedColumn,
                                "QQ PIP",
                                string.Empty);

                            AddHeaderBoxField(
                                feedColumn,
                                "QQ Desarrollo",
                                string.Empty);

                            AddHeaderBoxField(
                                feedColumn,
                                "QQ Final",
                                string.Empty);
                        });


                    /*
                     * Production References
                     */
                    row.ConstantItem(170)
                        .PaddingLeft(2)
                        .PaddingRight(6)
                        .Column(productionColumn =>
                        {
                            AddHeaderBoxField(
                                productionColumn,
                                "PDJ",
                                string.Empty);

                            AddHeaderBoxField(
                                productionColumn,
                                "M²",
                                string.Empty);

                            AddHeaderBoxField(
                                productionColumn,
                                "Densidad",
                                string.Empty);
                        });


                    /*
                     * Report Number
                     *
                     * Positioned directly beside the production
                     * reference information.
                     */
                    row.ConstantItem(135)
                        .AlignBottom()
                        .PaddingBottom(3)
                        .Column(numberColumn =>
                        {
                            AddHeaderLineField(
                                numberColumn,
                                "Nº",
                                header.ReportNumber.ToString(),
                                true,
                                24);
                        });


                    /*
                     * Remaining Horizontal Space
                     */
                    row.RelativeItem();
                });

            /*
             * =========================================================
             * Brood Identification
             * =========================================================
             *
             * These values are not displayed inside a table.
             *
             * Each field contains:
             *
             * Label:    Value
             *           ─────────────
             *
             * matching the original printed control sheet.
             */
            column.Item()
                .PaddingTop(8)
                .Row(row =>
                {
                    row.RelativeItem(1.15f)
                        .PaddingRight(10)
                        .Element(container =>
                        {
                            ComposeHeaderInlineField(
                                container,
                                "Fecha:",
                                header.Date.ToString(
                                    "dd/MM/yyyy"));
                        });


                    row.RelativeItem(1.20f)
                        .PaddingRight(10)
                        .Element(container =>
                        {
                            ComposeHeaderInlineField(
                                container,
                                "Granja:",
                                header.FarmName);
                        });


                    row.RelativeItem(1.20f)
                        .PaddingRight(10)
                        .Element(container =>
                        {
                            ComposeHeaderInlineField(
                                container,
                                "Nº Aves:",
                                header.BirdQuantity.ToString(
                                    "N0"));
                        });


                    row.RelativeItem(1.10f)
                        .Element(container =>
                        {
                            ComposeHeaderInlineField(
                                container,
                                "Galera:",
                                header.BroilerHouseName);
                        });
                });
        }

        /*
         * PDF Helper | Header Box Field
         *
         * Displays a production reference label followed by
         * the small rectangular field used in the original
         * printed Brood Report.
         *
         * The dimensions remain compact so the QQ and
         * production reference groups stay close together.
         */
        private void AddHeaderBoxField(
            ColumnDescriptor column,
            string label,
            string value)
        {
            column.Item()
                .PaddingVertical(1.5f)
                .Row(row =>
                {
                    /*
                     * Field Label
                     */
                    row.ConstantItem(76)
                        .AlignMiddle()
                        .Text(label)
                        .FontSize(8);


                    /*
                     * Small Rectangular Field
                     */
                    row.ConstantItem(72)
                        .Height(17)
                        .Border(0.7f)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text(value)
                        .FontSize(8);
                });
        }


        /*
         * PDF Helper | Header Line Field
         *
         * Displays a label followed by a value positioned
         * above a horizontal line.
         *
         * The label width can be adjusted when a more compact
         * field is required, such as the report number.
         */
        private void AddHeaderLineField(
            ColumnDescriptor column,
            string label,
            string value,
            bool boldValue = false,
            float labelWidth = 82)
        {
            column.Item()
                .PaddingVertical(2)
                .Row(row =>
                {
                    /*
                     * Field Label
                     */
                    row.ConstantItem(labelWidth)
                        .AlignMiddle()
                        .Text(label)
                        .FontSize(8);


                    /*
                     * Field Value + Line
                     */
                    row.RelativeItem()
                        .Column(valueColumn =>
                        {
                            valueColumn.Item()
                                .MinHeight(11)
                                .AlignCenter()
                                .Element(container =>
                                {
                                    if (boldValue)
                                    {
                                        container
                                            .Text(value)
                                            .Bold()
                                            .FontSize(8);
                                    }
                                    else
                                    {
                                        container
                                            .Text(value)
                                            .FontSize(8);
                                    }
                                });

                            valueColumn.Item()
                                .BorderBottom(0.7f);
                        });
                });
        }


        /*
         * PDF Helper | Header Inline Field
         *
         * Displays the field label followed by the historical
         * value positioned directly above a horizontal line.
         *
         * Example:
         *
         * Fecha:      01/07/2026
         *             ───────────
         */
        private void ComposeHeaderInlineField(
            IContainer container,
            string label,
            string value)
        {
            container
                .Row(row =>
                {
                    /*
                     * Label
                     */
                    row.AutoItem()
                        .PaddingRight(4)
                        .AlignBottom()
                        .Text(label)
                        .FontSize(8);


                    /*
                     * Value and writable line
                     */
                    row.RelativeItem()
                        .Column(valueColumn =>
                        {
                            valueColumn.Item()
                                .MinHeight(11)
                                .AlignCenter()
                                .Text(value)
                                .FontSize(8);

                            valueColumn.Item()
                                .BorderBottom(0.7f);
                        });
                });
        }


        /*
         * PDF Helper | Empty Header Row
         */
        private void AddEmptyHeaderRow(
            TableDescriptor table,
            string label)
        {
            table.Cell()
                .Border(1)
                .Padding(4)
                .Text(label)
                .Bold();

            table.Cell()
                .Border(1)
                .Padding(4)
                .Text(string.Empty);
        }


        /*
         * PDF Helper | Header Cell
         */
        private void AddHeaderCell(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Border(1)
                .Padding(4)
                .AlignCenter()
                .Text(text)
                .Bold();
        }


        /*
         * PDF Helper | Value Cell
         */
        private void AddValueCell(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Border(1)
                .Padding(4)
                .AlignCenter()
                .Text(text);
        }


        /*
         * PDF Section | Daily and Weekly Control
         *
         * Generates the main operational table containing
         * Control de Aves, Control de Alimento and
         * Control Semanal.
         *
         * Each Weekly Check is displayed beside the seven
         * Daily Check records that belong to the same
         * production week.
         *
         * The printed template contains forty-five visual rows.
         * Six complete production weeks provide forty-two Daily
         * Check records, therefore rows forty-three through
         * forty-five remain empty.
         */
        private void ComposeDailyControl(
            ColumnDescriptor column,
            BroodReportGetByIdViewModel model)
        {
            /*
             * Historical Daily Information
             */
            var dailyRows =
                model.Snapshot.DailyRows
                    .OrderBy(row =>
                        row.DayNumber)
                    .ToList();

            /*
             * Historical Weekly Information
             */
            var weeklyChecks =
                model.Snapshot.WeeklyChecks
                    .OrderBy(weeklyCheck =>
                        weeklyCheck.Week)
                    .ToList();


            /*
             * Section Title
             
            column.Item()
                .PaddingTop(10)
                .Text(
                    "Control de Aves / Control de Alimento / Control Semanal")
                .Bold()
                .FontSize(10);
            */

            /*
             * Main Operational Table
             */
            column.Item()
                .PaddingTop(10)
                .Table(table =>
                {
                    /*
                     * Table Columns
                     *
                     * Control General:
                     * Fecha | Día
                     *
                     * Control de Aves:
                     * Nat. | Selec. | Acum. | Saldo Aves
                     *
                     * Control de Alimento:
                     * Ingreso Día | Ingreso Acum.
                     * Gasto Día | Gasto Acum. | Saldo
                     *
                     * Control Semanal:
                     * Semana / Concepto | Esp. | Real | Dif.
                     */
                    table.ColumnsDefinition(columns =>
                    {
                        /*
                         * General Information
                         */
                        columns.RelativeColumn(1.40f);  // Fecha
                        columns.RelativeColumn(0.55f);  // Día

                        /*
                         * Control de Aves
                         */
                        columns.RelativeColumn(0.65f);  // Nat.
                        columns.RelativeColumn(0.65f);  // Selec.
                        columns.RelativeColumn(0.75f);  // Acum.
                        columns.RelativeColumn(1.00f);  // Saldo Aves

                        /*
                         * Control de Alimento
                         */
                        columns.RelativeColumn(0.80f);  // Ingreso Día
                        columns.RelativeColumn(0.90f);  // Ingreso Acum.

                        columns.RelativeColumn(0.80f);  // Gasto Día
                        columns.RelativeColumn(0.90f);  // Gasto Acum.

                        columns.RelativeColumn(0.85f);  // Saldo

                        /*
                         * Control Semanal
                         */
                        columns.RelativeColumn(0.85f);  // Semana / Concepto
                        columns.RelativeColumn(0.85f);  // Esperado
                        columns.RelativeColumn(0.85f);  // Real
                        columns.RelativeColumn(0.95f);  // Diferencia
                    });


                    /*
                     * =================================================
                     * Header Row 1
                     * =================================================
                     */

                    /*
                     * Fecha
                     */
                    table.Cell()
                        .RowSpan(3)
                        .Element(DailyHeaderCell)
                        .Text("Fecha");

                    /*
                     * Día
                     */
                    table.Cell()
                        .RowSpan(3)
                        .Element(DailyHeaderCell)
                        .Text("Día");


                    /*
                     * Control de Aves
                     */
                    table.Cell()
                        .ColumnSpan(4)
                        .Element(DailyHeaderCell)
                        .Text("Control de Aves");


                    /*
                     * Control de Alimento
                     */
                    table.Cell()
                        .ColumnSpan(5)
                        .Element(DailyHeaderCell)
                        .Text("Control de Alimento");


                    /*
                     * Control Semanal
                     */
                    table.Cell()
                        .ColumnSpan(4)
                        .Element(DailyHeaderCell)
                        .Text("Control Semanal");


                    /*
                     * =================================================
                     * Header Row 2
                     * =================================================
                     */

                    /*
                     * Control de Aves | Mortalidad
                     */
                    table.Cell()
                        .ColumnSpan(3)
                        .Element(DailyHeaderCell)
                        .Text("Mortalidad");

                    /*
                     * Control de Aves | Saldo
                     */
                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Saldo Aves");


                    /*
                     * Control de Alimento | Ingreso
                     */
                    table.Cell()
                        .ColumnSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Ingreso");

                    /*
                     * Control de Alimento | Gasto
                     */
                    table.Cell()
                        .ColumnSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Gasto");

                    /*
                     * Control de Alimento | Saldo
                     */
                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text(
                            "Saldo\n(qq)");


                    /*
                     * Control Semanal | Headers
                     *
                     * These cells span the second and third
                     * header rows because the Weekly Control
                     * does not require another nested level.
                     */
                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Semana");

                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Esp.");

                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Real");

                    table.Cell()
                        .RowSpan(2)
                        .Element(DailyHeaderCell)
                        .Text("Dif.");


                    /*
                     * =================================================
                     * Header Row 3
                     * =================================================
                     */

                    /*
                     * Mortality
                     */
                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text("Nat.");

                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text("Selec.");

                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text("Acum.");


                    /*
                     * Concentrate Income
                     */
                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text(
                            "Día\n(qq)");

                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text(
                            "Acum.\n(qq)");


                    /*
                     * Concentrate Consumption
                     */
                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text(
                            "Día\n(qq)");

                    table.Cell()
                        .Element(DailyHeaderCell)
                        .Text(
                            "Acum.\n(qq)");


                    /*
                     * =================================================
                     * Operational Rows
                     * =================================================
                     *
                     * Always renders forty-five visual rows.
                     */
                    for (var rowNumber = 1;
                         rowNumber <= 45;
                         rowNumber++)
                    {
                        var dailyRow =
                            dailyRows.FirstOrDefault(
                                row =>
                                    row.DayNumber ==
                                    rowNumber);


                        /*
                         * =============================================
                         * Daily Check Information
                         * =============================================
                         */
                        if (dailyRow != null)
                        {
                            /*
                             * Fecha
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.Date.ToString(
                                    "dd/MM/yyyy"));

                            /*
                             * Día
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.DayNumber.ToString());

                            /*
                             * Mortalidad Natural
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.NaturalMortality
                                    .ToString());

                            /*
                             * Selección
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.SelectQuantity
                                    .ToString());

                            /*
                             * Mortalidad Acumulada
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.AccumulatedMortality
                                    .ToString());

                            /*
                             * Saldo Aves
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.BirdBalance
                                    .ToString("N0"));

                            /*
                             * Ingreso Día
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.IncomeDailyQuintals.HasValue
                                    ? dailyRow.IncomeDailyQuintals
                                        .Value
                                        .ToString("N2")
                                    : string.Empty);

                            /*
                             * Ingreso Acumulado
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.IncomeAccumulatedQuintals.HasValue
                                    ? dailyRow.IncomeAccumulatedQuintals
                                        .Value
                                        .ToString("N2")
                                    : string.Empty);

                            /*
                             * Gasto Día
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.ConsumptionDailyQuintals
                                    .ToString("N2"));

                            /*
                             * Gasto Acumulado
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.ConsumptionAccumulatedQuintals
                                    .ToString("N2"));

                            /*
                             * Saldo Concentrado
                             */
                            AddDailyValueCell(
                                table,
                                dailyRow.ConcentrateBalanceQuintals
                                    .ToString("N2"));
                        }
                        else
                        {
                            /*
                             * Empty Visual Daily Row
                             *
                             * Rows 43 through 45 preserve the
                             * original printed template.
                             */

                            /*
                             * Fecha
                             */
                            AddDailyValueCell(
                                table,
                                string.Empty);

                            /*
                             * Día
                             */
                            AddDailyValueCell(
                                table,
                                rowNumber.ToString());

                            /*
                             * Remaining Daily Control Columns
                             */
                            for (var columnIndex = 0;
                                 columnIndex < 9;
                                 columnIndex++)
                            {
                                AddDailyValueCell(
                                    table,
                                    string.Empty);
                            }
                        }


                        /*
                         * =============================================
                         * Weekly Check Information
                         * =============================================
                         *
                         * Each group of seven Daily Check rows
                         * corresponds to one Weekly Check.
                         *
                         * Days  1 - 7  = Semana 1
                         * Days  8 - 14 = Semana 2
                         * Days 15 - 21 = Semana 3
                         * Days 22 - 28 = Semana 4
                         * Days 29 - 35 = Semana 5
                         * Days 36 - 42 = Semana 6
                         */
                        var weekIndex =
                            (rowNumber - 1) / 7;

                        var positionInWeek =
                            (rowNumber - 1) % 7;


                        if (weekIndex < weeklyChecks.Count)
                        {
                            var weeklyCheck =
                                weeklyChecks[weekIndex];

                            AddWeeklyControlCells(
                                table,
                                weeklyCheck,
                                positionInWeek);
                        }
                        else
                        {
                            /*
                             * Rows 43 through 45 do not belong
                             * to any of the six Weekly Checks.
                             */
                            for (var weeklyColumn = 0;
                                 weeklyColumn < 4;
                                 weeklyColumn++)
                            {
                                AddWeeklyControlCell(
                                    table,
                                    string.Empty);
                            }
                        }
                    }
                });
        }


        /*
         * PDF Helper | Daily Header Cell
         *
         * Applies the common style used by the multi-level
         * operational table headers.
         */
        private IContainer DailyHeaderCell(
            IContainer container)
        {
            return container
                .Border(0.7f)
                .PaddingVertical(2)
                .PaddingHorizontal(2)
                .AlignCenter()
                .AlignMiddle();
        }


        /*
         * PDF Helper | Daily Value Cell
         *
         * Applies the common style used by each Daily
         * Check information cell.
         */
        private void AddDailyValueCell(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Border(0.5f)
                .PaddingVertical(1.5f)
                .PaddingHorizontal(1)
                .AlignCenter()
                .AlignMiddle()
                .Text(text)
                .FontSize(7.2f);
        }


        /*
         * PDF Helper | Weekly Control Cells
         *
         * Places one Weekly Check beside its corresponding
         * group of seven Daily Check records.
         *
         * Position 0 displays the Week number and column labels.
         *
         * Positions 1 through 4 display:
         * Consumption, Weight, Conversion and Mortality.
         *
         * Positions 5 and 6 remain empty to preserve the
         * original seven-row weekly layout.
         */
        private void AddWeeklyControlCells(
            TableDescriptor table,
            BroodReportWeeklyViewModel weeklyCheck,
            int positionInWeek)
        {
            switch (positionInWeek)
            {
                /*
                 * Weekly Header
                 */
                case 0:

                    AddWeeklyControlCell(
                        table,
                        GetWeekNumber(
                            weeklyCheck.Week),
                        true);

                    AddWeeklyControlCell(
                        table,
                        "Esp.",
                        true);

                    AddWeeklyControlCell(
                        table,
                        "Real",
                        true);

                    AddWeeklyControlCell(
                        table,
                        "Dif.",
                        true);

                    break;


                /*
                 * Consumption
                 */
                case 1:

                    AddWeeklyControlCell(
                        table,
                        "Cons.",
                        true);

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ExpectedConsumption
                            .ToString("N3"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.RealConsumption
                            .ToString("N3"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ConsumptionDifference
                            .ToString("N3"));

                    break;


                /*
                 * Weight
                 */
                case 2:

                    AddWeeklyControlCell(
                        table,
                        "Peso",
                        true);

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ExpectedWeight
                            .ToString("N3"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.RealWeight
                            .ToString("N3"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.WeightDifference
                            .ToString("N3"));

                    break;


                /*
                 * Conversion
                 */
                case 3:

                    AddWeeklyControlCell(
                        table,
                        "Conv.",
                        true);

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ExpectedConversion
                            .ToString("N2"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.RealConversion
                            .ToString("N2"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ConversionDifference
                            .ToString("N2"));

                    break;


                /*
                 * Mortality
                 */
                case 4:

                    AddWeeklyControlCell(
                        table,
                        "% Mort.",
                        true);

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.ExpectedMortality
                            .ToString("N2"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.RealMortality
                            .ToString("N2"));

                    AddWeeklyControlCell(
                        table,
                        weeklyCheck.MortalityDifference
                            .ToString("N2"));

                    break;


                /*
                 * Remaining Weekly Rows
                 */
                default:

                    AddWeeklyControlCell(
                        table,
                        string.Empty);

                    AddWeeklyControlCell(
                        table,
                        string.Empty);

                    AddWeeklyControlCell(
                        table,
                        string.Empty);

                    AddWeeklyControlCell(
                        table,
                        string.Empty);

                    break;
            }
        }


        /*
         * PDF Helper | Weekly Control Cell
         *
         * Applies the common visual format used by
         * the Weekly Control section.
         */
        private void AddWeeklyControlCell(
            TableDescriptor table,
            string text,
            bool bold = false)
        {
            var cell =
                table.Cell()
                    .Border(0.5f)
                    .PaddingVertical(1.5f)
                    .PaddingHorizontal(1)
                    .AlignCenter()
                    .AlignMiddle();

            if (bold)
            {
                cell.Text(text)
                    .Bold()
                    .FontSize(7.0f);
            }
            else
            {
                cell.Text(text)
                    .FontSize(7.0f);
            }
        }


        /*
         * PDF Helper | Week Number
         *
         * Converts the stored week description into the
         * short numeric representation used by the original
         * printed report.
         *
         * Example:
         * "Semana 1" -> "1"
         */
        private string GetWeekNumber(
            string week)
        {
            if (string.IsNullOrWhiteSpace(
                week))
            {
                return string.Empty;
            }

            return week
                .Replace(
                    "Semana",
                    string.Empty,
                    StringComparison.OrdinalIgnoreCase)
                .Trim();
        }

        /*
         * PDF Section | Footer
         *
         * Generates the original closing sections displayed
         * at the bottom of the Brood Report.
         *
         * These fields remain empty because they are completed
         * later by the company that receives the birds.
         */
        private void ComposeFooter(
            ColumnDescriptor column)
        {
            column.Item()
                .PaddingTop(8)
                .Row(row =>
                {
                    /*
                     * Left Section | Bird Conditions
                     */
                    row.RelativeItem(1.7f)
                        .PaddingRight(5)
                        .Column(leftColumn =>
                        {
                            leftColumn.Item()
                                .Border(0.7f)
                                .PaddingVertical(2)
                                .AlignCenter()
                                .Text(
                                    "Condiciones de las aves ingresadas")
                                .Bold()
                                .FontSize(8);

                            leftColumn.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(0.8f); // Lote
                                        columns.RelativeColumn(1.0f); // Cantidad
                                        columns.RelativeColumn(0.8f); // Peso
                                        columns.RelativeColumn(0.8f); // % Nac.
                                        columns.RelativeColumn(1.1f); // Edad Reprod.
                                        columns.RelativeColumn(1.1f); // Incubadora
                                        columns.RelativeColumn(0.8f); // Huevo
                                        columns.RelativeColumn(0.8f); // Raza
                                    });

                                    /*
                                     * Header
                                     */
                                    AddFooterHeaderCell(
                                        table,
                                        "Lote");

                                    AddFooterHeaderCell(
                                        table,
                                        "Cantidad");

                                    AddFooterHeaderCell(
                                        table,
                                        "Peso");

                                    AddFooterHeaderCell(
                                        table,
                                        "% Nac.");

                                    AddFooterHeaderCell(
                                        table,
                                        "Edad Reprod.");

                                    AddFooterHeaderCell(
                                        table,
                                        "Incubadora");

                                    AddFooterHeaderCell(
                                        table,
                                        "Huevo");

                                    AddFooterHeaderCell(
                                        table,
                                        "Raza");


                                    /*
                                     * Empty Rows
                                     */
                                    for (var rowIndex = 0;
                                         rowIndex < 3;
                                         rowIndex++)
                                    {
                                        for (var columnIndex = 0;
                                             columnIndex < 8;
                                             columnIndex++)
                                        {
                                            AddFooterValueCell(
                                                table,
                                                string.Empty);
                                        }
                                    }
                                });
                        });


                    /*
                     * Right Section | Final Closing
                     */
                    row.RelativeItem(1.0f)
                        .PaddingLeft(5)
                        .Column(rightColumn =>
                        {
                            rightColumn.Item()
                                .Border(0.7f)
                                .PaddingVertical(2)
                                .AlignCenter()
                                .Text(
                                    "Cierre Final")
                                .Bold()
                                .FontSize(8);

                            rightColumn.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2.3f);
                                        columns.RelativeColumn(1.0f);
                                    });

                                    AddFooterLabelValueRow(
                                        table,
                                        "Aves entregadas a planta");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Kg entregados/planta");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Peso promedio/ave");

                                    AddFooterLabelValueRow(
                                        table,
                                        "% Mortalidad");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Q.Q. Ingresados");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Q.Q. Consumidos");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Q.Q. Sobrantes");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Consumo ave");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Conversión alimenticia");

                                    AddFooterLabelValueRow(
                                        table,
                                        "Índice de productividad");

                                    /*
                                     * Original Document Reference
                                     *
                                     * Displays the printed reference information located
                                     * below the Cierre Final section of the original form.
 */
                                    rightColumn.Item()
                                        .PaddingTop(3)
                                        .AlignRight()
                                        .Text(
                                            "LITO. SAN ALFONSO 2442-6161 / PER-06 / A.24-06-2020")
                                        .FontSize(5.5f);
                                });
                        });
                });


            /*
             * Signatures
             */
            column.Item()
                .PaddingTop(8)
                .Row(row =>
                {
                    row.RelativeItem()
                        .PaddingRight(10)
                        .Column(signatureColumn =>
                        {
                            signatureColumn.Item()
                                .Text(
                                    "Nombre del Granjero:")
                                .Bold()
                                .FontSize(7);

                            signatureColumn.Item()
                                .PaddingTop(8)
                                .BorderBottom(0.6f);
                        });


                    row.RelativeItem()
                        .PaddingLeft(10)
                        .Column(signatureColumn =>
                        {
                            signatureColumn.Item()
                                .Text(
                                    "Nombre del Supervisor:")
                                .Bold()
                                .FontSize(7);

                            signatureColumn.Item()
                                .PaddingTop(8)
                                .BorderBottom(0.6f);
                        });
                });
        }

        /*
         * PDF Helper | Footer Header Cell
         */
        private void AddFooterHeaderCell(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Border(0.5f)
                .PaddingVertical(2)
                .PaddingHorizontal(1)
                .AlignCenter()
                .AlignMiddle()
                .Text(text)
                .Bold()
                .FontSize(7);
        }


        /*
         * PDF Helper | Footer Value Cell
         */
        private void AddFooterValueCell(
            TableDescriptor table,
            string text)
        {
            table.Cell()
                .Border(0.5f)
                .PaddingVertical(3)
                .PaddingHorizontal(1)
                .AlignCenter()
                .AlignMiddle()
                .Text(text)
                .FontSize(7);
        }


        /*
         * PDF Helper | Footer Label and Empty Value
         */
        private void AddFooterLabelValueRow(
            TableDescriptor table,
            string label)
        {
            table.Cell()
                .Border(0.5f)
                .PaddingVertical(1.5f)
                .PaddingHorizontal(2)
                .Text(label)
                .FontSize(7);

            table.Cell()
                .Border(0.5f)
                .PaddingVertical(1.5f)
                .PaddingHorizontal(2)
                .Text(string.Empty);
        }







    }
}