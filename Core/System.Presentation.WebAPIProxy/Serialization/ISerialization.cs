using System.IO;

namespace System.Presentation.WebAPIProxy.Serialization
{
    public interface ISerialization
    {
        string Serialize<T>(object o);
        string Serialize<T>(object obj, bool ApplySerializeSettings);
        TDestination Serialize<TSource, TDestination>(TSource source);
        T DeSerialize<T>(Stream stream);
        T DeSerialize<T>(string value);
    }
}
