using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DepartmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Appointment mapping (string <-> TimeOnly handled explicitly here)
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => src.AppointmentTime.ToString("HH:mm")));

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => TimeOnly.Parse(src.AppointmentTime)));

        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => TimeOnly.Parse(src.AppointmentTime)));

        // Patient mappings
        CreateMap<Patient, PatientDto>();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();

        // Doctor mappings
        CreateMap<Doctor, DoctorDto>();
        CreateMap<CreateDoctorDto, Doctor>();
        CreateMap<UpdateDoctorDto, Doctor>();

        // Department mappings
        CreateMap<Department, DepartmentDto>();
        CreateMap<CreateDepartmentDto, Department>();
        CreateMap<UpdateDepartmentDto, Department>();

        // MedicalRecord mappings
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : null))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

        CreateMap<CreateMedicalRecordDto, MedicalRecord>();
        CreateMap<UpdateMedicalRecordDto, MedicalRecord>();

        // Prescription mappings
        CreateMap<Prescription, PrescriptionDto>()
            .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.Medicine != null ? src.Medicine.Name : null));

        CreateMap<CreatePrescriptionDto, Prescription>();
        CreateMap<UpdatePrescriptionDto, Prescription>();

        // Medicine mappings
        CreateMap<Medicine, MedicineDto>();
        CreateMap<CreateMedicineDto, Medicine>();
        CreateMap<UpdateMedicineDto, Medicine>();
    }
}