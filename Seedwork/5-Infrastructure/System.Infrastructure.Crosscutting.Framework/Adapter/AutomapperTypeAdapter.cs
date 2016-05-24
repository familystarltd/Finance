namespace System.Infrastructure.CrossCutting.Framework.Adapter
{
    using AutoMapper;
    using System.Infrastructure.CrossCutting.Adapter;

    /// <summary>
    /// Base contract for adapter factory
    /// </summary>

    public class AutomapperTypeAdapter : ITypeAdapter
    {
        IMapper _autoMapper;
        public AutomapperTypeAdapter(IMapper AutoMapper)
        {
            _autoMapper = AutoMapper;
        }
        /// <summary>
        /// Automapper type adapter implementation
        /// </summary>

        public TTarget Adapt<TSource, TTarget>(TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            return _autoMapper.Map<TSource,TTarget>(source);
        }

        public TTarget Adapt<TTarget>(object source) where TTarget : class, new()
        {
            return _autoMapper.Map<TTarget>(source);
        }
    }
}