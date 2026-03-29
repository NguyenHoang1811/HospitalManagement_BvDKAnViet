using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.DTOs.DepartmentDTO
{
    public class CreateDepartmentDto
    {
        [Required, StringLength(200)]
        public string DepartmentName { get; set; } = null!;
    }
}