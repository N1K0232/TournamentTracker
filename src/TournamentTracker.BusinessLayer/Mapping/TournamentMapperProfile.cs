using AutoMapper;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Mapping;

public class TournamentMapperProfile : Profile
{
    public TournamentMapperProfile()
    {
        CreateMap<Entities.Tournament, Tournament>();
        CreateMap<SaveTournamentRequest, Entities.Tournament>();
    }
}