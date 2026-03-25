using System;

namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int PatientId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }

        public Patient? Patient { get; set; }
    }
}