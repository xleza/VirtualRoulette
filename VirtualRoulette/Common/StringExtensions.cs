namespace VirtualRoulette.Common
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string self) => string.IsNullOrWhiteSpace(self);
    }
}
