using AutoMapper;
using TasksMaster.DTOs;

namespace TasksMaster.Mappers
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            // 1. Mapping voor de Respons: Task (Domain) -> TaskReadDto
            // Hier filter je de interne velden (zoals CreatedBy)
            CreateMap<Domain.Task, TaskReadDto>();

            // 2. Mapping voor Creatie: TaskCreateDto -> Task (Domain)
            // Dit map je om de Domain.Task aan te maken vanuit de input
            CreateMap<TaskCreateDto, Domain.Task>();

            // 3. Mapping voor Update (optioneel, maar goed om te hebben voor PUT/PATCH)
            // We mappen de DTO naar een *bestaande* entiteit.
            CreateMap<TaskUpdateDto, Domain.Task>();
        }
    }
}
