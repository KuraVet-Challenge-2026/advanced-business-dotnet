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
    public class TutorController : ControllerBase
    {
        private readonly KuraVetDbContext _context;
        private readonly ILogger<TutorController> _logger;

        public TutorController(KuraVetDbContext context, ILogger<TutorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lista todos os tutores", Description = "Retorna todos os tutores cadastrados no sistema KuraVet.")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _context.Tutores.ToListAsync();
                if (!resultado.Any())
                {
                    _logger.LogInformation("Listagem de tutores retornou vazia.");
                    return NoContent();
                }

                _logger.LogInformation("Listagem de tutores retornou {Quantidade} registro(s).", resultado.Count);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao listar tutores.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Obter tutor por ID", Description = "Busca um tutor específico pelo seu ID.")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null)
                {
                    _logger.LogWarning("Tutor {TutorId} não encontrado.", id);
                    return NotFound();
                }

                return Ok(tutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao buscar o tutor {TutorId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adicionar novo tutor", Description = "Cadastra um novo tutor no banco de dados.")]
        public async Task<IActionResult> Post([FromBody] Tutor model)
        {
            try
            {
                _context.Tutores.Add(model);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tutor {TutorId} cadastrado com sucesso.", model.Id);
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao cadastrar tutor.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Editar tutor existente", Description = "Atualiza os dados de um tutor já cadastrado.")]
        public async Task<IActionResult> Put(int id, [FromBody] Tutor model)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null)
                {
                    _logger.LogWarning("Tentativa de editar tutor inexistente {TutorId}.", id);
                    return NotFound();
                }

                tutor.Nome = model.Nome;

                _context.Tutores.Update(tutor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tutor {TutorId} atualizado com sucesso.", id);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar o tutor {TutorId}.", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Deletar um tutor", Description = "Remove um tutor do banco de dados pelo ID.")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor is null)
                {
                    _logger.LogWarning("Tentativa de remover tutor inexistente {TutorId}.", id);
                    return NotFound();
                }

                _context.Tutores.Remove(tutor);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tutor {TutorId} removido com sucesso.", id);
                return Ok(tutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao remover o tutor {TutorId}.", id);
                return BadRequest(ex.Message);
            }
        }
    }
}