using DigitalMarketplace.Core.DTOs;

namespace DigitalMarketplace.Core.Services;
public interface IStorageService
{
    Task<ServiceResponse<string>> Upload(Stream inputStream);
}
