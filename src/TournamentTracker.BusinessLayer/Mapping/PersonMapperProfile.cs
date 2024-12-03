using AutoMapper;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Mapping;

public class PersonMapperProfile : Profile
{
    public PersonMapperProfile()
    {
        CreateMap<Entities.Person, Person>();
        CreateMap<SavePersonRequest, Entities.Person>().ForMember(p => p.Team, options => options.Ignore());
    }
}