using AutoMapper;
using HospitalManagement_BvDKAnViet.Core.DTOs;
using HospitalManagement_BvDKAnViet.Core.DTOs.PatientDTO;
using HospitalManagement_BvDKAnViet.Core.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientDto>();
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();
    }
}