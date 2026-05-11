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
    public class TutorController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        public TutorController(KuraVetDbContext context) => _context = context;

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os tutores", Description = "Retorna todos os tutores cadastrados no sistema KuraVet.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.Tutores.ToListAsync();
                if (!resultado.Any()) return NoContent();
                return Ok(resultado);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter tutor por ID", Description = "Busca um tutor específico pelo seu ID.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null) return NotFound();
                return Ok(tutor);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo tutor", Description = "Cadastra um novo tutor no banco de dados.")]
        public async Task<IActionResult> Post([FromBody] Tutor model)
        {
            try
            {
                _context.Tutores.Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar tutor existente", Description = "Atualiza os dados de um tutor já cadastrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] Tutor model)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null) return NotFound();

                tutor.Nome = model.Nome;

                _context.Tutores.Update(tutor);
                await _context.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um tutor", Description = "Remove um tutor do banco de dados pelo ID.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null) return NotFound();

                _context.Tutores.Remove(tutor);
                await _context.SaveChangesAsync();
                return Ok(tutor);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}