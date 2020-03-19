using Abp.AutoMapper;
using AcmStatisticsBackend.Authentication.External;

namespace AcmStatisticsBackend.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
