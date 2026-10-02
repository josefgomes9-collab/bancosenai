using System.ComponentModel.DataAnnotations;


namespace BancoSENAIAPI.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string NomeUsusario { get; set; }
        [Required]
        public required string SenhaHash {  get; set; }
    }
}
