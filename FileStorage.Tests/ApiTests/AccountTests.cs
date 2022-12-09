using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Policy;
using Microsoft.AspNetCore.Authorization.Policy;

namespace FileStorage.Tests.ApiTests
{
    [Collection("Sequential")]
    public class AccountTests
    {
        private /*readonly*/ HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private const string RequestUri = "api/Account/";

        public AccountTests()
        {
            _factory = new CustomWebApplicationFactory();
        }

        [Fact]
        public async Task Can_Get_Users()
        {
            //Arrange
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();

            //Act
            var httpResponse = await _client.GetAsync(RequestUri);

            //Assert
            Assert.NotNull(httpResponse);
        }

       
    }
}
