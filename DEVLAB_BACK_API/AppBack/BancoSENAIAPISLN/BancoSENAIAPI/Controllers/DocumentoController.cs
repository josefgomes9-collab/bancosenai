using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(),
            "ClienteArquivos"
        );

        private static List<Models.DocumentoMetadado> _documentosMetadados =
            new List<Models.DocumentoMetadado>();

        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(
            int codigoCliente,
            IFormFile arquivo)
        {
            // Validação de arquivo vazio ou inexistente
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new
                {
                    mensagem = "Nenhum arquivo foi enviado."
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

            var documentoMetadados = new Models.DocumentoMetadado
            {
                Id = _nextId++,
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso",
                arquivoSalvo = novoNome
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarPorCliente(int codigoCliente)
        {
            var documentos = _documentosMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

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
        public IActionResult DownloadArquivo(int id)
        {
            var arquivo = _documentosMetadados
                .FirstOrDefault(d => d.Id == id);

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

            var fileBytes = System.IO.File.ReadAllBytes(arquivo.Caminho);

            string nomeArquivo =
                $"{arquivo.Name}{arquivo.Extensao}";

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo
            );
        }

        [HttpDelete("excluir/{id}")]
        public IActionResult ExcluirArquivo(int id)
        {
            var arquivo = _documentosMetadados
                .FirstOrDefault(d => d.Id == id);

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

            _documentosMetadados.Remove(arquivo);

            return Ok(new
            {
                mensagem = "O documento e arquivo foram excluídos com sucesso."
            });
        }
    }
}