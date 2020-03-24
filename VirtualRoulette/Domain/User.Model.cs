using System;
using System.Collections.Generic;

namespace VirtualRoulette.Domain
{
    public sealed partial class User
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public int RowVersion { get; set; }
        public List<UserBet> Bets { get; set; }
        public DateTime? DateModified { get; set; }

        public bool BetMade { get; private set; }

        [Flags]
        public enum ControlFlags // Used to control loading
        {
            Basic = 0,
            Bets = 1
        }
    }
}
