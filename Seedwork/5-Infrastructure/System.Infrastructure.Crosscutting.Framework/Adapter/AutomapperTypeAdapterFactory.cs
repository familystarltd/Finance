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

    public interface IDomainProfileMapper
    {
        T Map<T>(object source);
        //object Configuration { get; }
    }
    public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
    {
        private IMapper _autoMapper;
        /// <summary>
        /// Create a new Automapper type adapter factory
        /// </summary>
        public AutomapperTypeAdapterFactory()
        {
            try {
                //scan all assemblies finding Automapper Profile that implements IDomainProfileMapper
                //PlatformServices
                ICollection<Profile> profiles = new List<Profile>();
                foreach (AssemblyName assemblyName in typeof(IDomainProfileMapper).GetTypeInfo().Assembly.GetReferencedAssemblies())
                {
                   Assembly assembly = Assembly.Load(assemblyName);
                   foreach(Type type in assembly.GetTypes())
                    {
                        if(type.GetTypeInfo().BaseType == typeof(Profile))
                        {
                            profiles.Add((Profile)Activator.CreateInstance(type));
                        }
                    }
                }
                var config = new MapperConfiguration(cfg =>
                {
                    foreach (var profile in profiles)
                    {
                        cfg.AddProfile(profile);
                    }
                });
                _autoMapper = config.CreateMapper();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        //if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        //{
                        //    sb.AppendLine("Fusion Log:");
                        //    sb.AppendLine(exFileNotFound.FusionLog);
                        //}
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                throw new Exception(errorMessage);
                //Display or log the error based on your application.
            }
        }
        #region ITypeAdapterFactory Members
        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter(_autoMapper);
        }
        #endregion
    }
}
