namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int RecordId { get; set; }
        public int MedicineId { get; set; }
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }
        public Medicine? Medicine { get; set; }
    }
}