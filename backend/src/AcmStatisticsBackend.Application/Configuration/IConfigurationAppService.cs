using System.Threading.Tasks;
using AcmStatisticsBackend.Configuration.Dto;

namespace AcmStatisticsBackend.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
