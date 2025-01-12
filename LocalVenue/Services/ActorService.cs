using AutoMapper;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using LocalVenue.Core.Entities;
using LocalVenue.Core.Models;
using LocalVenue.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LocalVenue.Services;

public class ActorService(IHttpClientFactory httpClientFactory) : IActorService
{
    public async Task<List<Actor>> GetRandomActors()
    {
        using HttpClient client = httpClientFactory.CreateClient("TmdbClient");
        
        var maxMovieId = await GetMaxMovieId(client);
        
        var response = new HttpResponseMessage();
        response.StatusCode = HttpStatusCode.NotFound;

        var random = new Random();
        
        while (response.StatusCode == HttpStatusCode.NotFound)
        {
            var randomMovieId = random.NextInt64(1, maxMovieId);
            
            try
            {
                response = await client.GetAsync("movie/" + randomMovieId + "/credits");
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // run the while loop again
                    continue;
                }

                var actors = new List<Actor>();
                
                try
                {
                    actors = JsonDocument.Parse(response.Content.ReadAsStringAsync(CancellationToken.None).Result).
                        RootElement.GetProperty("cast").
                        Deserialize<List<Actor>>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
                }
                catch (Exception e)
                {
                    // the "cast" array is empty, or a field in an actor is empty
                    // run the while loop again
                    response.StatusCode = HttpStatusCode.NotFound;
                    continue;
                }
                
                if (actors.IsNullOrEmpty())
                {
                    // run the while loop again
                    response.StatusCode = HttpStatusCode.NotFound;
                    continue;
                }
                
                if (actors.Count < 3)
                {
                    // run the while loop again
                    response.StatusCode = HttpStatusCode.NotFound;
                    continue;
                }

                return actors.Count > 12 ? actors.Take(12).ToList() : actors;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        throw new UnreachableException();
    }

    private async Task<long> GetMaxMovieId(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync("movie/latest");
            JsonDocument.Parse(response.Content.ReadAsStringAsync(CancellationToken.None).Result).RootElement.GetProperty("id").TryGetInt64(out var maxMovieId);
            return maxMovieId;
        }
        catch (Exception e)
        {
            throw new HttpRequestException("Did not get a response from TMDB api.");
        }
    }
}