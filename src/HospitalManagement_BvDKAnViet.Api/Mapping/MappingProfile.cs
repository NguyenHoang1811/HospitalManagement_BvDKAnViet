using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DepartmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.DTOs.PrescriptionDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicineDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.AccountDTO;

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

        // Account / User mappings
        // Map User entity -> UserDto (include role name if Role navigation present)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.RoleName,
                opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : null));

        // Map CreateUserDto -> User.
        // NOTE: Do NOT map Password here to avoid storing plain text; repository should hash the password.
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.ExpriredTime, opt => opt.Ignore());

        // Map UpdateUserDto -> User. Keep password handling out of AutoMapper.
        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshToken, opt => opt.Ignore())
            .ForMember(dest => dest.ExpriredTime, opt => opt.Ignore());
    }
}