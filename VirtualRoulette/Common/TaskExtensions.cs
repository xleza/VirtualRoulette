using System;
using System.Threading.Tasks;
using VirtualRoulette.Exceptions;

namespace VirtualRoulette.Common
{
    public static class TaskExtensions
    {
        public static async Task<T> ThrowIfNull<T>(this Task<T> self, string key)
        {
            var result = await self;
            if (result == null)
                throw new NotFoundException(typeof(T), key);

            return result;
        }

        public static async Task<TResult> Map<TSource, TResult>(this Task<TSource> self, Func<TSource, TResult> mapper)
            => mapper(await self);
    }
}
