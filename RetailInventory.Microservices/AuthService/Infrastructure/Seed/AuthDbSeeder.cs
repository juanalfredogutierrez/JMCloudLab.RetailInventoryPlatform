using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Seeders;

public static class AuthDbSeeder
{
    public static async Task SeedAsync(AuthDbContext context)
    {
        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(
                new Rol
                {
                    Nombre = "ADMIN",
                    Descripcion = "Administrador del sistema"
                },
                new Rol
                {
                    Nombre = "COMPRAS",
                    Descripcion = "Gestión de compras e ingresos de stock"
                },
                new Rol
                {
                    Nombre = "VENTAS",
                    Descripcion = "Gestión de ventas y salidas de stock"
                });

            await context.SaveChangesAsync();
        }

        if (!await context.Usuarios.AnyAsync())
        {
            var roles = await context.Roles.ToDictionaryAsync(x => x.Nombre);

            var passwordHasher = new PasswordHasher<Usuario>();

            var usuarios = new List<Usuario>
            {
                new Usuario
                {
                    NombreUsuario = "admin",
                    CorreoElectronico = "admin@retailinventory.com",
                    Activo = true,
                    RolId = roles["ADMIN"].Id
                },
                new Usuario
                {
                    NombreUsuario = "compras",
                    CorreoElectronico = "compras@retailinventory.com",
                    Activo = true,
                    RolId = roles["COMPRAS"].Id
                },
                new Usuario
                {
                    NombreUsuario = "ventas",
                    CorreoElectronico = "ventas@retailinventory.com",
                    Activo = true,
                    RolId = roles["VENTAS"].Id
                }
            };

            usuarios[0].ClaveHash =
                passwordHasher.HashPassword(
                    usuarios[0],
                    "Admin123*");

            usuarios[1].ClaveHash =
                passwordHasher.HashPassword(
                    usuarios[1],
                    "Compras123*");

            usuarios[2].ClaveHash =
                passwordHasher.HashPassword(
                    usuarios[2],
                    "Ventas123*");

            context.Usuarios.AddRange(usuarios);

            await context.SaveChangesAsync();
        }
    }
}