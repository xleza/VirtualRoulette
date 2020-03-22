namespace VirtualRoulette.Security
{
    public sealed class AuthConfig
    {
        public short ExpirationInMinutes { get; set; }
        public short IdleTimeoutInMinutes { get; set; }
    }
}
