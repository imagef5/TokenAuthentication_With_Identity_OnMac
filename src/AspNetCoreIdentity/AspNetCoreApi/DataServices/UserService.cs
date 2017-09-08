using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreApi.Configures;
using AspNetCoreApi.DataServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AspNetCoreApi.DataServices
{
    public class UserService : IUserService
    {
        private readonly string userApiUri;
        public UserService(IOptions<UsersApiServiceOptions> options)
        {
            userApiUri = options.Value.UsersApiServer;
        }

        public async Task<string> GetUsersAsync(int? page = 1, int? results = 10, string seed = "netcoreapi", bool? isIncludeInfo = false)
        {
			var builder = new UriBuilder(userApiUri);
			builder.Path = "api/";
			string noinfo = isIncludeInfo.Value ? string.Empty : "&noinfo";
			string param = $@"?page={page}&results={results}&seed={seed}" + noinfo;

			var uri = builder.ToString() + param;
			var users = await GetAsync(uri);
			return users;
        }

		private async Task<string> GetAsync(string uri)
		{
			//HttpClient httpClient = CreateHttpClient();
			HttpResponseMessage response = new HttpResponseMessage();
			try
			{
				using (var httpClient = CreateHttpClient())
				{
					httpClient.Timeout = TimeSpan.FromSeconds(20);
					var cancelTokenSource = new CancellationTokenSource();
					var cancelToken = cancelTokenSource.Token;

					response = await httpClient.GetAsync(uri, cancelToken).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error in: {ex}");
			}

			await HandleResponse(response);

			string serialized = await response.Content.ReadAsStringAsync();
			return serialized;
		}

		/// <summary>
		/// HttpClient 초기화
		/// </summary>
		/// <returns>HttpClient Instance</returns>
		private HttpClient CreateHttpClient()
		{
			var httpClient = new HttpClient();

			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return httpClient;
		}

		/// <summary>
		/// 네트웍 에러 핸들링
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		private async Task HandleResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				throw new HttpRequestException(content);
			}
		}
    }
}
