using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIMovies : ControllerBase
    {
        private readonly MvcMovieContext _context;

        public APIMovies(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: api/APIMovies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovie()
        {
            return await _context.Movie
                .Where(x => x.Validate == "Y")
                .Select(x => MovieToDTO(x))
                .ToListAsync();
        }

        // GET: api/APIMovies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(int id)
        {
            var Movie = await _context.Movie.FindAsync(id);

            if (Movie == null)
            {
                return NotFound();
            }

            return MovieToDTO(Movie);
        }

        // PUT: api/APIMovies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, MovieDTO movieDTO)
        {
            if (id != movieDTO.Id)
            {
                return BadRequest();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            movie.Title = movieDTO.Title;
            movie.ReleaseDate = movieDTO.ReleaseDate;
            movie.Price = movieDTO.Price;
            movie.Genre = movieDTO.Genre;
            movie.Rating = movieDTO.Rating;

            //_context.Entry(Movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!MovieExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/APIMovies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovieDTO>> PostMovie(MovieDTO movieDTO)
        {
            var movie = new Movie
            {
                Title = movieDTO.Title,
                ReleaseDate = movieDTO.ReleaseDate,
                Price = movieDTO.Price,
                Genre = movieDTO.Genre,
                Rating = movieDTO.Rating
            };

            _context.Movie.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMovie), 
                new { id = movie.Id },
                MovieToDTO(movie));
        }

        // DELETE: api/APIMovies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var Movie = await _context.Movie.FindAsync(id);
            if (Movie == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(Movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }

        private static MovieDTO MovieToDTO(Movie movie)
        {
            if(movie.Validate == "Y")
            {
                return new MovieDTO
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    ReleaseDate = movie.ReleaseDate,
                    Price = movie.Price,
                    Genre = movie.Genre,
                    Rating = movie.Rating
                };
            }
            return null;
        }
        
    }
}
