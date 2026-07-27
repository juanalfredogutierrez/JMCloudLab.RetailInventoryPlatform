using AuthService.Application.Commands.Login;
using MediatR;

public class LoginHandler: IRequestHandler<LoginCommand, Result<string>>
{
    private readonly AuthDbContext _context;
    private readonly JwtTokenGenerator _jwt;
    private readonly ILogger<LoginHandler> _logger;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    public LoginHandler(
        AuthDbContext context,
        JwtTokenGenerator jwt,
        ILogger<LoginHandler> logger,
        PasswordHasher<Usuario> passwordHasher)
    {
        _context = context;
        _jwt = jwt;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<string>> Handle(LoginCommand request,CancellationToken cancellationToken)
    {
        var user = await _context.Usuarios
                         .Include(x => x.Rol)
                         .FirstOrDefaultAsync( x => x.NombreUsuario == request.Username,cancellationToken);

        if (user is null)
        {
            _logger.LogBusiness($"Login fallido para {request.Username}. Usuario no encontrado.");

            return Result<string>.Failure(Errors.Unauthorized("Credenciales inválidas"));
        }

        if (!user.Activo)
        {
            _logger.LogBusiness($"Login fallido para {request.Username}. Usuario inactivo.");

            return Result<string>.Failure(Errors.Unauthorized("Usuario inactivo"));
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.ClaveHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogBusiness($"Login fallido para {request.Username}. Contraseña inválida.");

            return Result<string>.Failure(Errors.Unauthorized("Credenciales inválidas"));
        }

        var token = _jwt.GenerateToken(
            user.Id,
            user.NombreUsuario,
            user.CorreoElectronico,
            user.Rol.Nombre);

        _logger.LogBusiness($"Usuario {request.Username} autenticado");

        return token;
    }
}