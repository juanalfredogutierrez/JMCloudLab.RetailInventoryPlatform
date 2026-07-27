using AuthService.Application.Commands.Login;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using BuildingBlocks.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthService.Tests.Application.Login;

public class LoginHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Token_When_Credentials_Are_Valid()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AuthDbContext(options);

        var rol = new Rol
        {
            Nombre = "Administrador"
        };

        context.Roles.Add(rol);

        var passwordHasher = new PasswordHasher<Usuario>();

        var usuario = new Usuario
        {
            NombreUsuario = "admin",
            CorreoElectronico = "admin@retail.com",
            Activo = true,
            RolId = rol.Id,
            Rol = rol
        };

        usuario.ClaveHash = passwordHasher.HashPassword(
            usuario,
            "Admin123*");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = "EstaEsUnaClaveSuperSeguraDeMasDe32Caracteres",
            Issuer = "RetailInventory",
            Audience = "RetailInventoryUsers",
            ExpirationMinutes = 60
        });

        var jwtGenerator = new JwtTokenGenerator(jwtOptions);

        var logger = Mock.Of<ILogger<LoginHandler>>();

        var handler = new LoginHandler(
            context,
            jwtGenerator,
            logger,
            passwordHasher);

        var command = new LoginCommand(
            "admin",
            "Admin123*");

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNullOrWhiteSpace();

        result.Errors.Should().BeEmpty(); ;
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_User_Does_Not_Exist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AuthDbContext(options);

        var passwordHasher = new PasswordHasher<Usuario>();

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = "EstaEsUnaClaveSuperSeguraDeMasDe32Caracteres",
            Issuer = "RetailInventory",
            Audience = "RetailInventoryUsers",
            ExpirationMinutes = 60
        });

        var jwtGenerator = new JwtTokenGenerator(jwtOptions);

        var logger = Mock.Of<ILogger<LoginHandler>>();

        var handler = new LoginHandler(
            context,
            jwtGenerator,
            logger,
            passwordHasher);

        var command = new LoginCommand(
            "usuario-inexistente",
            "Admin123*");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        result.Errors.Should().ContainSingle();

        result.FirstError?.Message.Should()
            .Be("Credenciales inválidas");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Password_Is_Invalid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AuthDbContext(options);

        var rol = new Rol
        {
            Nombre = "Administrador"
        };

        context.Roles.Add(rol);

        await context.SaveChangesAsync();

        var passwordHasher = new PasswordHasher<Usuario>();

        var usuario = new Usuario
        {
            NombreUsuario = "admin",
            CorreoElectronico = "admin@retail.com",
            Activo = true,
            Rol = rol
        };

        usuario.ClaveHash = passwordHasher.HashPassword(
            usuario,
            "Admin123*");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = "EstaEsUnaClaveSuperSeguraDeMasDe32Caracteres",
            Issuer = "RetailInventory",
            Audience = "RetailInventoryUsers",
            ExpirationMinutes = 60
        });

        var jwtGenerator = new JwtTokenGenerator(jwtOptions);

        var logger = Mock.Of<ILogger<LoginHandler>>();

        var handler = new LoginHandler(
            context,
            jwtGenerator,
            logger,
            passwordHasher);

        var command = new LoginCommand(
            "admin",
            "PasswordIncorrecto");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        result.Errors.Should().ContainSingle();

        result.FirstError?.Message.Should()
            .Be("Credenciales inválidas");
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_User_Is_Inactive()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AuthDbContext(options);

        var rol = new Rol
        {
            Nombre = "Administrador"
        };

        context.Roles.Add(rol);

        await context.SaveChangesAsync();

        var passwordHasher = new PasswordHasher<Usuario>();

        var usuario = new Usuario
        {
            NombreUsuario = "inactivo",
            CorreoElectronico = "inactivo@retail.com",
            Activo = false,
            Rol = rol
        };

        usuario.ClaveHash = passwordHasher.HashPassword(
            usuario,
            "Inactivo123*");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = "EstaEsUnaClaveSuperSeguraDeMasDe32Caracteres",
            Issuer = "RetailInventory",
            Audience = "RetailInventoryUsers",
            ExpirationMinutes = 60
        });

        var jwtGenerator = new JwtTokenGenerator(jwtOptions);

        var logger = Mock.Of<ILogger<LoginHandler>>();

        var handler = new LoginHandler(
            context,
            jwtGenerator,
            logger,
            passwordHasher);

        var command = new LoginCommand(
            "inactivo",
            "Inactivo123*");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        result.Errors.Should().ContainSingle();

        result.FirstError?.Message.Should()
            .Be("Usuario inactivo");
    }
}