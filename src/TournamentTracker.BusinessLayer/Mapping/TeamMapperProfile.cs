using AutoMapper;
using Entities = TournamentTracker.DataAccessLayer.Entities;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;

namespace TournamentTracker.BusinessLayer.Mapping;

public class TeamMapperProfile : Profile
{
    public TeamMapperProfile()
    {
        CreateMap<Entities.Team, Team>();
        CreateMap<SaveTeamRequest, Entities.Team>();
    }
}