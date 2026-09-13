using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Hubcon;

namespace HubconTestDomain;

[HttpTransport]
public interface IChildUserContract : IUserContract
{
    public new Task<HubconResponse<TestInputClass>> GetTemperatureFromServerWithInput([Required] TestInputClass input, CancellationToken cancellationToken = default);
}