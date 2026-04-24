using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.PrescriptionDTO   
{
    public class CreatePrescriptionDto
    {
        [Required]
        public int RecordId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        [StringLength(200)]
        public string? Dosage { get; set; }

        [StringLength(2000)]
        public string? Instructions { get; set; }
    }
}
