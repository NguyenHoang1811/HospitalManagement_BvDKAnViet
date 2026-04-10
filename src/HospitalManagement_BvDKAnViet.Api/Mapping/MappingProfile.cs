using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DoctorDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.DepartmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.AppointmentDTO;
using HospitalManagement_BvDKAnViet.Core.DTOs.MedicalRecordDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;
using HospitalManagement_BvDKAnViet.Core.Enums;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

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

        // Appointment mappings
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => src.AppointmentTime.ToString("HH:mm")))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.StatusName,
                opt => opt.MapFrom(src => Enum.GetName(typeof(AppointmentStatus), src.Status)));

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => TimeOnly.Parse(src.AppointmentTime)));
            

        CreateMap<UpdateAppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentTime,
                opt => opt.MapFrom(src => TimeOnly.Parse(src.AppointmentTime)))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status));


        // MedicalRecord mappings
        CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : null))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Name : null));

        CreateMap<CreateMedicalRecordDto, MedicalRecord>();
        CreateMap<UpdateMedicalRecordDto, MedicalRecord>();
    }
}