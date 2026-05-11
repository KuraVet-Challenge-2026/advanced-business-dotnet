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
    public class PetController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public PetController(KuraVetDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os pets", Description = "Retorna todos os pets cadastrados.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.Pets.ToListAsync();
                if (!resultado.Any()) return NoContent();
                return Ok(resultado);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter pet por ID", Description = "Busca um perfil completo de um pet pelo seu ID.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null) return NotFound();
                return Ok(pet);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("tutor/{tutorId:int}")]
        [SwaggerOperation(Summary = "Obter pets por Tutor", Description = "Lista todos os Pets vinculados a um Tutor específico.")]
        public async Task<IActionResult> GetPorTutor(int tutorId)
        {
            try
            {
                var pets = await _context.Pets.Where(p => p.TutorId == tutorId).ToListAsync();
                if (!pets.Any()) return NoContent();
                return Ok(pets);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo pet", Description = "Cadastra um novo Pet vinculando-o a um Tutor.")]
        public async Task<IActionResult> Post([FromBody] Pet model)
        {
            try
            {
                _context.Pets.Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar pet existente", Description = "Atualiza os dados de um pet já cadastrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] Pet model)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null) return NotFound();

                pet.Nome = model.Nome;
                pet.TutorId = model.TutorId;

                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um pet", Description = "Remove um pet do banco de dados.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null) return NotFound();

                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                return Ok(pet);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}