using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CheckinController : ControllerBase
    {
        private readonly KuraVetDbContext _context;

        public CheckinController(KuraVetDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra um novo Check-in de saúde física para um Pet.
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/Checkin
        ///     {
        ///        "frequenciaRespiratoria": 25,
        ///        "tempoPreenchimentoCapilar": 1,
        ///        "corMucosa": "Rosada",
        ///        "nivelHidratacao": "Normal",
        ///        "petId": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Check-in registrado com sucesso.</response>
        /// <response code="400">Se os dados enviados forem inválidos ou incompletos.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarCheckin([FromBody] CheckinHistorico checkin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Simulação da IA
            checkin.NivelRiscoIA = checkin.TempoPreenchimentoCapilar > 2 ? "Moderado" : "Baixo";

            _context.CheckinsHistoricos.Add(checkin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(BuscarPorId), new { id = checkin.Id }, checkin);
        }

        /// <summary>
        /// Busca um Check-in específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do Check-in no banco de dados.</param>
        /// <response code="200">Retorna os dados do Check-in solicitado.</response>
        /// <response code="404">Se o Check-in não for encontrado.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var checkin = await _context.CheckinsHistoricos.FindAsync(id);
            if (checkin == null) return NotFound(new { mensagem = "Check-in não encontrado." });
            return Ok(checkin);
        }

        /// <summary>
        /// Lista o histórico completo de Check-ins de um Pet específico.
        /// </summary>
        /// <param name="petId">O ID do Pet (ex: 1).</param>
        /// <response code="200">Retorna a lista de check-ins ordenados por data.</response>
        /// <response code="404">Se o Pet não possuir nenhum histórico.</response>
        [HttpGet("pet/{petId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorPet(int petId)
        {
            var historico = await _context.CheckinsHistoricos
                .Where(c => c.PetId == petId)
                .OrderByDescending(c => c.DataCheckin)
                .ToListAsync();

            if (!historico.Any()) return NotFound(new { mensagem = "Nenhum check-in encontrado." });
            return Ok(historico);
        }

        /// <summary>
        /// Filtra os Check-ins baseados no nível de Risco definido pela IA.
        /// </summary>
        /// <param name="nivelRisco">O nível de risco (ex: Baixo, Moderado, Grave).</param>
        /// <response code="200">Retorna os alertas correspondentes ao nível de risco.</response>
        [HttpGet("risco/{nivelRisco}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarPorRisco(string nivelRisco)
        {
            var alertas = await _context.CheckinsHistoricos
                .Where(c => c.NivelRiscoIA != null && c.NivelRiscoIA.ToLower() == nivelRisco.ToLower())
                .ToListAsync();
            return Ok(alertas);
        }

        /// <summary>
        /// Atualiza os dados de um Check-in existente.
        /// </summary>
        /// <param name="id">ID do Check-in a ser atualizado.</param>
        /// <param name="checkinAtualizado">O JSON contendo os novos dados do check-in.</param>
        /// <response code="204">Check-in atualizado com sucesso.</response>
        /// <response code="400">Se o ID da URL for diferente do ID no corpo da requisição.</response>
        /// <response code="404">Se o Check-in não existir.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AtualizarCheckin(int id, [FromBody] CheckinHistorico checkinAtualizado)
        {
            if (id != checkinAtualizado.Id) return BadRequest();

            var checkinExistente = await _context.CheckinsHistoricos.FindAsync(id);
            if (checkinExistente == null) return NotFound();

            checkinExistente.FrequenciaRespiratoria = checkinAtualizado.FrequenciaRespiratoria;
            checkinExistente.TempoPreenchimentoCapilar = checkinAtualizado.TempoPreenchimentoCapilar;
            checkinExistente.CorMucosa = checkinAtualizado.CorMucosa;
            checkinExistente.NivelHidratacao = checkinAtualizado.NivelHidratacao;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deleta permanentemente um Check-in do histórico do Pet.
        /// </summary>
        /// <param name="id">ID do Check-in a ser deletado.</param>
        /// <response code="204">Remoção concluída com sucesso.</response>
        /// <response code="404">Se o Check-in não for encontrado.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletarCheckin(int id)
        {
            var checkin = await _context.CheckinsHistoricos.FindAsync(id);
            if (checkin == null) return NotFound();

            _context.CheckinsHistoricos.Remove(checkin);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}