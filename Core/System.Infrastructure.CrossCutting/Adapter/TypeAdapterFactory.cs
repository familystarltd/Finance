namespace System.Infrastructure.CrossCutting.Adapter
{
    public static class TypeAdapterFactory
    {
        static ITypeAdapterFactory _currentTypeAdapterFactory = null;

        /// <summary>
        /// Set the current type adapter factory
        /// </summary>
        /// <param name="adapterFactory">The adapter factory to set</param>
        public static void SetAdapter(ITypeAdapterFactory adapterFactory)
        {
            _currentTypeAdapterFactory = adapterFactory;
        }
        /// <summary>
        /// Create a new type adapter from currect factory
        /// </summary>
        /// <returns>Created type adapter</returns>
        public static ITypeAdapter CreateAdapter()
        {
            return _currentTypeAdapterFactory.Create();
        }
    }
}