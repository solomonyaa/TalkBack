using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talkback.Common;
using Talkback.Common.Backgammon;
using Talkback.Server.Interfaces;

namespace Talkback.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private IDal<Client> dal;
        private HttpHandler notifier;
        private Dictionary<string, PlayOptions> state;
        public GameController(IDal<Client> dal)
        {
            this.dal = dal;
            this.notifier = new HttpHandler();
            this.state = new Dictionary<string, PlayOptions>();
        }

        [HttpGet("play")]
        public IActionResult PlayRequest(string directedClient)
        {
            var clients = this.dal.GetAll().ToList();

            var onlineClients = clients.Where((c) => c.IsOnline == true).ToList();

            if (!clients.Exists((c) => c.UserName == directedClient))
                return NotFound($"user {directedClient} isn't registerd!");

            if (!onlineClients.Exists((c) => c.UserName == directedClient))
                return NotFound($"user {directedClient} is offline right now.");

            var client = onlineClients.Find((c) => c.UserName == directedClient);

            var resp = this.notifier.MakeHttpGet(client.URL, "game/playRequest");

            if (resp.IsSuccessStatusCode)
            {
                this.state[User.Identity.Name] = PlayOptions.WaitingForResponse;
                return Ok($"play request successfully sent to {directedClient}.");
            }

            return BadRequest("something went wrong");
        }
        [HttpGet("cancel")]
        public IActionResult CancelPlayRequest()
        {
            this.state[User.Identity.Name] = PlayOptions.Canceled;
            return Ok();
        }
        [HttpGet("response")]
        public IActionResult PlayResponse(string askingClient, string response)
        {
            PlayOptions result = PlayOptions.Default;
            Enum.TryParse(response, false, out result);
            var aClient = this.dal.Get(askingClient);

            switch (result)
            {
                case PlayOptions.Approved:
                    var currentClient = this.dal.Get(User.Identity.Name);
                    if (this.state[askingClient] == PlayOptions.Canceled)
                    {
                        this.notifier.MakeHttpGet(currentClient.URL, "game/canceledPlay");
                        return Ok("Game canceled by the other client");
                    }
                    this.notifier.MakeHttpGet(aClient.URL, "game/launchPlay");
                    this.notifier.MakeHttpGet(currentClient.URL, "game/launchPlay");
                    this.state[askingClient] = PlayOptions.Approved;
                    return Ok("Game launched");
                case PlayOptions.Rejected:
                    if (this.state[askingClient] == PlayOptions.Canceled)
                        return Ok();

                    this.notifier.MakeHttpGet(aClient.URL, "game/declinedPlay");
                    this.state[askingClient] = PlayOptions.Rejected;
                    return Ok();
                case PlayOptions.WaitingForResponse:
                case PlayOptions.Canceled:
                case PlayOptions.Default:
                    return BadRequest();

            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult PlayMove(Move move, string directedClient)
        {
            var c = this.dal.Get(directedClient);
            return (IActionResult)this.notifier.MakeHttpPost(c.URL, "backgammon/playedMove", null, move.ToString());
        }
    }
}
