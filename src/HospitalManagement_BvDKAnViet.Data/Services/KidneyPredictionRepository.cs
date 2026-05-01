using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.IServices;
using HospitalManagement_BvDKAnViet.Data.Context;

namespace HospitalManagement_BvDKAnViet.Data.Repositories
{
    public class KidneyPredictionRepository : IKidneyPredictionRepository
    {
        private readonly ApplicationDbContext _db;
        public KidneyPredictionRepository(ApplicationDbContext db) => _db = db;

        public async Task<KidneyPrediction> AddAsync(KidneyPrediction prediction)
        {
            prediction.CreatedAt = DateTime.Now;
            var entry = await _db.KidneyPredictions.AddAsync(prediction);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IEnumerable<KidneyPrediction>> GetByPatientIdAsync(int patientId)
        {
            return await _db.KidneyPredictions
                .AsNoTracking()
                .Where(p => p.PatientId == patientId)
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<KidneyPrediction?> GetByIdAsync(int id)
        {
            return await _db.KidneyPredictions
                .AsNoTracking()
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.PredictionId == id);
        }
        public async Task<IEnumerable<KidneyPrediction>> GetByDoctorIdAsync(int doctorId)
        {
            return await _db.KidneyPredictions
                .AsNoTracking()
                .Where(p => p.DoctorId == doctorId)
                .Include(p => p.Patient)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}