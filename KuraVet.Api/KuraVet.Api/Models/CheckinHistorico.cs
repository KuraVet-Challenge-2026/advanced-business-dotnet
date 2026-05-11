using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KuraVet.Api.Models
{
    [Table("TB_KV_CHECKIN_HISTORICO")]
    public class CheckinHistorico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime DataCheckin { get; set; } = DateTime.Now;

        [Required]
        public int FrequenciaRespiratoria { get; set; }

        [Required]
        public int TempoPreenchimentoCapilar { get; set; }

        [Required]
        [MaxLength(50)]
        public string CorMucosa { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NivelHidratacao { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? NivelRiscoIA { get; set; }

        [Required]
        public int PetId { get; set; }

        [JsonIgnore] // O pulo do gato para o Swagger ficar limpo!
        public Pet? Pet { get; set; }
    }
}