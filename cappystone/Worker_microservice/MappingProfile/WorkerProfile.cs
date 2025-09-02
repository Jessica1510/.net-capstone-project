using AutoMapper;
using Worker_microservice.Models.DTO;

using Worker_microservice.Models;

namespace Worker_microservice.MappingProfile
{
    public class WorkerProfile : Profile
    {
        public WorkerProfile()
        {
            CreateMap<Create, Worker>().ReverseMap();
            CreateMap<Update, Worker>().ReverseMap();
        }

    }
}
