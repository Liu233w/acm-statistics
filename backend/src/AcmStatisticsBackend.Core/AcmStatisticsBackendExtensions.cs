using System;

namespace AcmStatisticsBackend
{
    public static class AcmStatisticsBackendExtensions
    {
#pragma warning disable SA1618
        /// <summary>
        /// 用法：
        /// <code>
        /// Get().A().Object().WithIn(it => {
        ///     it.methodA();
        ///     it.methodB();
        /// })
        /// </code>
        /// </summary>
        public static TR WithIn<TT, TR>(this TT obj, Func<TT, TR> func)
            where TT : class
        {
            return func(obj);
        }
#pragma warning restore SA1618

        public static TR WithIn<TT, TR>(this ref TT obj, Func<TT, TR> func)
            where TT : struct
        {
            return func(obj);
        }

        public static void WithIn<T>(this T obj, Action<T> action)
            where T : class
        {
            action(obj);
        }

        public static void WithIn<T>(this ref T obj, Action<T> action)
            where T : struct
        {
            action(obj);
        }

        // from https://stackoverflow.com/a/47815787
        public static void Deconstruct<T>(this T[] items, out T t0)
        {
            t0 = items.Length > 0 ? items[0] : default;
        }

        public static void Deconstruct<T>(this T[] items, out T t0, out T t1)
        {
            t0 = items.Length > 0 ? items[0] : default;
            t1 = items.Length > 1 ? items[1] : default;
        }
    }
}
