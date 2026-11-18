using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talkback.Common;

namespace Talkback.Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private HttpHandler notifier;
        private string serverURL;
        private GameState state;

        //update views
        public GameController(Config config, [FromServices] GameState state)
        {
            this.serverURL = config.ServerUrl;
            this.notifier = new HttpHandler();
            this.state = state;
        }
        [HttpGet("play")]
        public IActionResult Play(string oponnentUserName)
        {
            var reqParams = new Dictionary<string, string>
            {
                {"directedClient", oponnentUserName }
            };
            var resp = notifier.MakeHttpGet(serverURL, "game/play", reqParams);

            return Ok(resp);
        }
        [HttpGet("playRequest")]
        public IActionResult RespondToPlayRequest(string oponnentUserName, string response)
        {
            var reqParams = new Dictionary<string, string>
            {
                {"askingClient", oponnentUserName },
                {"response", response }
            };
            var resp = notifier.MakeHttpGet(serverURL, "game/response", reqParams);

            return Ok(resp);
        }
        [HttpGet("cancel")]
        public IActionResult CancelPlayRequest()
        {
            var resp = notifier.MakeHttpGet(serverURL, "game/cancel", null, this.state.Token.getToken());
            this.state.currentState = Common.Backgammon.PlayOptions.Canceled;

            //TODO Implement in view

            return Ok(resp);
        }


        [HttpGet("launchPlay")]
        public IActionResult LaunchGame()
        {
            this.state.currentState = Common.Backgammon.PlayOptions.Approved;
            return Ok();
        }
        [HttpGet("declinedPlay")]
        public IActionResult DeclinedGame()
        {
            this.state.currentState = Common.Backgammon.PlayOptions.Rejected;
            return Ok();
        }
        [HttpGet("canceledPlay")]
        public IActionResult CancelGame()
        {
            this.state.currentState = Common.Backgammon.PlayOptions.Rejected;

            return Ok();
        }

        [HttpGet("updateGameView")]
        public IActionResult GameStatus()
        {
            return Ok(this.state.currentState);
        }
    }
}

