using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using VirtualRoulette.Commands;
using VirtualRoulette.Domain;
using VirtualRoulette.Exceptions;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;
using Xunit;

namespace VirtualRoulette.Tests
{
    public sealed class SecurityServiceTest
    {
        private readonly Mock<IHttpContext> _httpContext;
        private readonly Mock<IDbHelper> _dbHelper;
        private readonly SecurityService _service;

        public SecurityServiceTest()
        {
            _httpContext = new Mock<IHttpContext>();
            _dbHelper = new Mock<IDbHelper>();
            _service = new SecurityService(_httpContext.Object, _dbHelper.Object);
        }

        [Fact]
        public async Task Authenticate_With_Not_Existing_User()
        {
            _dbHelper.Setup(x => x.GetUserAsync("user", It.IsAny<User.ControlFlags>()))
                .ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.AuthenticateAsync(new AuthenticateUser
            {
                Username = "user"
            }));

            Assert.Equal(Constants.InvalidUsernameOrPasswordExceptionText, exception.Message);
        }

        [Fact]
        public async Task Authenticate_With_Invalid_Password()
        {
            _dbHelper.Setup(x => x.GetUserAsync("user", It.IsAny<User.ControlFlags>()))
                .ReturnsAsync(new User
                {
                    Password = "123"
                });

            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.AuthenticateAsync(new AuthenticateUser
            {
                Username = "user",
                Password = "123"
            }));

            Assert.Equal(Constants.InvalidUsernameOrPasswordExceptionText, exception.Message);
        }

        [Fact]
        public async Task Authenticate_With_Valid_Password()
        {
            _dbHelper.Setup(x => x.GetUserAsync("user", It.IsAny<User.ControlFlags>()))
                .ReturnsAsync(new User
                {
                    Id = 17,
                    Username = "user",
                    Password = "vJo0U9Bz7IiCCt9G6am7s83sDdnuc2bp73qKImjJlPo="
                });

            _httpContext.Setup(x => x.SignInAsync(Constants.CookiesKey, It.IsAny<ClaimsPrincipal>()))
                .Returns(Task.CompletedTask);

            var authenticate = _service.AuthenticateAsync(new AuthenticateUser
            {
                Username = "user",
                Password = "pwd123"
            });

            await authenticate;

            Assert.True(authenticate.IsCompleted);
        }

        [Fact]
        public void Current_User_Properties_When_Authenticated()
        {
            _httpContext.Setup(x => x.UserIsAuthenticated).Returns(true);
            _httpContext.Setup(x => x.UserClaims).Returns(new List<Claim>
            {
                new Claim(SecurityService.ClaimKeys.Id,"17"),
                new Claim(SecurityService.ClaimKeys.Username,"user")
            });

            var currentUser = _service.CurrentUser;

            Assert.Equal(17, currentUser.Id);
            Assert.Equal("user", currentUser.Username);
        }

        [Fact]
        public void Current_User_Properties_When_Not_Authenticated()
        {
            _httpContext.Setup(x => x.UserIsAuthenticated).Returns(false);
            _httpContext.Setup(x => x.UserClaims).Returns(new List<Claim>
            {
                new Claim(SecurityService.ClaimKeys.Id,"17"),
                new Claim(SecurityService.ClaimKeys.Username,"user")
            });

            Assert.Null(_service.CurrentUser);
        }
    }
}
