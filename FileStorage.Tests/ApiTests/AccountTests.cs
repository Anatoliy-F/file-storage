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
using Newtonsoft.Json;
using BuisnessLogicLayer.Models;
using System.Net;

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
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();
        }

        [Fact]
        public async Task Can_Get_Users()
        {
            //Arrange
            /*_client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();*/

            //Act
            var httpResponse = await _client.GetAsync(RequestUri);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<PaginationResultModel<UserModel>>(content);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.NotNull(content);
            Assert.Equal(4, actual.TotalCount);
        }

        [Fact]
        public async Task Can_Get_By_Id()
        {
            //Arrange
            var mapper = TestHelper.CreateMapperProfile();
            var expected = mapper.Map<UserModel>(TestHelper.Users[0]);

            //Act
            var httpResponse = await _client.GetAsync(RequestUri + TestHelper.Users[0].Id);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<UserModel>(content);
            var httpResponseNF = await _client.GetAsync(RequestUri + Guid.NewGuid());

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(HttpStatusCode.NotFound, httpResponseNF.StatusCode);
        }

        [Fact]
        public async Task Can_Delete_By_Id()
        {
            //Arrange
            var mapper = TestHelper.CreateMapperProfile();
            var user = mapper.Map<UserModel>(TestHelper.Users[0]);
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Content = content,
                RequestUri = new Uri(RequestUri + TestHelper.Users[0].Id.ToString(), UriKind.Relative)
            };
            HttpRequestMessage wrongRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Content = content,
                RequestUri = new Uri(RequestUri + Guid.NewGuid().ToString(), UriKind.Relative)
            };

            //Act
            var httpResponse = await _client.SendAsync(request);
            var wrongHttpResponse = await _client.SendAsync(wrongRequest);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, wrongHttpResponse.StatusCode);
        }


       
    }
}
