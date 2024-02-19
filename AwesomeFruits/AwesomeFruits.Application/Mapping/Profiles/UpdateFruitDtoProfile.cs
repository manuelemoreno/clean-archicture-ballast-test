using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Application.Mapping.Profiles;

public class UpdateFruitDtoProfile : Profile
{
    public UpdateFruitDtoProfile()
    {
        CreateMap<UpdateFruitDto, Fruit>()
            .ForMember(dest =>
                    dest.Name,
                opt => opt.MapFrom(src => src.Name))
            .ForMember(dest =>
                    dest.Description,
                opt => opt.MapFrom(src => src.Description));
    }
}