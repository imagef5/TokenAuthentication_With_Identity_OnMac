using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Models;

namespace ApiClient.DataServices.Interfaces
{
	/// <summary>
	/// UserService Interfce
	/// </summary>
	public interface IUserService
	{
		/// <summary>
		/// 사용자 리스트 가져오기(페이지 단위)
		/// </summary>
        /// <param name="token">Api Token</param>
		/// <param name="page">페이지 번호</param>
		/// <returns>사용자 리스트 </returns>
		Task<IList<User>> GetUsersAsync(string token , int? page = 1); 
	}
}
