using BuisnessLogicLayer.Models;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using WebAPI.Models;

namespace FileStorage.Tests.ApiTests
{
    [Collection("Sequential")]
    public class AccountTests
    {
        private readonly HttpClient _client;
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
        public async Task Can_Register_User()
        {
            //Arrange
            var user = new RegistrationRequestModel
            {
                UserName = "TestUser",
                Email = "UserTest@mail.com",
                Password = "Tesr$4uSer",
                ConfirmPassword = "Tesr$4uSer"
            };

            var badUser = new RegistrationRequestModel
            {
                UserName = "TestUser",
                Email = "UserTestmail.com",
                Password = "Tesr$4uSer",
                ConfirmPassword = "Te$4uSer"
            };

            //Act
            var httpResponse = await _client.PostAsJsonAsync<RegistrationRequestModel>
                (RequestUri + "Registration", user);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<RegistrationResponseModel>(content);

            var badHttpResponse = await _client.PostAsJsonAsync<RegistrationRequestModel>
                (RequestUri + "Registration", badUser);
            var badContent = await badHttpResponse.Content.ReadAsStringAsync();

            var wronghttpResponse = await _client.PostAsJsonAsync<RegistrationRequestModel>
                (RequestUri + "Registration", user);
            var wrongContent = await wronghttpResponse.Content.ReadAsStringAsync();
            var wrongActual = JsonConvert.DeserializeObject<RegistrationResponseModel>(wrongContent);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.NotNull(content);
            Assert.Equal(true, actual?.Success);
            Assert.Equal(HttpStatusCode.BadRequest, badHttpResponse.StatusCode);
            Assert.NotNull(badContent);
            Assert.Equal(HttpStatusCode.BadRequest, wronghttpResponse.StatusCode);
            Assert.NotNull(wrongContent);
            Assert.Equal(false, wrongActual?.Success);

            //Act2 Check Login
            var user2 = new LoginRequestModel
            {
                Email = "UserTest@mail.com",
                Password = "Tesr$4uSer",
            };
            var httpResponse2 = await _client.PostAsJsonAsync<LoginRequestModel>
               (RequestUri + "Login", user2);
            var content2 = await httpResponse.Content.ReadAsStringAsync();
            var actual2 = JsonConvert.DeserializeObject<LoginResponseModel>(content);
            Assert.Equal(HttpStatusCode.OK, httpResponse2.StatusCode);
            Assert.NotNull(content2);
            Assert.Equal(true, actual2?.Success);
        }

        /*[Fact]
        public async Task Can_Login_User()
        {
            //Arrange
            var user = new LoginRequestModel
            {
                Email = "UserTest@mail.com",
                Password = "Tesr$4uSer",
            };

            var badUser = new LoginRequestModel
            {
                Email = "UserTest33@mail.com",
                Password = "Tesr$4uSer",
            };

            //Act
            var httpResponse = await _client.PostAsJsonAsync<LoginRequestModel>
               (RequestUri + "Login", user);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var actual = JsonConvert.DeserializeObject<LoginResponseModel>(content);

            var wronghttpResponse = await _client.PostAsJsonAsync<LoginRequestModel>
                (RequestUri + "Login", badUser);
            var wrongContent = await wronghttpResponse.Content.ReadAsStringAsync();
            var wrongActual = JsonConvert.DeserializeObject<LoginResponseModel>(wrongContent);

            //Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.NotNull(content);
            Assert.Equal(true, actual?.Success);
            Assert.Equal(HttpStatusCode.BadRequest, wronghttpResponse.StatusCode);
            Assert.NotNull(wrongContent);
            Assert.Equal(false, wrongActual?.Success);
        }*/


        [Fact]
        public async Task Can_Get_Users()
        {
            //Arrange

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
