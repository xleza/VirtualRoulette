using System;

namespace VirtualRoulette.Common
{
    public class Guard
    {
        public static void NotNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        public static void NotEmpty(string arg, string argName)
        {
            if (arg.IsNullOrEmpty())
                throw new ArgumentException(argName, $"String parameter {argName} cannot be null or empty");
        }
    }
}
