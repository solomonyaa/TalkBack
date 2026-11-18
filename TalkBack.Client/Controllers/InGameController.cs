using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common;

namespace Talkback.Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InGameController : ControllerBase
    {
        private GameState state;
        private HttpHandler httpHandler;
        private string serverURL;


        public InGameController(Config config, [FromServices] GameState state)
        {
            this.serverURL = config.ServerUrl;
            this.state = state;
            this.httpHandler = new HttpHandler();
        }

        [HttpGet("passRoll")]
        public IActionResult PassRollResult(string opponentName, string cube1, string cube2)
        {
            var resp = this.httpHandler.MakeHttpGet(serverURL, "ingame/GetRoll", new Dictionary<string, string>()
            {
                {"cube1",cube1 },
                {"cube2",cube2 }
            }, this.state.Token.getToken());
            return Ok(resp);
        }
        [HttpGet("GetRoll")]
        public IActionResult GetRollResult(string cube1, string cube2)
        {
            this.state.CurrentCubes = (cube1, cube2);
            return Ok();
        }

        [HttpGet("ViewRoll")]
        public IActionResult ViewRollResult()
        {
            return Ok(this.state.CurrentCubes);
        }

        [HttpGet("passTurn")]
        public IActionResult PassTurn(string opponentName)
        {
            this.state.CanPlay = false;
            httpHandler.MakeHttpGet(serverURL, "ingame/passTurn", null, state.Token.getToken());
            return Ok();
        }
        [HttpGet("ReceiveTurn")]
        public IActionResult ReceiveTurn()
        {
            this.state.CanPlay = true;
            return Ok();
        }

        [HttpGet("viewTurn")]
        public IActionResult ViewTurn()
        {
            return Ok(this.state.CanPlay);
        }

        [HttpGet("passMoves")]
        public IActionResult PassMoves()
        {
            // send roll result
            return Ok();
        }
        [HttpGet("GetMoves")]
        public IActionResult GetMoves()
        {
            // send roll result
            return Ok();
        }
        [HttpGet("sendGameResult")]
        public IActionResult SendGameResult(string winnerName)
        {
            var resp = this.httpHandler.MakeHttpGet(serverURL, "ingame/sendGameResult", new Dictionary<string, string>()
            {
                {"winnerName",winnerName },
            }, this.state.Token.getToken());

            // send roll result
            return Ok();
        }
        [HttpGet("receiveGameResult")]
        public IActionResult ReceiveGameResult(string winnerName)
        {
            this.state.GameWinner = winnerName;
            return Ok();
        }
        [HttpGet("viewGameResult")]
        public IActionResult ViewResult()
        {
            return Ok(this.state.GameWinner);
        }

    }
}

