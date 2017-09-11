using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace ApiClient.DataServices.Base
{
    public class AccountProvider : IAccountProvider
    {
        private readonly JsonSerializerSettings serializerSettings;

        public AccountProvider(IJsonSettings jsonSettings)
        {
            //Json Serialize 초기값 설정
            serializerSettings = jsonSettings.JsonSerializerSettings;
        }

        public async Task<TResult> GetTokenAsync<TResult>(string uri, string username, string password)
        {
            #region Case : Send Form Data
            var keyValues = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", password),
				//new KeyValuePair<string, string>("grant_type", "password")
			};

			var sendValue  = new FormUrlEncodedContent(keyValues);
            #endregion

            #region Case : Send Json Data in Body
            //var jsonValues = new { username = username, password = password };
            //string postBody = JsonConvert.SerializeObject(jsonValues);
            //var sendValue = new StringContent(
            //                        postBody,
            //                        Encoding.UTF8,
            //                        "application/json");
            #endregion

            HttpResponseMessage response = new HttpResponseMessage();
			try
			{
				using (var httpClient = new HttpClient())
				{
					httpClient.Timeout = TimeSpan.FromSeconds(20);
					//var cancelTokenSource = new CancellationTokenSource();
					//var cancelToken = cancelTokenSource.Token;

                    response = await httpClient.PostAsync(uri, sendValue);//, cancelToken);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error in: {ex}");
			}

            if(!response.IsSuccessStatusCode)
            {
				var message = await response.Content.ReadAsStringAsync();

				throw new HttpRequestException(message);
            }
			
			string content = await response.Content.ReadAsStringAsync();
            TResult result = await Task.Run(() => JsonConvert.DeserializeObject<TResult>(content, serializerSettings));

            return result;
        }
    }

}
