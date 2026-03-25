using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement_BvDKAnViet.Core.Entities
{
    public class KidneyPrediction
    {
        [Key]
        public int PredictionId { get; set; }
        public int PatientId { get; set; }

        public int? Age { get; set; }
        public double? BloodPressure { get; set; }
        public double? SpecificGravity { get; set; }
        public double? Albumin { get; set; }
        public double? Sugar { get; set; }

        public string? RedBloodCells { get; set; }
        public string? PusCell { get; set; }
        public string? PusCellClumps { get; set; }
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

        public string? Hypertension { get; set; }
        public string? Diabetes { get; set; }
        public string? CoronaryArteryDisease { get; set; }

        public string? Appetite { get; set; }
        public string? PedalEdema { get; set; }
        public string? Anemia { get; set; }

        public string? PredictionResult { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Patient? Patient { get; set; }
    }
}