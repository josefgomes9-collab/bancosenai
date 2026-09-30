using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var clientes = await _context.Cliente.ToListAsync();
            return Ok(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente novoCliente)
        {
            if (novoCliente == null)
                return BadRequest(new { message = "Dados inválidos." });

            if (string.IsNullOrWhiteSpace(novoCliente.NomeCliente))
                return BadRequest(new { message = "Nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(novoCliente.CPF))
                return BadRequest(new { message = "CPF é obrigatório." });

            if (await _context.Cliente.AnyAsync(c => c.CPF == novoCliente.CPF))
                return BadRequest(new { message = "Já existe um cliente com este CPF." });

            novoCliente.CodigoCliente = 0;

            if (novoCliente.NumeroAgencia == 0) novoCliente.NumeroAgencia = 10;

            await _context.Cliente.AddAsync(novoCliente);
            await _context.SaveChangesAsync();

            return Created("", novoCliente);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Atualizar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (clienteExistente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            if (string.IsNullOrWhiteSpace(clienteAtualizado.NomeCliente))
                return BadRequest(new { message = "Nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(clienteAtualizado.CPF))
                return BadRequest(new { message = "CPF é obrigatório." });

            clienteExistente.NomeCliente = clienteAtualizado.NomeCliente;
            clienteExistente.CPF = clienteAtualizado.CPF;
            clienteExistente.NumeroAgencia = clienteAtualizado.NumeroAgencia == 0 ? 10 : clienteAtualizado.NumeroAgencia;
            clienteExistente.SaldoTotal = clienteAtualizado.SaldoTotal;
            clienteExistente.Sexo = clienteAtualizado.Sexo;
            clienteExistente.Endereco = clienteAtualizado.Endereco;
            clienteExistente.Cidade = clienteAtualizado.Cidade;
            clienteExistente.Estado = clienteAtualizado.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> ConsultarPorCodigo(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            return Ok(cliente);
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Excluir(int codigo)
        {
            var cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
                return NotFound(new { message = "Cliente não encontrado." });

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}