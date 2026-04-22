namespace HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int RecordId { get; set; }
        public int MedicineId { get; set; }
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }

        // Optional helper fields (populate via mapping if needed)
        public string? MedicineName { get; set; }
    }
}
