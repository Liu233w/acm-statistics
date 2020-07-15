using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;

namespace OHunt.Tests
{
    public static class TestExtensions
    {
        public static NonGenericAsyncFunctionAssertions ShouldResult(this Task task)
        {
            return FluentActions.Awaiting(async () => await task).Should();
        }
    }
}
