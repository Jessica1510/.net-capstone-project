using AutoMapper;
using Certification_microservice.Models;
using Certification_microservice.DTO;

namespace Certification_microservice.Profiles
{
    public class CertificationProfile : Profile
    {
        public CertificationProfile()
        {
            CreateMap<CertificationDTO, Certification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Certification, CertificationDTO>();
        }
    }
}
