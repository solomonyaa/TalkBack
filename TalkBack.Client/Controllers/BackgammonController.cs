using Microsoft.AspNetCore.Mvc;
using Talkback.Common.Backgammon;

namespace Talkback.Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BackgammonController : ControllerBase
    {
        private bool gameEnded;

        public BackgammonController()
        {
            gameEnded = false;
        }
        [HttpGet("makeMove")]
        public IActionResult DisplayOponenntDice(Move move)
        {
            if (gameEnded)
                return BadRequest("game already ended");
            //send to server the move
            return Ok();
        }
        [HttpGet("role")]
        public IActionResult DisplayOpponentDice(int num)
        {
            if (gameEnded)
                return BadRequest("game already ended");
            //update view with dice num
            return Ok();
        }
        [HttpGet("showMove")]
        public IActionResult DisplayMove(Move move)
        {
            if (gameEnded)
                return BadRequest("game already ended");
            //update view with move
            return Ok();
        }
        [HttpGet("showWinner")]
        public IActionResult DisplayMove(string winnerUserName)
        {
            //update view with move
            return Ok();
        }
    }
}
