using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OscarFilmeApi.Data;
using OscarFilmeApi.Models;

namespace OscarFilmeApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    public class FilmesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FilmesController(AppDbContext context) => _context = context;

    [HttpGet]
    public IActionResult Get()
        {

            var filmes = _context.filmes.ToList();  

            return Ok(filmes);

        }

    [HttpGet("vencedores/{venceu}")]
        public IActionResult Get(bool venceu)
        {

            var filmes = _context.filmes.Where(f => f.Venceu == venceu).ToList();
            if (filmes.Count == 0)
            {
                return NotFound();
            }

            return Ok(filmes);

        }

        [HttpGet("estatisticas")]
        public IActionResult Get(int id)
        {
            var ContagemFilme = _context.filmes.Count();
            var ContagemVencedores = _context.filmes.Count(f => f.Venceu == true);
            return Ok("Quantidade de filmes cadastrados: " + ContagemFilme + "Contagem de vencedores:" + ContagemVencedores);
            
        }

    [HttpPost]
        public IActionResult Post(Filme filme)
        {

            var AnoLancamento = filme.AnoLancamento;
            if (AnoLancamento < 1929)
            {
                return BadRequest("O ano de lançamento é menor que a data do primeiro Oscar, que foi em 1929.");    
            }
            
            _context.filmes.Add(filme);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = filme.Id }, filme);

        }

    [HttpPut("{id}")]
        public IActionResult Put(int id, Filme filme)
        {
            var filmeExistente = _context.filmes.Find(id);

            if (filmeExistente == null)
            {
                return NotFound();
            }

            if (filmeExistente.Venceu)
            {
                return Ok("Temos um novo vencedor! " + filmeExistente.Titulo);
            }

            _context.Entry(filmeExistente).CurrentValues.SetValues(filme);

            filmeExistente.Venceu = true;
            
            filmeExistente.Id = id; 

            _context.SaveChanges();
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var filme = _context.filmes.Find(id);
            if (filme == null)
            {
                return NotFound();
            }

            _context.filmes.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }


    }
}