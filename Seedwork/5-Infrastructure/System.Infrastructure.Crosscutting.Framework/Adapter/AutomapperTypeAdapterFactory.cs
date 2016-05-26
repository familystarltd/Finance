namespace System.Infrastructure.CrossCutting.Framework.Adapter
{
    using System;
    using System.Linq;
    using AutoMapper;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Infrastructure.CrossCutting.Adapter;
    using Text;
    using IO;
    using Microsoft.Extensions.PlatformAbstractions;
    using Microsoft.Extensions.DependencyModel;
    using System.Runtime.Loader;
    public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
    {
        private IMapper _autoMapper;
        /// <summary>
        /// Create a new Automapper type adapter factory
        /// </summary>
        public AutomapperTypeAdapterFactory(IMapper AutoMapper)
        {
            _autoMapper = AutoMapper;
        }
        #region ITypeAdapterFactory Members
        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter(_autoMapper);
        }
        #endregion
    }
}
