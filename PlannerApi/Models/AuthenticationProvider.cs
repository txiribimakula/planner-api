using Microsoft.Graph;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class AuthenticationProvider : IAuthenticationProvider
{
    public AuthenticationHeaderValue HeaderValue { get; set; }

    public AuthenticationProvider(AuthenticationHeaderValue headerValue) {
        HeaderValue = headerValue;
    }

    public Task AuthenticateRequestAsync(HttpRequestMessage request) {
        request.Headers.Authorization = HeaderValue;

        return Task.FromResult(0);
    }
}