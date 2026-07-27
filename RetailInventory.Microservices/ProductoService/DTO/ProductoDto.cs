namespace ProductoService.DTO
{
    public record ProductoDto(
        int productoId,
     Guid Uid,
     string Codigo,
     string Nombre,
     decimal Precio,
     bool Activo);
}
