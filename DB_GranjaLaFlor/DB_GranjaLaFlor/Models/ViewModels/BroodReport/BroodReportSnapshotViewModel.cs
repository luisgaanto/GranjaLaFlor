namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Architecture Decision | Brood Report Snapshot
     *
     * Represents the complete historical information required
     * to reproduce a generated Brood Report.
     *
     * The snapshot is serialized and stored in BroodReportData
     * so the generated report preserves the information that
     * existed at the moment it was created.
     */
    public class BroodReportSnapshotViewModel
    {
        /*
         * Header information displayed in the printed report.
         */
        public BroodReportHeaderViewModel Header { get; set; }
            = new();

        /*
         * Daily operational information displayed in the
         * Control de Aves and Control de Alimento sections.
         *
         * Normally six production weeks provide 42 Daily Checks.
         * The printed report may display up to 45 visual rows.
         */
        public List<BroodReportDailyRowViewModel> DailyRows { get; set; }
            = new();

        /*
         * Weekly production information displayed in the
         * Control Semanal section.
         *
         * A complete Brood Report contains six Weekly Checks.
         */
        public List<BroodReportWeeklyViewModel> WeeklyChecks { get; set; }
            = new();
    }
}