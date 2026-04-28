using Microsoft.EntityFrameworkCore;
using HospitalManagement_BvDKAnViet.Core.Entities;

namespace HospitalManagement_BvDKAnViet.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Doctor> Doctors { get; set; } = null!;
        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<KidneyPrediction> KidneyPredictions { get; set; } = null!;
        public DbSet<Medicine> Medicines { get; set; } = null!;
        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== MAP TABLE =====
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Patient>().ToTable("Patient");
            modelBuilder.Entity<Doctor>().ToTable("Doctor");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Appointment>().ToTable("Appointment");
            modelBuilder.Entity<MedicalRecord>().ToTable("MedicalRecord");
            modelBuilder.Entity<KidneyPrediction>().ToTable("KidneyPrediction");
            modelBuilder.Entity<Medicine>().ToTable("Medicine");
            modelBuilder.Entity<Prescription>().ToTable("Prescription");
            modelBuilder.Entity<Invoice>().ToTable("Invoice");

            // ===== RELATIONSHIP =====

            modelBuilder.Entity<KidneyPrediction>()
                .HasOne(k => k.Patient)
                .WithMany()
                .HasForeignKey(k => k.PatientId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.MedicalRecord)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.RecordId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Medicine)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.MedicineId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== DATE/TIME CONFIG =====
            modelBuilder.Entity<Patient>()
                .Property(p => p.DateOfBirth)
                .HasColumnType("date");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.AppointmentDate)
                .HasColumnType("date");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.AppointmentTime)
                .HasColumnType("time");

            // ===== DEFAULT VALUE =====
            modelBuilder.Entity<Patient>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<MedicalRecord>()
                .Property(m => m.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<KidneyPrediction>()
                .Property(k => k.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PatientId)
                .OnDelete(DeleteBehavior.SetNull);
            // Patient - User (1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.User)
                .HasForeignKey<User>(u => u.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Doctor - User (1-1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.User)
                .HasForeignKey<User>(u => u.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PatientId)
                .IsUnique()
                .HasFilter("[PatientId] IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.DoctorId)
                .IsUnique()
                .HasFilter("[DoctorId] IS NOT NULL");
        }
    }
}