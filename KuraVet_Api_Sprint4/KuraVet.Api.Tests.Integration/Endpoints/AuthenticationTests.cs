using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Auth;
using KuraVet.Domain.Entities;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class AuthenticationTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthenticationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornar401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var resposta = await client.GetAsync("/api/Tutor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Get_ComTokenInvalido_DeveRetornar401()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token.invalido.123");

        // Act
        var resposta = await client.GetAsync("/api/Tutor");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_AdminDoSeed_DeveEmitirTokenQueAcessaRotaProtegida()
    {
        // Arrange
        var client = _factory.CreateClient();
        var login = new LoginRequest
        {
            Email = CustomWebApplicationFactory.AdminEmail,
            Senha = CustomWebApplicationFactory.AdminSenha
        };

        // Act
        var respostaLogin = await client.PostAsJsonAsync("/api/auth/login", login);
        var token = await respostaLogin.Content.ReadFromJsonAsync<TokenResponse>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        var respostaMe = await client.GetAsync("/api/auth/me");
        var usuario = await respostaMe.Content.ReadFromJsonAsync<UsuarioResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, respostaLogin.StatusCode);
        Assert.Equal("Bearer", token.TokenType);
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
        Assert.Equal(HttpStatusCode.OK, respostaMe.StatusCode);
        Assert.Equal(Perfis.Admin, usuario!.Perfil);
    }

    [Fact]
    public async Task Login_SenhaIncorreta_DeveRetornar401ComProblemDetails()
    {
        // Arrange
        var client = _factory.CreateClient();
        var login = new LoginRequest { Email = CustomWebApplicationFactory.AdminEmail, Senha = "senha-errada" };

        // Act
        var resposta = await client.PostAsJsonAsync("/api/auth/login", login);
        var corpo = await resposta.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        Assert.Contains("traceId", corpo);
    }

    [Fact]
    public async Task Register_NovoTutor_DeveRetornar201EPermitirLogin()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = $"tutor{Guid.NewGuid():N}@kuravet.com";
        var registro = new RegistroRequest { Nome = "Novo Tutor", Email = email, Senha = "SenhaForte@123" };

        // Act
        var respostaRegistro = await client.PostAsJsonAsync("/api/auth/register", registro);
        var respostaLogin = await client.PostAsJsonAsync(
            "/api/auth/login", new LoginRequest { Email = email, Senha = "SenhaForte@123" });
        var token = await respostaLogin.Content.ReadFromJsonAsync<TokenResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, respostaRegistro.StatusCode);
        Assert.Equal(HttpStatusCode.OK, respostaLogin.StatusCode);
        Assert.Equal(Perfis.Tutor, token!.Usuario.Perfil);
    }

    [Fact]
    public async Task Register_EmailDuplicado_DeveRetornar409()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registro = new RegistroRequest
        {
            Nome = "Duplicado",
            Email = CustomWebApplicationFactory.AdminEmail,
            Senha = "SenhaForte@123"
        };

        // Act
        var resposta = await client.PostAsJsonAsync("/api/auth/register", registro);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task Delete_PerfilTutor_DeveRetornar403()
    {
        // Arrange
        var admin = _factory.CriarClienteAutenticado(Perfis.Admin);
        var tutor = await admin.CriarTutorAsync("Tutor Protegido");
        var clienteTutor = _factory.CriarClienteAutenticado(Perfis.Tutor);

        // Act
        var resposta = await clienteTutor.DeleteAsync($"/api/Tutor/{tutor.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task CriarUsuario_PerfilTutor_DeveRetornar403()
    {
        // Arrange
        var clienteTutor = _factory.CriarClienteAutenticado(Perfis.Tutor);
        var request = new CriarUsuarioRequest
        {
            Nome = "Tentativa",
            Email = "tentativa@kuravet.com",
            Senha = "SenhaForte@123",
            Perfil = Perfis.Admin
        };

        // Act
        var resposta = await clienteTutor.PostAsJsonAsync("/api/auth/usuarios", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
