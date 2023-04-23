using API.DTOs;
using API.DTOs.response;
using API.Models;
using AutoMapper;

namespace API.Profiles;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<TbMEmployee, EmployeeDTO>();
        CreateMap<TbMEmployee, EmployeesMasterResponse>()
            .ForMember(dest => dest.Education, opt => opt.MapFrom(src => src.TbTrProfiling.Education))
            .ForMember(dest => dest.University, opt => opt.MapFrom(src => src.TbTrProfiling.Education.University));
    }
}