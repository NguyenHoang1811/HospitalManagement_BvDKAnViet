// KidneyPredictionResponseDto.cs
namespace HospitalManagement_BvDKAnViet.Core.DTOs.KidneyPrediction
{
    public class KidneyPredictionResponseDto
    {
        public int PredictionId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? DoctorId { get; set; }      
        public string? DoctorName { get; set; } 
        public string? PredictionResult { get; set; }  // "CKD" | "Not CKD"
        public double ProbabilityCkd { get; set; }
        public double ProbabilityNotCkd { get; set; }
        public string? RiskLevel { get; set; }         // "HIGH" | "MEDIUM" | "LOW"
        public DateTime CreatedAt { get; set; }
    }
}