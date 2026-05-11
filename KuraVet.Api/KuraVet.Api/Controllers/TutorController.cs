using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TutorController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public TutorController(KuraVetDbContext context) => _context = context;

        /// <summary>Cadastra um novo Tutor no sistema KuraVet.</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Tutor tutor)
        {
            _context.Tutores.Add(tutor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(BuscarPorId), new { id = tutor.Id }, tutor);
        }

        /// <summary>Lista todos os Tutores cadastrados.</summary>
        [HttpGet]
        public async Task<IActionResult> ListarTodos() => Ok(await _context.Tutores.ToListAsync());

        /// <summary>Busca um Tutor específico pelo ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var tutor = await _context.Tutores.FindAsync(id);
            return tutor == null ? NotFound() : Ok(tutor);
        }
    }
}