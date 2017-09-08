using System;
using System.Threading.Tasks;

namespace ApiClient.DataServices.Interfaces
{
    public interface IAccountService
    {
		// <summary>
		/// 사용자 리스트 가져오기(페이지 단위)
		/// </summary>
		/// <param name="username">username</param>
		/// <param name="password">password</param>
		/// <returns>token value</returns>
		Task<string> LoginAsync(string username, string password);
	}
}
