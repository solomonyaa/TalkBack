using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talkback.Common;

namespace Talkback.Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private HttpHandler notifier;
        private string serverURL;
        private Token token;

        public MessageController(Config config, [FromServices]Token token)
        {
            this.serverURL = config.ServerUrl;
            this.notifier = new HttpHandler();
            this.token = token;
        }

        [HttpGet("sendmessage")]
        public IActionResult SendMessage(string otherClient, string message)
        {
            var reqParams = new Dictionary<string, string>
            {
                {"directedClient", otherClient }
            };
            var resp = notifier.MakeHttpPost(serverURL, "chat/send", reqParams, message, token.getToken());

            return Ok(resp);
        }
        [HttpGet("getmessages")]
        public IActionResult GetMessage(string otherClient)
        {
            var reqParams = new Dictionary<string, string>
            {
                {"directedClient", otherClient }
            };
            var resp = notifier.HttpGet<IEnumerable<Message>>(serverURL, "chat/getAllMessages", reqParams, token.getToken());

            return Ok(resp);
        }

        [HttpPost("messageSent")]
        public IActionResult ReceivedMessage(string otherClient, string message)
        {
            // update view with message and otherClient
            // if chat pane is not shown -> notify user
            // also - make sound
            return Ok(message);
        }
    }
}

