using ProductoService.Domain.Entities;

namespace ProductoService.Infrastructure.Seeders;

public static class ProductoDbSeeder
{
    public static async Task SeedAsync(ProductoDbContext context)
    {
        if (await context.Productos.AnyAsync())
            return;

        var productos = new List<Producto>
        {
            new() { Codigo = "LAP-001", Nombre = "Laptop Dell Inspiron 15", Descripcion = "Intel Core i5, 16GB RAM, 512GB SSD", Precio = 2899.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "LAP-002", Nombre = "Laptop HP Pavilion", Descripcion = "Intel Core i7, 16GB RAM, 1TB SSD", Precio = 3599.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "LAP-003", Nombre = "Laptop Lenovo ThinkPad E14", Descripcion = "Ryzen 7, 16GB RAM, 512GB SSD", Precio = 3299.90m, Activo = true ,CreatedBy="Seeder"},

            new() { Codigo = "MON-001", Nombre = "Monitor LG 24\"", Descripcion = "Full HD IPS 75Hz", Precio = 649.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "MON-002", Nombre = "Monitor Samsung 27\"", Descripcion = "QHD 144Hz Gaming", Precio = 1299.90m, Activo = true ,CreatedBy="Seeder"},       
            new() { Codigo = "MOU-001", Nombre = "Mouse Logitech G203", Descripcion = "Mouse Gamer RGB", Precio = 129.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "MOU-002", Nombre = "Mouse Logitech MX Master 3S", Descripcion = "Mouse Inalámbrico Profesional", Precio = 429.90m, Activo = true ,CreatedBy="Seeder"},

            new() { Codigo = "KEY-001", Nombre = "Teclado Mecánico Redragon Kumara", Descripcion = "Switch Blue RGB", Precio = 199.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "KEY-002", Nombre = "Teclado Logitech MX Keys", Descripcion = "Teclado Inalámbrico Premium", Precio = 449.90m, Activo = true ,CreatedBy="Seeder"}, 
            new() { Codigo = "HDD-001", Nombre = "SSD Kingston NV2 1TB", Descripcion = "M.2 NVMe PCIe 4.0", Precio = 279.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "HDD-002", Nombre = "SSD Samsung 990 Pro 2TB", Descripcion = "M.2 NVMe Alto Rendimiento", Precio = 749.90m, Activo = true ,CreatedBy="Seeder"},

            new() { Codigo = "RAM-001", Nombre = "Memoria Kingston Fury 16GB", Descripcion = "DDR4 3200MHz", Precio = 189.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "RAM-002", Nombre = "Memoria Corsair Vengeance 32GB", Descripcion = "DDR5 5600MHz", Precio = 499.90m, Activo = true ,CreatedBy="Seeder"},  
            new() { Codigo = "TAB-001", Nombre = "Tablet Samsung Galaxy Tab S9", Descripcion = "256GB WiFi", Precio = 2999.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "TAB-002", Nombre = "iPad Air 11", Descripcion = "128GB WiFi", Precio = 3499.90m, Activo = true ,CreatedBy="Seeder"},

            new() { Codigo = "PHN-001", Nombre = "Samsung Galaxy S25", Descripcion = "256GB", Precio = 4299.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "PHN-002", Nombre = "iPhone 16", Descripcion = "128GB", Precio = 4899.90m, Activo = true ,CreatedBy="Seeder"},     
            new() { Codigo = "PRN-001", Nombre = "Impresora Epson L4260", Descripcion = "Multifuncional WiFi", Precio = 999.90m, Activo = true ,CreatedBy="Seeder"},
            new() { Codigo = "PRN-002", Nombre = "Impresora HP LaserJet MFP", Descripcion = "Láser Monocromática", Precio = 1299.90m, Activo = true ,CreatedBy="Seeder"},

            new() { Codigo = "WEB-001", Nombre = "WebCam Logitech C920", Descripcion = "Full HD 1080p", Precio = 299.90m, Activo = true ,CreatedBy="Seeder"}
        };

        await context.Productos.AddRangeAsync(productos);
        await context.SaveChangesAsync();
    }
}