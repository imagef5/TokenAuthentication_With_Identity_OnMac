using System;
using System.Threading.Tasks;

namespace ApiClient.DataServices.Base
{
    public interface IAccountProvider
    {
        Task<TResult> GetTokenAsync<TResult>(string uri, string username, string password);
    }
}
