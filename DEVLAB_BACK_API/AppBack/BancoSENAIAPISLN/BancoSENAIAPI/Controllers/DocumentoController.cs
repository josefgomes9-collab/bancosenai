using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }
         [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(
            int codigoCliente,
            IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new
                {
                    mensagem = "Nenhum arquivo foi enviado."
                });
            }

            if (!await _context.Cliente.AnyAsync(c => c.CodigoCliente == codigoCliente))
            {
                return NotFound(new
                {
                    mensagem = "Cliente não encontrado."
                });
            }

            // R06F - Limite máximo de 2 MB
            const long limiteTamanho = 2 * 1024 * 1024;

            if (arquivo.Length > limiteTamanho)
            {
                return BadRequest(new
                {
                    mensagem = "O arquivo excede o limite máximo permitido de 2 MB."
                });
            }

            // R06G - Extensões permitidas
            string extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();

            string[] extensoesPermitidas =
            {
                ".pdf",
                ".jpg",
                ".png"
            };

            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest(new
                {
                    mensagem = "Extensão de arquivo não permitida. Apenas arquivos .pdf, .jpg e .png são aceitos."
                });
            }

            string pastaCliente = Path.Combine(
                _caminhoRaiz,
                codigoCliente.ToString()
            );

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nomeOriginal =
                Path.GetFileNameWithoutExtension(arquivo.FileName);

            string novoNome =
                $"{codigoCliente}{nomeOriginal}{Guid.NewGuid()}{extensao}";

            string caminhoFinal =
                Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(
                caminhoFinal,
                FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new DocumentoMetadado
           {    Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            await _context.DocumentoMetadado.AddAsync(documentoMetadados);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso",
                arquivoSalvo = novoNome
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarPorCliente(int codigoCliente)
        {
            var documentos = await _context.DocumentoMetadado
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound(new
                {
                    mensagem = "Nenhum documento encontrado."
                });
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadArquivo(int id)
        {
            var arquivo = await _context.DocumentoMetadado
                .FirstOrDefaultAsync(d => d.Id == id);

            if (arquivo == null)
            {
                return NotFound(new
                {
                    mensagem = "Documento não encontrado."
                });
            }

            if (!System.IO.File.Exists(arquivo.Caminho))
            {
                return NotFound(new
                {
                    mensagem = "Arquivo físico não encontrado no servidor."
                });
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(arquivo.Caminho);

            string nomeArquivo =
                $"{arquivo.Name}{arquivo.Extensao}";

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo
            );
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> ExcluirArquivo(int id)
        {
            var arquivo = await _context.DocumentoMetadado
                .FirstOrDefaultAsync(d => d.Id == id);

            if (arquivo == null)
            {
                return NotFound(new
                {
                    mensagem = "Documento não encontrado."
                });
            }

            if (System.IO.File.Exists(arquivo.Caminho))
            {
                System.IO.File.Delete(arquivo.Caminho);
            }

            _context.DocumentoMetadado.Remove(arquivo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "O documento e arquivo foram excluídos com sucesso."
            });
        }
    }
}