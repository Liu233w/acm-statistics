namespace OHunt.Web.Schedule
{
    /// <summary>
    /// A message sent to database inserter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DatabaseInserterMessage<T>
        where T : class
    {
        public T? Entity { get; set; }

        /// <summary>
        /// If it is true, all data in buffer is inserted to database
        /// even if the buffer is not full.
        /// </summary>
        public bool ForceInsert { get; set; }
    }
}
