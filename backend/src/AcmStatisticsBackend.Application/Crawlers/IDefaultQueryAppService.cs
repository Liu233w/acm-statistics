using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 用来管理默认用户名（自动在查询页面填写的用户名）
    /// </summary>
    public interface IDefaultQueryAppService : IApplicationService
    {
        /// <summary>
        /// 获取默认用户名
        /// </summary>
        Task<DefaultQueryDto> GetDefaultQueries();

        /// <summary>
        /// 保存默认用户名
        /// </summary>
        Task SetDefaultQueries(DefaultQueryDto dto);
    }
}
