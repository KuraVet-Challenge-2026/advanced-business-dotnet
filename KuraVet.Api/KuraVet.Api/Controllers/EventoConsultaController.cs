using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class EventoConsultaController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        private readonly ILogger<EventoConsultaController> _logger;

        public EventoConsultaController(KuraVetDbContext context, ILogger<EventoConsultaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os eventos clínicos", Description = "Retorna todos os eventos clínicos cadastrados no banco.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.EventosConsultas.ToListAsync();
                if (!resultado.Any())
                {
                    _logger.LogInformation("Listagem de eventos clínicos retornou vazia.");
                    return NoContent();
                }

                _logger.LogInformation("Listagem de eventos clínicos retornou {Quantidade} registro(s).", resultado.Count);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao listar eventos clínicos.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter evento por ID", Description = "Busca os detalhes de um Evento Clínico específico.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null)
                {
                    _logger.LogWarning("Evento clínico {EventoId} não encontrado.", id);
                    return NotFound();
                }

                return Ok(evento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar o evento clínico {EventoId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pet/{petId:int}")]
        [SwaggerOperation(Summary = "Obter linha do tempo clínica do Pet", Description = "Busca todos os eventos clínicos atrelados a um Pet, ordenados por data.")]
        public async Task<IActionResult> GetPorPet(int petId)
        {
            try
            {
                var linhaDoTempo = await _context.EventosConsultas
                    .Where(e => e.PetId == petId)
                    .OrderByDescending(e => e.DataEvento)
                    .ToListAsync();

                if (!linhaDoTempo.Any())
                {
                    _logger.LogInformation("Pet {PetId} não possui eventos clínicos registrados.", petId);
                    return NoContent();
                }

                return Ok(linhaDoTempo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar a linha do tempo clínica do pet {PetId}.", petId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo evento", Description = "Registra um novo Evento Clínico (Ex: Vacina, Retorno, Exame, Cirurgia).")]
        public async Task<IActionResult> Post([FromBody] EventoConsulta model)
        {
            try
            {
                _context.EventosConsultas.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Evento clínico {EventoId} registrado para o pet {PetId}.", model.Id, model.PetId);
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao registrar evento clínico.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar evento existente", Description = "Atualiza as informações de um evento clínico já registrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] EventoConsulta model)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null)
                {
                    _logger.LogWarning("Tentativa de editar evento clínico inexistente {EventoId}.", id);
                    return NotFound();
                }

                evento.TipoEvento = model.TipoEvento;
                evento.DataEvento = model.DataEvento;
                evento.Descricao = model.Descricao;
                evento.VeterinarioResponsavel = model.VeterinarioResponsavel;
                evento.PetId = model.PetId;

                _context.EventosConsultas.Update(evento);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Evento clínico {EventoId} atualizado com sucesso.", id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar o evento clínico {EventoId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um evento", Description = "Remove permanentemente um evento clínico do sistema.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null)
                {
                    _logger.LogWarning("Tentativa de remover evento clínico inexistente {EventoId}.", id);
                    return NotFound();
                }

                _context.EventosConsultas.Remove(evento);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Evento clínico {EventoId} removido com sucesso.", id);
                return Ok(evento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao remover o evento clínico {EventoId}.", id);
                return BadRequest(ex.Message);
            }
        }
    }
}