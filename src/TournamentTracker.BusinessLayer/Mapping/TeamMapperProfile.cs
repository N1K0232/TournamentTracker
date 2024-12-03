using AutoMapper;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Mapping;

public class TeamMapperProfile : Profile
{
    public TeamMapperProfile()
    {
        CreateMap<Entities.Team, Team>();
        CreateMap<SaveTeamRequest, Entities.Team>().ForMember(t => t.Tournament, options => options.Ignore());
    }
}