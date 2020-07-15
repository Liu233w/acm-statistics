using System.Collections.Generic;

namespace OHunt.Web.Options
{
    public class DatabaseInserterOptions
    {
        public int DefaultBufferSize { get; set; }

        public Dictionary<string, int> BufferSize { get; set; }
    }
}
