// KidneyPredictionRequestDto.cs
namespace HospitalManagement_BvDKAnViet.Core.DTOs.KidneyPrediction
{
    public class KidneyPredictionRequestDto
    {
        public int PatientId { get; set; }
        public int? Age { get; set; }
        public double? BloodPressure { get; set; }
        public double? SpecificGravity { get; set; }
        public double? Albumin { get; set; }
        public double? Sugar { get; set; }
        public string? RedBloodCells { get; set; }    // "normal" | "abnormal"
        public string? PusCell { get; set; }
        public string? PusCellClumps { get; set; }   // "present" | "notpresent"
        public string? Bacteria { get; set; }
        public double? BloodGlucoseRandom { get; set; }
        public double? BloodUrea { get; set; }
        public double? SerumCreatinine { get; set; }
        public double? Sodium { get; set; }
        public double? Potassium { get; set; }
        public double? Hemoglobin { get; set; }
        public double? PackedCellVolume { get; set; }
        public double? WhiteBloodCellCount { get; set; }
        public double? RedBloodCellCount { get; set; }
        public string? Hypertension { get; set; }    // "yes" | "no"
        public string? Diabetes { get; set; }
        public string? CoronaryArteryDisease { get; set; }
        public string? Appetite { get; set; }        // "good" | "poor"
        public string? PedalEdema { get; set; }
        public string? Anemia { get; set; }
    }
}