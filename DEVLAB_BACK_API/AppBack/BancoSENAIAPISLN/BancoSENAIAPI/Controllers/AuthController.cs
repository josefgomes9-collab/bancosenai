using BancoSENAIAPI.Data;
using BancoSENAIAPI.Dtos;
using BancoSENAIAPI.Models;
using BancoSENAIAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/1/[controller]")]
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegisterRequest dto)
        {
            if (await _context.Usuario.AnyAsync(u => u.NomeUsuario == dto.NomeUsuario))
            {
                return BadRequest(new { message = "Este nome do usuário já está em uso" });
            }

            var usuario = new Usuario
            {
                NomeUsusario = dto.NomeUsuario,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return Created("", new { usuario.Id, usuario.NomeUsusario });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.NomeUsuario == dto.NomeUsuario);

            {
                if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                {
                    return Unauthorized(new { message = "Usuário ou senha inválidos" });
                }
            }

            var (token, expiradaEm) = _tokenService.GerarToken(usuario);

            return Ok(new LoginResponseDto { Token = token, ExpiraEm = expiradaEm });
        }
    }
}