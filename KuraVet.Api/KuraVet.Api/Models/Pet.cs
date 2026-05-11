using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KuraVet.Api.Models
{
    [Table("TB_KV_PET")]
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public int TutorId { get; set; }

        [JsonIgnore] 
        public Tutor? Tutor { get; set; }

        [JsonIgnore]
        public ICollection<CheckinHistorico>? Checkins { get; set; }
    }
}