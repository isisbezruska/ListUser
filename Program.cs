using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GraphAPIListUser
{
    class Program
    {
        private const string clientId = "e8027724-a2e0-4b44-8c13-557d77af647f";
        private const string aadInstance = "https://login.microsoftonline.com/";
        private const string tenant = "302de125-622a-4ac3-a029-4431603ffed3";
        private const string resource = "https://graph.microsoft.com/";
        private const string appKey = "-vlv1P8H18.mDi.gXudAJkA_51Fr8zFQCL";

        static string authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

        private static HttpClient httpClient = new HttpClient();
        private static AuthenticationContext context = null;
        private static ClientCredential credential = null;

        static void Main(string[] args)
        {
            context = new AuthenticationContext(authority + tenant);
            credential = new ClientCredential(clientId, appKey);

            Task<string> accessToken = GetToken();
            accessToken.Wait();
            Console.WriteLine(accessToken.Result);

            Task<string> users = GetUsers(accessToken.Result);
            users.Wait();

            Console.WriteLine(users.Result);
            Console.ReadLine();
        }

        private static async Task<string> GetUsers(string accessToken)
        {
            var url = "https://graph.microsoft.com/v1.0/users";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + "Obtendo informacoes de " + url + " da Graph API..." + "\n");
            Console.ResetColor();

            string users = null;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var getResult = await httpClient.GetAsync(url);

            if (getResult.Content != null)
            {
                users = await getResult.Content.ReadAsStringAsync();
            }

            return users;
        }

        private static async Task<string> GetToken()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" + "Obtendo token de acesso da Azure AD..." + "\n");
            Console.ResetColor();

            AuthenticationResult result = null;
            string accessToken = null;
            result = await context.AcquireTokenAsync(resource, credential);
            accessToken = result.AccessToken;
            return accessToken;
        }

    }
}