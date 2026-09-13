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
    public class PetController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        private readonly ILogger<PetController> _logger;

        public PetController(KuraVetDbContext context, ILogger<PetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os pets", Description = "Retorna todos os pets cadastrados.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.Pets.ToListAsync();
                if (!resultado.Any())
                {
                    _logger.LogInformation("Listagem de pets retornou vazia.");
                    return NoContent();
                }

                _logger.LogInformation("Listagem de pets retornou {Quantidade} registro(s).", resultado.Count);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao listar pets.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter pet por ID", Description = "Busca um perfil completo de um pet pelo seu ID.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null)
                {
                    _logger.LogWarning("Pet {PetId} não encontrado.", id);
                    return NotFound();
                }

                return Ok(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar o pet {PetId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tutor/{tutorId:int}")]
        [SwaggerOperation(Summary = "Obter pets por Tutor", Description = "Lista todos os Pets vinculados a um Tutor específico.")]
        public async Task<IActionResult> GetPorTutor(int tutorId)
        {
            try
            {
                var pets = await _context.Pets.Where(p => p.TutorId == tutorId).ToListAsync();
                if (!pets.Any())
                {
                    _logger.LogInformation("Tutor {TutorId} não possui pets cadastrados.", tutorId);
                    return NoContent();
                }

                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar pets do tutor {TutorId}.", tutorId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo pet", Description = "Cadastra um novo Pet vinculando-o a um Tutor.")]
        public async Task<IActionResult> Post([FromBody] Pet model)
        {
            try
            {
                _context.Pets.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pet {PetId} cadastrado com sucesso para o tutor {TutorId}.", model.Id, model.TutorId);
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao cadastrar pet.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar pet existente", Description = "Atualiza os dados de um pet já cadastrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] Pet model)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null)
                {
                    _logger.LogWarning("Tentativa de editar pet inexistente {PetId}.", id);
                    return NotFound();
                }

                pet.Nome = model.Nome;
                pet.TutorId = model.TutorId;

                _context.Pets.Update(pet);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pet {PetId} atualizado com sucesso.", id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar o pet {PetId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um pet", Description = "Remove um pet do banco de dados.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pet = await _context.Pets.FindAsync(id);
                if (pet is null)
                {
                    _logger.LogWarning("Tentativa de remover pet inexistente {PetId}.", id);
                    return NotFound();
                }

                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pet {PetId} removido com sucesso.", id);
                return Ok(pet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao remover o pet {PetId}.", id);
                return BadRequest(ex.Message);
            }
        }
    }
}