using AutoMapper;
using TournamentTracker.Shared.Models;
using Entities = TournamentTracker.DataAccessLayer.Entities;

namespace TournamentTracker.BusinessLayer.Mapping;

public class ImageMapperProfile : Profile
{
    public ImageMapperProfile()
    {
        CreateMap<Entities.Image, Image>();
    }
}