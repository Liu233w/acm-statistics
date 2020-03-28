using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AcmStatisticsBackend.ServiceClients
{
    public interface ICaptchaServiceClient
    {
        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="id">验证码的ID</param>
        /// <param name="text">用户输入的验证码</param>
        /// <returns>验证结果。如果不正确，返回(false, 错误信息)</returns>
        Task<CaptchaServiceValidateResult> ValidateAsync(string id, string text);
    }
}
