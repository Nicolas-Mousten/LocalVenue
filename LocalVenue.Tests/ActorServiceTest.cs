using LocalVenue.Services;
using LocalVenue.Tests.Helpers;
using Moq;

namespace LocalVenue.Tests
{
    public class ActorServiceTest
    {
        [Fact]
        public async Task TestActorServiceGetRandomActors()
        {
            // Arrange
            const int expectedCountMin = 3;
            const int expectedCountMax = 12;

            // Act
            var mockFactory = HttpClientFactoryHelper.GetActorServiceMockClientFactory();

            var actorService = new ActorService(mockFactory.Object);

            try
            {
                var result = await actorService.GetRandomActors();

                // Assert
                Assert.NotNull(result);
                Assert.True(result.Count >= expectedCountMin);
                Assert.True(result.Count <= expectedCountMax);
                Assert.All(result, actor => Assert.NotEqual(actor.Name, string.Empty));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
