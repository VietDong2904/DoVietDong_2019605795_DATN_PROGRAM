using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.BaseAddress
{
    public interface IBaseAddressHandler
    {
        Task<Response> GetCity();
        Task<Response> GetDistrictByCity(string matp);
        Task<Response> GetCommuneByDistrict(string maqh);
    }
}
