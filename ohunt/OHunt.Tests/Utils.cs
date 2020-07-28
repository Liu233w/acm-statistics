using System;
using System.Threading.Tasks;

namespace OHunt.Tests
{
    public class Utils
    {
        public static Task WaitSecond(int second = 1)
        {
            return Task.Delay(TimeSpan.FromSeconds(second));
        }
    }
}
