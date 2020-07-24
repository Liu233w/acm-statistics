namespace OHunt.Web.Dataflow
{
    /// <summary>
    /// A message sent to database inserter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct DatabaseInserterMessage<T>
        where T : class
    {
        private DatabaseInserterMessage(T? entity, bool forceInsert)
        {
            Entity = entity;
            ForceInsert = forceInsert;
        }

        /// <summary>
        /// Create an instance with the entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="forceInsert">Should it trigger force insert (default false)</param>
        /// <returns></returns>
        public static DatabaseInserterMessage<T> OfEntity(T entity, bool forceInsert = false)
        {
            return new DatabaseInserterMessage<T>(entity, forceInsert);
        }

        /// <summary>
        /// Create an instance with force insert
        /// </summary>
        public static DatabaseInserterMessage<T> ForceInsertMessage
            => new DatabaseInserterMessage<T>(null, true);

        public T? Entity { get; }

        /// <summary>
        /// If it is true, all data in buffer is inserted to database
        /// even if the buffer is not full.
        /// </summary>
        public bool ForceInsert { get; }
    }
}
