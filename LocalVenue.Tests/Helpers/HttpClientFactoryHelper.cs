using System.Net;
using System.Text.RegularExpressions;
using Moq;

namespace LocalVenue.Tests.Helpers
{
    public static class HttpClientFactoryHelper
    {
        private static string BaseAddress = "https://api.themoviedb.org/3/";
        private static string LatestMovieUri = "movie/latest";
        private static string MovieCreditsUriRegex =
            @"^https://api\.themoviedb\.org/3/movie/\d{1,19}/credits$";
        private static string HttpClientName = "TmdbClient";
        private static string LatestMovieJson = ReadJsonFile("Helpers/jsonLatestMovie.json");
        private static string MovieCreditsJson = ReadJsonFile("Helpers/jsonMovieCredits.json");

        public static Mock<IHttpClientFactory> GetActorServiceMockClientFactory()
        {
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .SetupSendAsync(HttpMethod.Get, BaseAddress + LatestMovieUri)
                .ReturnsHttpResponseAsync(LatestMovieJson, HttpStatusCode.OK);

            httpMessageHandler
                .SetupSendAsyncRegex(HttpMethod.Get, MovieCreditsUriRegex)
                .ReturnsHttpResponseAsync(MovieCreditsJson, HttpStatusCode.OK);

            var httpClient = new HttpClient(httpMessageHandler.Object, false)
            {
                BaseAddress = new Uri(BaseAddress),
            };

            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory
                .Setup(_ => _.CreateClient(It.Is<string>(i => i == HttpClientName)))
                .Returns(httpClient);

            return mockFactory;
        }

        private static string ReadJsonFile(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string jsonString = reader.ReadToEnd();
                return jsonString;
            }
        }
    }
}
