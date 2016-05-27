namespace System.Infrastructure.CrossCutting.Framework.Extensions
{
    using System.Collections.Generic;
    using System.Domain;
    using System.Infrastructure.CrossCutting.Adapter;
    public static class IEnumerableExtension
    {
        public static bool IsEmpty<T>(this IEnumerable<T> items)
        {
            using (var enumerator = items.GetEnumerator())
            {
                try {
                    return !enumerator.MoveNext();
                }
                catch { return true; }
            }
        }
    }
    public static class DataProjections
    {
        /// <summary>
        /// Project a type using a DTO
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="entity">The source entity to project</param>
        /// <returns>The projected type</returns>
        //public static TProjection ProjectedTo<TProjection>(this  DTO dto) where TProjection : class,new()
        //{
        //    var adapter = TypeAdapterFactory.CreateAdapter();
        //    return adapter.Adapt<TProjection>(dto);
        //}
        public static TProjection ProjectedTo<TProjection>(object source) where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(source);
        }

        /// <summary>
        /// projected a enumerable collection of DTO
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        //public static List<TProjection> ProjectedToCollection<TProjection>(this IEnumerable<DTO> dto)
        //    where TProjection : class,new()
        //{
        //    var adapter = TypeAdapterFactory.CreateAdapter();
        //    return adapter.Adapt<List<TProjection>>(dto);
        //}
        public static List<TProjection> ProjectedToCollection<TProjection>(object source)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(source);
        }

        /// <summary>
        /// Project a type using a Entity type
        /// </summary>
        /// <typeparam name="TProjection">The dto projection</typeparam>
        /// <param name="entity">The source entity to project</param>
        /// <returns>The projected type</returns>
        public static TProjection ProjectedTo<TProjection>(this Entity entity) where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<TProjection>(entity);
        }

        /// <summary>
        /// projected a enumerable collection of Entity types
        /// </summary>
        /// <typeparam name="TProjection">The dtop projection type</typeparam>
        /// <param name="items">the collection of entity items</param>
        /// <returns>Projected collection</returns>
        public static List<TProjection> ProjectedToCollection<TProjection>(this IEnumerable<Entity> entities)
            where TProjection : class, new()
        {
            var adapter = TypeAdapterFactory.CreateAdapter();
            return adapter.Adapt<List<TProjection>>(entities);
        }
    }
}