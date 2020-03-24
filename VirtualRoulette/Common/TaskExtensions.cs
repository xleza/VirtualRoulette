using System;
using System.Threading.Tasks;

namespace VirtualRoulette.Common
{
    public static class TaskExtensions
    {
        public static async Task<TResult> Map<TSource, TResult>(this Task<TSource> self, Func<TSource, TResult> mapper)
            => mapper(await self);
    }
}
