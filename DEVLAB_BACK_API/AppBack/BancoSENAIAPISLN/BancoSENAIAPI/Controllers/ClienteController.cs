using BancoSENAIAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private static List<Cliente> _clientes = new List<Cliente>();
        private static int _proximoCodigo = 1;

        [HttpGet]
        public IActionResult ListarTodos()
        {
            return Ok(_clientes);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente novoCliente)
        {
            if (novoCliente == null)
                return BadRequest(new { message = "Dados inválidos." });

            if (string.IsNullOrWhiteSpace(novoCliente.NomeCliente))
                return BadRequest(new { message = "Nome do cliente é obrigatório." });

            if (string.IsNullOrWhiteSpace(novoCliente.CPF))
                return BadRequest(new { message = "CPF é obrigatório." });

            if (_clientes.Any(c => c.CPF == novoCliente.CPF))
                return BadRequest(new { message = "Já existe um cliente com este CPF." });

            novoCliente.CodigoCliente = _proximoCodigo++;

            if (novoCliente.NumeroAgencia == 0) novoCliente.NumeroAgencia = 10;

            _clientes.Add(novoCliente);
            return Created("", novoCliente);
        }

        [HttpPut("{codigo}")]
        public IActionResult Atualizar(int codigo, [FromBody] Cliente clienteAtualizado)
        {
            var clienteExistente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);
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

            return NoContent();
        }

        [HttpGet("{codigo}")]
        public IActionResult ConsultarPorCodigo(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);
            if (cliente == null) return NotFound(new { message = "Cliente não encontrado." });
            return Ok(cliente);
        }

        [HttpDelete("{codigo}")]
        public IActionResult Excluir(int codigo)
        {
            var cliente = _clientes.FirstOrDefault(c => c.CodigoCliente == codigo);
            if (cliente == null) return NotFound(new { message = "Cliente não encontrado." });

            _clientes.Remove(cliente);
            return Ok(new { message = "Cliente excluído com sucesso." });
        }
    }
}