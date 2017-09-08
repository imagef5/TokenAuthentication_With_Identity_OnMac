﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ApiClient.Helpers;
using ApiClient.DataServices.Base;
using ApiClient.DataServices.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiClient.DataServices
{
    public class AccountService : IAccountService
    {
        private IAccountProvider accountProvider;

        public AccountService(IAccountProvider provider)
        {
            accountProvider = provider;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
			
            var builder = new UriBuilder(AppConstants.AppTokenServer);
            builder.Path = "token";
            var uri = builder.ToString();
            JObject jwtDynamic = await accountProvider.GetTokenAsync<dynamic>(uri, username, password);

			var accessTokenExpiration = jwtDynamic.Value<DateTime>("expires");
			var accessToken = jwtDynamic.Value<string>("access_token");

            Settings.AuthTokenExpirationDate = accessTokenExpiration;

            #region Debug
            Debug.WriteLine(accessTokenExpiration);
			Debug.WriteLine(jwtDynamic);
            #endregion

            return accessToken;
        }
    }
}
