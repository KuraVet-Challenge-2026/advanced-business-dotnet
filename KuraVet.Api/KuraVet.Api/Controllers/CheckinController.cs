using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os check-ins", Description = "Retorna o histórico global de check-ins de saúde.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.CheckinsHistoricos.ToListAsync();
                if (!resultado.Any()) return NoContent();
                return Ok(resultado);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter check-in por ID", Description = "Busca os detalhes de um check-in de saúde específico.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var checkin = await _context.CheckinsHistoricos.FindAsync(id);
                if (checkin == null) return NotFound();
                return Ok(checkin);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("pet/{petId:int}")]
        [SwaggerOperation(Summary = "Obter histórico por Pet", Description = "Retorna todos os check-ins realizados para um pet específico.")]
        public async Task<IActionResult> GetPorPet(int petId)
        {
            try
            {
                var historico = await _context.CheckinsHistoricos
                    .Where(c => c.PetId == petId)
                    .OrderByDescending(c => c.DataCheckin)
                    .ToListAsync();

                if (!historico.Any()) return NoContent();
                return Ok(historico);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("risco/{nivelRisco}")]
        [SwaggerOperation(Summary = "Filtrar por nível de risco", Description = "Busca check-ins filtrando pelo status da IA (Baixo, Moderado, Grave).")]
        public async Task<IActionResult> GetPorRisco(string nivelRisco)
        {
            try
            {
                var alertas = await _context.CheckinsHistoricos
                    .Where(c => c.NivelRiscoIA != null && c.NivelRiscoIA.ToLower() == nivelRisco.ToLower())
                    .ToListAsync();

                if (!alertas.Any()) return NoContent();
                return Ok(alertas);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Registrar novo check-in", Description = "Cadastra os sinais vitais e gera automaticamente o nível de risco via IA.")]
        public async Task<IActionResult> Post([FromBody] CheckinHistorico model)
        {
            try
            {
                // Lógica da IA simplificada
                model.NivelRiscoIA = model.TempoPreenchimentoCapilar > 2 ? "Moderado" : "Baixo";

                _context.CheckinsHistoricos.Add(model);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar check-in existente", Description = "Atualiza os dados de sinais vitais de um check-in já realizado.")]
        public async Task<IActionResult> Put(int id, [FromBody] CheckinHistorico model)
        {
            try
            {
                var checkinExistente = await _context.CheckinsHistoricos.FindAsync(id);
                if (checkinExistente == null) return NotFound();

                checkinExistente.FrequenciaRespiratoria = model.FrequenciaRespiratoria;
                checkinExistente.TempoPreenchimentoCapilar = model.TempoPreenchimentoCapilar;
                checkinExistente.CorMucosa = model.CorMucosa;
                checkinExistente.NivelHidratacao = model.NivelHidratacao;
                checkinExistente.PetId = model.PetId;

                // Recalcula o risco na edição
                checkinExistente.NivelRiscoIA = model.TempoPreenchimentoCapilar > 2 ? "Moderado" : "Baixo";

                _context.CheckinsHistoricos.Update(checkinExistente);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um check-in", Description = "Remove permanentemente um registro de check-in do histórico.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var checkin = await _context.CheckinsHistoricos.FindAsync(id);
                if (checkin == null) return NotFound();

                _context.CheckinsHistoricos.Remove(checkin);
                await _context.SaveChangesAsync();
                return Ok(checkin);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}