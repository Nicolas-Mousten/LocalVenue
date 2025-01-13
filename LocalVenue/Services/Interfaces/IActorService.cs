using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;

namespace LocalVenue.Services.Interfaces;

public interface IActorService
{
    public Task<List<Actor>> GetRandomActors();
}
