using KuraVet.Api.Data;
using KuraVet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PetController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public PetController(KuraVetDbContext context) => _context = context;

        /// <summary>Cadastra um novo Pet vinculando-o a um Tutor.</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(BuscarPorId), new { id = pet.Id }, pet);
        }

        /// <summary>Busca o perfil completo de um Pet.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            return pet == null ? NotFound() : Ok(pet);
        }

        /// <summary>Lista todos os Pets de um Tutor específico.</summary>
        [HttpGet("tutor/{tutorId:int}")]
        public async Task<IActionResult> BuscarPorTutor(int tutorId)
        {
            var pets = await _context.Pets.Where(p => p.TutorId == tutorId).ToListAsync();
            return Ok(pets);
        }
    }
}