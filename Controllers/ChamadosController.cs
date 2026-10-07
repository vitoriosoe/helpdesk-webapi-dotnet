using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskApi.Data;
using HelpDeskApi.Models;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChamadosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chamado>>> GetChamados()
        {
            return await _context.Chamados.Include(c => c.Usuario).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chamado>> GetChamado(int id)
        {
            var chamado = await _context.Chamados.Include(c => c.Usuario).FirstOrDefaultAsync(c => c.Id == id);
            if (chamado == null) return NotFound();
            return chamado;
        }

        [HttpPost]
        public async Task<ActionResult<Chamado>> PostChamado(Chamado chamado)
        {
            chamado.DataCriacao = DateTime.UtcNow;
            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetChamado), new { id = chamado.Id }, chamado);
        }
    }
}
