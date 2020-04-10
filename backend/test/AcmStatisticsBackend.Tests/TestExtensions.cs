using Abp.Json;
using Shouldly;

namespace AcmStatisticsBackend.Tests
{
    public static class TestExtensions
    {
        /// <summary>
        /// 将左右两个对象转换成 json 再比较
        /// </summary>
        public static void ShouldEqualInJson(this object left, object right)
        {
            left.ShouldNotBeNull();
            left.ToJsonString(indented: true).ShouldBe(right.ToJsonString(indented: true));
        }
    }
}
