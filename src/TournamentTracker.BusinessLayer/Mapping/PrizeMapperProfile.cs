using AutoMapper;
using TournamentTracker.Shared.Models;
using TournamentTracker.Shared.Models.Requests;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Mapping;

public class PrizeMapperProfile : Profile
{
    public PrizeMapperProfile()
    {
        CreateMap<Entities.Prize, Prize>();
        CreateMap<SavePrizeRequest, Entities.Prize>().ForMember(p => p.Tournament, options => options.Ignore());
    }
}