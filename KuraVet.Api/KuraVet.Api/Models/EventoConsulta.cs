using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KuraVet.Api.Models
{
    [Table("TB_KV_EVENTO_CONSULTA")]
    public class EventoConsulta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoEvento { get; set; } = string.Empty; 

        [Required]
        public DateTime DataEvento { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string VeterinarioResponsavel { get; set; } = string.Empty;

        [Required]
        public int PetId { get; set; }

        [JsonIgnore]
        public Pet? Pet { get; set; }
    }
}