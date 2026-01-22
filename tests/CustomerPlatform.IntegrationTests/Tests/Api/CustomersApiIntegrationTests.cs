using CustomerPlatform.Api.Models;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerPlatform.IntegrationTests.Tests.Api
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Api")]
    public sealed class CustomersApiIntegrationTests
    {
        #region Nested types
        private sealed class ApiApplicationFactory : WebApplicationFactory<Program>
        {
            #region Protected Methods/Operators
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    var testConfiguration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .Build();

                    config.AddConfiguration(testConfiguration);
                });
            }
            #endregion
        }
        #endregion

        #region Variables
        private ApiApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private JsonSerializerOptions _jsonOptions = null!;
        #endregion

        #region SetUp Methods
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            SetEnvironmentVariable("ConnectionStrings__PostgreSql", configuration.GetConnectionString("PostgreSql"));
            SetEnvironmentVariable("ConnectionStrings__RabbitMq", configuration.GetConnectionString("RabbitMq"));
            SetEnvironmentVariable("ConnectionStrings__ElasticSearch", configuration.GetConnectionString("ElasticSearch"));
        }

        [SetUp]
        public void SetUp()
        {
            _factory = new ApiApplicationFactory();
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
        #endregion

        #region Private Methods/Operators
        private static void SetEnvironmentVariable(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return;

            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
        }
        #endregion

        #region Test Methods - CreateIndividual Valid Cases
        [Test]
        public async Task CriarIndividual_DeveRetornarCriado()
        {
            // Arrange
            var command = new CreateIndividualCustomerCommand
            {
                FullName = "Cliente Teste",
                Cpf = "17871018434",
                Email = "cliente@teste.com",
                Phone = "11999999999",
                BirthDate = new DateOnly(1990, 1, 10),
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "100",
                    Complement = "Apto 12",
                    PostalCode = "12345000",
                    City = "Sao Paulo",
                    State = "SP"
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/customers/pf", command, _jsonOptions).ConfigureAwait(false);
            var payload = await response.Content.ReadFromJsonAsync<CustomerDto>(_jsonOptions).ConfigureAwait(false);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload!.Id, Is.Not.EqualTo(Guid.Empty));
        }
        #endregion

        #region Test Methods - UpdateCustomer Valid Cases
        [Test]
        public async Task AtualizarCliente_DeveRetornarOk()
        {
            // Arrange
            var createCommand = new CreateIndividualCustomerCommand
            {
                FullName = "Cliente Inicial",
                Cpf = "08608744511",
                Email = "cliente@teste.com",
                Phone = "11988887777",
                BirthDate = new DateOnly(1992, 2, 2),
                Address = new AddressDto
                {
                    Street = "Rua Inicial",
                    Number = "200",
                    Complement = "Casa",
                    PostalCode = "12345001",
                    City = "Sao Paulo",
                    State = "SP"
                }
            };

            var createdResponse = await _client.PostAsJsonAsync("/customers/pf", createCommand, _jsonOptions).ConfigureAwait(false);
            var createdCustomer = await createdResponse.Content.ReadFromJsonAsync<CustomerDto>(_jsonOptions).ConfigureAwait(false);

            var updateRequest = new UpdateCustomerRequest
            {
                CustomerType = TipoCliente.PF,
                FullName = "Cliente Atualizado",
                Email = "cliente@teste.com",
                Phone = "11977776666",
                BirthDate = new DateOnly(1992, 2, 2),
                Address = new AddressDto
                {
                    Street = "Rua Atualizada",
                    Number = "300",
                    Complement = "Apto 101",
                    PostalCode = "12345002",
                    City = "Sao Paulo",
                    State = "SP"
                }
            };

            // Act
            var response = await _client.PutAsJsonAsync(
                    $"/customers/{createdCustomer!.Id}",
                    updateRequest,
                    _jsonOptions)
                .ConfigureAwait(false);
            var payload = await response.Content.ReadFromJsonAsync<CustomerDto>(_jsonOptions).ConfigureAwait(false);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(payload, Is.Not.Null);
            Assert.That(payload!.Name, Is.EqualTo(updateRequest.FullName));
        }
        #endregion

        #region Test Methods - HealthCheck Valid Cases
        [Test]
        public async Task Saude_DeveRetornarOk()
        {
            // Arrange
            // Act
            var response = await _client.GetAsync("/health").ConfigureAwait(false);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
        #endregion
    }
}
