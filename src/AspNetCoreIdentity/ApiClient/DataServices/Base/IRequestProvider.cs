using System.Threading.Tasks;

namespace ApiClient.DataServices.Base
{
	public interface IRequestProvider
	{
		Task<TResult> GetAsync<TResult>(string uri, string token);
	}
}
