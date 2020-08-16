using AutoMapper;
using SFM.Models;
using SFM.Models.ViewModels;

namespace SFM
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ConfigViewModel, Config>();
            CreateMap<Config, ConfigViewModel>();
        }
    }
}