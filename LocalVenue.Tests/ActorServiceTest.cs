using LocalVenue.Services;
using LocalVenue.Tests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;

namespace LocalVenue.Tests
{
    public class ActorServiceTest
    {
        private const int ExpectedCountMin = 3;
        private const int ExpectedCountMax = 12;

        [Fact]
        public async Task TestMockActorServiceGetRandomActors()
        {
            // Arrange
            var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();

            var actorService = new ActorService(mockFactory.Object);

            try
            {
                // Act
                var result = await actorService.GetRandomActors();

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Count >= ExpectedCountMin);
                Assert.True(result.Count <= ExpectedCountMax);
                Assert.All(result, actor => Assert.NotEqual(actor.Name, string.Empty));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [IgnoreOnBuildServerFact]
        public async Task TestLiveActorServiceGetRandomActors()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = WebApplication.CreateBuilder();
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json");

            static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                        retryAttempt)));
            }

            services.AddHttpClient("TmdbClient", client =>
                {
                    client.BaseAddress = new Uri(builder.Configuration.GetSection("TMDB").GetSection("Url").Value ?? throw new ArgumentNullException("TMDB.Url"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", builder.Configuration.GetSection("TMDB").GetSection("Token").Value ?? throw new ArgumentNullException("TMDB.Token"));
                })
                .AddPolicyHandler(GetRetryPolicy());

            var serviceProvider = services.BuildServiceProvider();
            var iHttpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var actorService = new ActorService(iHttpClientFactory!);

            try
            {
                // Act
                var result = await actorService.GetRandomActors();

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Count >= ExpectedCountMin);
                Assert.True(result.Count <= ExpectedCountMax);
                Assert.All(result, actor => Assert.NotEqual(actor.Name, string.Empty));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
