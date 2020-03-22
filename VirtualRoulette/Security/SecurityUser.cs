namespace VirtualRoulette.Security
{
    public sealed class SecurityUser
    {
        public int Id { get; private set; }
        public string Username { get; private set; }

        public SecurityUser(int id, string username)
        {
            Id = id;
            Username = username;
        }
    }
}
