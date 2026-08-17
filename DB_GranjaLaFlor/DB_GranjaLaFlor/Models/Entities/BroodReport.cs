using DB_GranjaLaFlor.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGranjaLaFlor.Models
{
    /*
     * Entity Model | Represents the brood_reports table.
     *
     * Each record stores the report identification information,
     * Brood relationship and serialized historical snapshot
     * used by Details and PDF generation.
     */
    [Table("brood_reports")]
    public class BroodReport
    {
        /*
         * Primary Key | Brood Report
         */
        [Key]
        [Column("brood_report_id")]
        [Display(Name = "ID")]
        public int BroodReportId { get; set; }

        /*
         * Report number entered by the user.
         *
         * The number may be repeated between different reports.
         */
        [Required]
        [Column("report_number")]
        [Display(Name = "Nº")]
        public int ReportNumber { get; set; }

        /*
         * Date and time when the historical report
         * snapshot was generated.
         *
         * This value is used by the application history
         * and is not printed as part of the original report header.
         */
        [Required]
        [Column("brood_report_generated_at")]
        [Display(Name = "Fecha de generación")]
        public DateTime BroodReportGeneratedAt { get; set; }

        /*
         * Automatic historical version assigned to reports
         * generated from the same Brood.
         */
        [Required]
        [Column("brood_report_version")]
        [Display(Name = "Versión")]
        public int BroodReportVersion { get; set; }

        /*
         * Historical Snapshot
         *
         * Stores the serialized BroodReportSnapshotViewModel.
         *
         * The snapshot preserves the report information
         * exactly as it existed when the report was generated.
         */
        [Required]
        [Column(
            "brood_report_data",
            TypeName = "longtext")]
        public string BroodReportData { get; set; }
            = string.Empty;

        /*
         * Foreign Key | Brood
         *
         * Identifies the Brood associated with
         * the generated historical report.
         */
        [Required]
        [Column("brood_id")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        /*
         * Navigation Property | Brood
         */
        [ForeignKey(nameof(BroodId))]
        public Brood Brood { get; set; }
            = null!;
    }
}