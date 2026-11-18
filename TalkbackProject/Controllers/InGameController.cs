using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InGameController : ControllerBase
    {
        private IDal<Client> dal;
        private HttpHandler httpHandler; 
        public InGameController(IDal<Client> dal)
        {
            this.dal = dal;
            this.httpHandler = new HttpHandler();
        }

        [HttpGet("GetRoll")]
        public IActionResult GetRollResult(string opponentName, string cube1, string cube2)
        {
            var client = this.dal.Get(opponentName);
            var resp = httpHandler.MakeHttpGet(client.URL, "ingame/getroll", new Dictionary<string, string>()
            {
                {"cube1",cube1.ToString() },
                {"cube2",cube2.ToString() }
            });
            return Ok(resp);
        }

        [HttpGet("passTurn")]
        public IActionResult PassTurn(string opponentName)
        {
            var client = this.dal.Get(opponentName);
            var resp = httpHandler.MakeHttpGet(client.URL, "ingame/receiveTurn");
            return Ok(resp);

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
            var client = this.dal.Get(winnerName);

            var resp = this.httpHandler.MakeHttpGet(client.URL, "ingame/receiveGameResult", new Dictionary<string, string>()
            {
                {"winnerName",winnerName },
            });
            return Ok();
        }
    }
}
