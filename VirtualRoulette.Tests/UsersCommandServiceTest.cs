using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtualRoulette.Commands;
using VirtualRoulette.Domain;
using VirtualRoulette.Exceptions;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;
using VirtualRoulette.Services;
using Xunit;

namespace VirtualRoulette.Tests
{
    public sealed class UsersCommandServiceTest
    {
        private readonly Mock<IDbHelper> _dbHelperMock;
        private readonly Mock<ISecurityService> _securityServiceMock;
        private readonly UsersCommandService _service;

        public UsersCommandServiceTest()
        {
            _dbHelperMock = new Mock<IDbHelper>();
            _securityServiceMock = new Mock<ISecurityService>();
            _service = new UsersCommandService(_dbHelperMock.Object, _securityServiceMock.Object);
        }

        [Fact]
        public async Task MakeBet_When_Bet_Invalid()
        {
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.MakeBet(new MakeBet
            {
                Bet = "abc",
                RowVersion = 1
            }, ""));

            Assert.Equal(Constants.InvalidBetExceptionTest, exception.Message);
        }

        [Fact]
        public Task MakeBet_With_Invalid_RowVersion()
        {
            _securityServiceMock.Setup(x => x.CurrentUser)
                .Returns(new SecurityUser(1, "abc"));
            _dbHelperMock.Setup(x => x.GetUserAsync(1, It.IsAny<User.ControlFlags>()))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    RowVersion = 7
                });

            return Assert.ThrowsAsync<ConcurrencyException>(() => _service.MakeBet(new MakeBet
            {
                Bet = "[{\"T\": \"v\", \"I\": 20, \"C\": 1, \"K\": 1}]",
                RowVersion = 1
            }, ""));
        }

        [Fact]
        public void MakeBet_Check_Parameters()
        {
            var user = new User
            {
                Balance = 10
            };

            user.MakeBat(
                new MakeBet
                {
                    Bet = "ABC"
                },
                (bet, number) =>
                {
                    Assert.Equal("ABC", bet);
                    return 10;
                },
                12,
                5,
                "123");

            var madeBet = user.Bets.FirstOrDefault();
            Assert.NotNull(madeBet);
            Assert.Equal("ABC", madeBet.Bet);
            Assert.Equal("123", madeBet.IpAddress);
            Assert.Equal(5, madeBet.Amount);
            Assert.Equal(12, madeBet.WinningNumber);
            Assert.Equal(10, madeBet.WonAmount);
            Assert.True(madeBet.Won);
        }

        [Fact]
        public void MakeBet_With_Not_Enough_Balance()
        {
            var user = new User
            {
                Balance = 10
            };

            var exception = Assert.Throws<BadRequestException>(() => user.MakeBat(
                   new MakeBet
                   {
                       Bet = "ABC"
                   },
                   (bet, number) => 0,
                   12,
                   25,
                   "123"));

            Assert.Equal(Constants.NotEnoughBalanceExceptionText, exception.Message);
        }

        [Fact]
        public void MakeBet_CheckParams_During_Win()
        {
            var user = new User
            {
                Balance = 10
            };

            user.MakeBat(
               new MakeBet
               {
                   Bet = "ABC"
               },
               (bet, number) => 25,
               12,
               5,
               "123");

            var madeBet = user.Bets.FirstOrDefault();
            Assert.NotNull(madeBet);
            Assert.Equal("ABC", madeBet.Bet);
            Assert.Equal("123", madeBet.IpAddress);
            Assert.Equal(5, madeBet.Amount);
            Assert.Equal(12, madeBet.WinningNumber);
            Assert.Equal(25, madeBet.WonAmount);
            Assert.True(madeBet.Won);

            Assert.Equal(30, user.Balance);
            Assert.True(user.BetMade);
        }

        [Fact]
        public void MakeBet_CheckParams_During_Loss()
        {
            var user = new User
            {
                Balance = 10
            };

            user.MakeBat(
                new MakeBet
                {
                    Bet = "ABC"
                },
                (bet, number) => 0,
                12,
                5,
                "123");

            var madeBet = user.Bets.FirstOrDefault();
            Assert.NotNull(madeBet);
            Assert.Equal("ABC", madeBet.Bet);
            Assert.Equal("123", madeBet.IpAddress);
            Assert.Equal(5, madeBet.Amount);
            Assert.Equal(12, madeBet.WinningNumber);
            Assert.Null(madeBet.WonAmount);
            Assert.False(madeBet.Won);

            Assert.Equal(5, user.Balance);
            Assert.True(user.BetMade);
        }

        [Fact]
        public async Task MakeBet_Response_With_Valid_Parameters()
        {
            _securityServiceMock.Setup(x => x.CurrentUser)
                .Returns(new SecurityUser(1, "user"));

            _dbHelperMock.Setup(x => x.GetUserAsync(1, It.IsAny<User.ControlFlags>()))
                .ReturnsAsync(new User
                {
                    Id = 1,
                    Balance = 100,
                    RowVersion = 1
                });
            _dbHelperMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>(), 1)).Returns(Task.CompletedTask);

            var response = await _service.MakeBet(new MakeBet
            {
                Bet = "[{\"T\": \"v\", \"I\": 20, \"C\": 1, \"K\": 1}]",
                RowVersion = 1
            }, "123");

            Assert.NotNull(response);
        }
    }
}
