using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.WepApp.Models.DepartmentDTO
{
    public class UpdateDepartmentDto
    {
        [Required, StringLength(200)]
        public string DepartmentName { get; set; } = null!;
    }
}