using Abp.Json;
using Shouldly;

namespace AcmStatisticsBackend.Tests
{
    public static class TestExtensions
    {
        /// <summary>
        /// 将左右两个对象转换成 json 再比较
        /// </summary>
        /// <typeparam name="T">两对象的类型</typeparam>
        public static void ShouldEqualInJson<T>(this T left, T right)
        {
            left.ShouldNotBeNull();
            left.ToJsonString(indented: true).ShouldBe(right.ToJsonString(indented: true));
        }
    }
}
