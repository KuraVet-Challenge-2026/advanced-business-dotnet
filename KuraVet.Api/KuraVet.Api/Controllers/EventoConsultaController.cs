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
    public class EventoConsultaController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public EventoConsultaController(KuraVetDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os eventos clínicos", Description = "Retorna todos os eventos clínicos cadastrados no banco.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.EventosConsultas.ToListAsync();
                if (!resultado.Any()) return NoContent();
                return Ok(resultado);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter evento por ID", Description = "Busca os detalhes de um Evento Clínico específico.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null) return NotFound();
                return Ok(evento);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
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

                if (!linhaDoTempo.Any()) return NoContent();
                return Ok(linhaDoTempo);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo evento", Description = "Registra um novo Evento Clínico (Ex: Vacina, Retorno, Exame, Cirurgia).")]
        public async Task<IActionResult> Post([FromBody] EventoConsulta model)
        {
            try
            {
                _context.EventosConsultas.Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar evento existente", Description = "Atualiza as informações de um evento clínico já registrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] EventoConsulta model)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null) return NotFound();

                evento.TipoEvento = model.TipoEvento;
                evento.DataEvento = model.DataEvento;
                evento.Descricao = model.Descricao;
                evento.VeterinarioResponsavel = model.VeterinarioResponsavel;
                evento.PetId = model.PetId;

                _context.EventosConsultas.Update(evento);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um evento", Description = "Remove permanentemente um evento clínico do sistema.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var evento = await _context.EventosConsultas.FindAsync(id);
                if (evento is null) return NotFound();

                _context.EventosConsultas.Remove(evento);
                await _context.SaveChangesAsync();
                return Ok(evento);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}