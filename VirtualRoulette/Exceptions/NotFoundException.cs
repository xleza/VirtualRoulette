using System;

namespace VirtualRoulette.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(Type type, string key)
            : base($"Type {type.FullName} not found with key {key}")
        {

        }
    }
}
