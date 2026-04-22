using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO
{
    public class CreatePatientDto
    {
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string MedicalHistory { get; set; }
    }
}
