using Moq;
using Moq.Language.Flow;
using Moq.Protected;
using System.Net;
using System.Text.RegularExpressions;

namespace LocalVenue.Tests.Helpers
{
    // https://github.com/danielwarddev/MoqHttpExtensions
    public static class MockHttpMessageHandlerHelper
    {
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupSendAsync(this Mock<HttpMessageHandler> handler, HttpMethod requestMethod, string requestUrl)
        {
            return handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == requestMethod &&
                        r.RequestUri != null &&
                        r.RequestUri.ToString() == requestUrl
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
        
        public static ISetup<HttpMessageHandler, Task<HttpResponseMessage>> SetupSendAsyncRegex(this Mock<HttpMessageHandler> handler, HttpMethod requestMethod, string requestUrlRegex)
        {
            var regex = new Regex(requestUrlRegex);
            
            return handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == requestMethod &&
                        r.RequestUri != null &&
                        r.RequestUri.ToString() == regex.Match(r.RequestUri.ToString()).Value
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }

        public static IReturnsResult<HttpMessageHandler> ReturnsHttpResponseAsync(this ISetup<HttpMessageHandler, Task<HttpResponseMessage>> moqSetup, string responseBody, HttpStatusCode responseCode)
        {
            var stringContent = new StringContent(responseBody ?? string.Empty);

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = responseCode,
                Content = stringContent
            };

            return moqSetup.ReturnsAsync(responseMessage);
        }
    }
}