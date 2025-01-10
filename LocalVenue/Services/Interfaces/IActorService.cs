using LocalVenue.Core.Entities;

namespace LocalVenue.Services.Interfaces;

public interface IActorService
{
    public Task<List<Actor>> GetRandomActors();
}