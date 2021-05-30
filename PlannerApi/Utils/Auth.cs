namespace PlannerApi.Utils
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;
    using System.Net.Http.Headers;

    public static class Auth
    {
        public static AuthenticationHeaderValue GetAuthHeader(IHeaderDictionary headers) {
            StringValues authValues;
            headers.TryGetValue("Authorization", out authValues);

            return new AuthenticationHeaderValue("Bearer", authValues[0].Replace("Bearer ", ""));
        }
    }
}
