namespace System.Infrastructure.CrossCutting.Validator
{
    /// <summary>
    /// Entity Validator Factory
    /// </summary>
    public static class EntityValidatorFactory
    {
        static IEntityValidatorFactory _factory = null;

        /// <summary>
        /// Set the  log factory to use
        /// </summary>
        /// <param name="factory">Log factory to use</param>
        public static void SetCurrent(IEntityValidatorFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Createt a new <paramref name="CareSystem.Infrastructure.Crosscutting.Logging.ILog"/>
        /// </summary>
        /// <returns>Created ILog</returns>
        public static IEntityValidator CreateValidator()
        {
            return (_factory != null) ? _factory.Create() : null;
        }
    }
}
