using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace AcmStatisticsBackend.ServiceClients
{
    /// <summary>
    /// <see cref="ICaptchaServiceClient.ValidateAsync"/> 的返回值
    /// </summary>
    public class CaptchaServiceValidateResult
    {
        /// <summary>
        /// 验证结果是否正确
        /// </summary>
        public bool Correct { get; set; }

        /// <summary>
        /// 如果不正确，这里存放错误信息
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}
