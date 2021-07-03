using Domain.Helper;

namespace Domain.Enums
{
    public enum ErrorCodeEnum
    {
        [StringValue("Failed Http Request to third party")]
        FailedHttpRequestToService = 1,
        [StringValue("Could not connect with third party service.")]
        CouldNotConnectThirdPartyService = 2,
        [StringValue("the region iso is invalid, it should be required.")]
        RegionCodeShouldBeRequired = 2,
    }
}
