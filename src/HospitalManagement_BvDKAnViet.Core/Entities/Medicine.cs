namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public ICollection<Prescription>? Prescriptions { get; set; }
    }
}