using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UtttApi.ObjectModel.Interfaces;
using UtttApi.ObjectModel.Models;

namespace UtttApi.WebApi.Controllers
{
    /// <summary>
    /// Controls the main game funtionality
    /// </summary>
    [ApiController]
    [Route("rest/uttt/[controller]")]
    public class UtttController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UtttController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get a game by its ID number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GameObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(string id)
        {
            GameObject game = await _unitOfWork.Game.SelectAsync(id);

            if (game == null)
            {
                return NotFound($"Could not find the game with id {id}");
            }

            return Ok(await _unitOfWork.Game.SelectAsync(id));
        }

        /// <summary>
        /// Create a new game and return the new Id
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync()
        {
            var id = await _unitOfWork.Game.InsertAsync(new GameObject());
            return Created($"/rest/uttt/uttt/{id}", id);
        }

        /// <summary>
        /// Make a move on a game by its ID number
        /// </summary>
        /// <param name="id"></param>
        /// <param name="player"></param>
        /// <param name="lbIndex"></param>
        /// <param name="markIndex"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GameObject), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutAsync(string id, MoveObject move)
        {
            GameObject game = await _unitOfWork.Game.SelectAsync(id);

            if (game == null)
            {
                return NotFound($"Could not find the game with id {id}");
            }

            if (game.Status != GameStatus.IN_PROGRESS)
            {
                return BadRequest($"Game {id}: The game is finished");
            }

            if (game.IsValidMove(move))
            {
                if (move.Mark == game.CurrentPlayer)
                {
                    game.MakeMove(move);
                    game.UpdateGameStatus();
                    await _unitOfWork.Game.UpdateAsync(game);
                    return Accepted(game);
                }
                return BadRequest($"Game {id}: It is not player {move.Mark.ToString("d")}'s turn");
            }

            return BadRequest($"Game {id}: The move ({move.LbIndex}, {move.MarkIndex}) is not valid for player {move.Mark.ToString("d")}");
        }

        /// <summary>
        /// Delete a game by its ID number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (await _unitOfWork.Game.DeleteAsync(id))
            {
                return NoContent();
            }

            return NotFound($"Could not find the game with id {id}");
        }
    }
}