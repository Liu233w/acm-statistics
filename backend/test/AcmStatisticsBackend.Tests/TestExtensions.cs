using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace AcmStatisticsBackend.Tests
{
    public static class TestExtensions
    {
        public static Task<ExceptionAssertions<T>> ShouldThrow<T>(
            this Task task,
            string because = "")
            where T : Exception
        {
            Func<Task> call = async () => await task;
            return call.Should().ThrowAsync<T>(because);
        }

        public static Task<ExceptionAssertions<TException>>
            ShouldThrow<TException, TResult>(
                this Task<TResult> task,
                string because = "")
            where TException : Exception
        {
            Func<Task<TResult>> call = async () => await task;
            return call.Should().ThrowAsync<TException>(because);
        }
    }
}
