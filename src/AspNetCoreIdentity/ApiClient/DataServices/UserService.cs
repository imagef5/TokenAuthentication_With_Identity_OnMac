using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.DataServices.Base;
using ApiClient.DataServices.Interfaces;
using ApiClient.Models;

namespace ApiClient.DataServices
{
	/// <summary>
	/// 사용자 정보 관련 Service class
	/// </summary>
	public class UserService : IUserService
	{
		private IRequestProvider requestProvider;
		public UserService(IRequestProvider provider)
		{
			requestProvider = provider;
		}

		/// <summary>
		/// 사용자 리스트 가져오기(페이지 단위)
		/// </summary>
        /// <param name="token">API Token</param>
		/// <param name="page">페이지 번호</param>
		/// <returns>사용자 리스트 </returns>
		public async Task<IList<User>> GetUsersAsync(string token , int? page = 1)
		{
			var builder = new UriBuilder(AppConstants.AppApiServer);
			builder.Path = "api/users";
			string param = $@"?page={page}";

			var uri = builder.ToString() + param;
            var users = await requestProvider.GetAsync<Users>(uri, token);
			return users.Results;
		} 
	}
}
