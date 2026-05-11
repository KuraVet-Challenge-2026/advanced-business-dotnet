using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EventoConsultaController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public EventoConsultaController(KuraVetDbContext context) => _context = context;

        /// <summary>Registra um novo Evento Clínico (Ex: Vacina, Retorno, Exame).</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] EventoConsulta evento)
        {
            _context.EventosConsultas.Add(evento);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(BuscarPorId), new { id = evento.Id }, evento);
        }

        /// <summary>Busca os detalhes de um Evento Clínico.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var evento = await _context.EventosConsultas.FindAsync(id);
            return evento == null ? NotFound() : Ok(evento);
        }

        /// <summary>Busca a Linha do Tempo Clínica completa de um Pet.</summary>
        [HttpGet("pet/{petId:int}")]
        public async Task<IActionResult> LinhaDoTempoPet(int petId)
        {
            var linhaDoTempo = await _context.EventosConsultas
                .Where(e => e.PetId == petId)
                .OrderByDescending(e => e.DataEvento)
                .ToListAsync();
            return Ok(linhaDoTempo);
        }
    }
}