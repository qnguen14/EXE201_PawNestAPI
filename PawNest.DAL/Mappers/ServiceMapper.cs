using Riok.Mapperly.Abstractions;
using PawNest.DAL.Data.Entities;
using PawNest.DAL.Data.Requests.Service;
using PawNest.DAL.Data.Responses.Service;

namespace PawNest.DAL.Mappers;

[Mapper]
public partial class ServiceMapper
{
    // CreateServiceRequest to Service
    public partial Service MapToService(CreateServiceRequest request);
    
    // Service to GetServiceResponse
    [MapProperty(nameof(Service.ServiceId), nameof(GetServiceResponse.Id))]
    public partial GetServiceResponse MapToGetServiceResponse(Service service);
    
    // IEnumerable mapping
    public partial IEnumerable<GetServiceResponse> MapToGetServiceResponseList(IEnumerable<Service> services);
    
    // For update - map request to existing service
    public partial void UpdateServiceFromRequest(UpdateServiceRequest request, Service target);
    
    // Handle ServiceType enum to string conversion
    private string MapServiceType(ServiceType type) => type.ToString();
}
