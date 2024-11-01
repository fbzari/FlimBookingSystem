﻿using Flim.API.Common;
using Flim.Application.DTOs;
using Flim.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Flim.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Flim.Application.Records;
using Microsoft.AspNetCore.Authorization;


namespace Flim.API.Controllers
{
    /// <summary>
    /// This Controller has all film operation like fetch, add.
    /// </summary>
    [Route("api/film")]
    [ApiController]
    [Authorize]
    public class FlimController : ControllerBase
    {
        private readonly IFilmService _filmService;

        public FlimController(IFilmService filmService)
        {
            _filmService = filmService;
        }

        /// <summary>
        /// Used to create film. Admin only has access
        /// </summary>
        /// <param name="filmDto"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFilm([FromBody] AddFilmDTO filmDto)
        {
            if (filmDto == null)
            {
                return BadRequest(ApiResponse<string>.Failure("Should Not Be an Empty", (int)HttpStatusCode.BadRequest));
            }

            var flim = await _filmService.CreateFilmAsync(filmDto);

            if (!flim)
            {
                return BadRequest(ApiResponse<string>.Failure("Film not created.", (int)HttpStatusCode.BadRequest));
            }

            return Ok(ApiResponse<string>.Success("Created",statusCode:(int)HttpStatusCode.Created));    

        }

        /// <summary>
        /// You can fetch a film by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("flim-name")]
        public async Task<IActionResult> GetFlimByName([FromQuery] string name)
        {
            var flims = await _filmService.GetFilmByNameAsync(name);

            if(flims is null || flims.Count() == 0)
            {
                return NotFound(ApiResponse<string>.Failure(message: $"{name} not found!", statusCode: (int)HttpStatusCode.NotFound));
            }

            var filmsDto = flims.Select(film => new ShowFilmRecord(

                film.Name, film.Description, film.Genre, film.Duration, film.Amount

            ));


            return Ok(ApiResponse<IEnumerable<ShowFilmRecord>>.Success(filmsDto, statusCode:(int)HttpStatusCode.OK));
        }

        /// <summary>
        /// You can fetch a film by genre
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        [HttpGet("flim-genre")]
        public async Task<IActionResult> GetFlimByGenre([FromQuery] string genre)
        {
            var flims = await _filmService.GetFilmByGenreAsync(genre);

            if (flims is null || flims.Count() == 0)
            {
                return NotFound(ApiResponse<string>.Failure(message: $"{genre} not found!", statusCode: (int)HttpStatusCode.NotFound));
            }

            var filmsDto = flims.Select(film => new ShowFilmRecord(

                film.Name, film.Description, film.Genre, film.Duration, film.Amount

            ));


            return Ok(ApiResponse<IEnumerable<ShowFilmRecord>>.Success(filmsDto, statusCode: (int)HttpStatusCode.OK));
        }

        /// <summary>
        /// You can fetxh film by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("film-id/{id:int}")]
        public async Task<IActionResult> GetFlimById([FromRoute] int id)
        {
            var film = await _filmService.GetFilmByIdAsync(id);

            if(film is null)
            {
                return NotFound(ApiResponse<string>.Failure($"Flim Not Found for this Id : {id}", statusCode: (int)HttpStatusCode.NotFound));
            }

            var result = new ShowFilmRecord(name: film.Name,Description:film.Description, Genre: film.Genre,Duration:film.Duration,amount:film.Amount);
            

            return Ok(ApiResponse<ShowFilmRecord>.Success(result, statusCode: (int)HttpStatusCode.OK));
        }

        /// <summary>
        /// You can fetch all films
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<IActionResult> GetAllAsync()
        {
            var films = await  _filmService.GetFilmAsync();

            if(films is null ||films.Count() == 0)
            {
                return NotFound();
            }


            var filmDtos = films.Select(f => new FilmDTO
            {
                Description = f.Description,
                Duration = f.Duration,
                FilmId = f.FilmId,
                Genre = f.Genre,
                Name = f.Name,
                Slots = f.Slots.Select(s => new Slots
                {
                    ShowCategory = s.ShowCategory.ToString(),
                    FilmId = s.FilmId,
                    SlotDate = s.SlotDate,
                    SlotId = s.SlotId,
                    Seats = s.Seats.Select(st => new SeatsDTO
                    {
                        SlotId = st.SlotId,
                        IsReserved = st.IsReserved,
                        Number = st.Number,
                        Row = st.Row,
                        SeatId = st.SeatId
                    }).ToList()


                }).ToList()

            });

            return Ok(filmDtos);
      
        }

        /// <summary>
        /// Track sales of films. Admin has only access.
        /// </summary>
        /// <returns></returns>
        [HttpGet("sales")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSalesAsync()
        {

           var result =  await _filmService.GetSales();

            return Ok(result);

        }

    }
}
