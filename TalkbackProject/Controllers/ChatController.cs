using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IDal<Client> dal;
        private IDal<Message> mDal;
        private HttpHandler notifier;

        public ChatController(IDal<Client> dal, IDal<Message> mdal)
        {
            this.dal = dal;
            this.mDal = mdal;
            this.notifier = new HttpHandler();
        }
        [HttpPost("send")]
        public IActionResult ChatRequest(string directedClient, [FromForm] string message)
        {
            var clients = this.dal.GetAll().ToList();

            var onlineClients = clients.Where((c) => c.IsOnline == true).ToList();

            if (!clients.Exists((c) => c.UserName == directedClient))
                return NotFound($"user {directedClient} isn't registerd!");

            if (!onlineClients.Exists((c) => c.UserName == directedClient))
                return NotFound($"user {directedClient} is offline right now.");

            var client = onlineClients.Find((c) => c.UserName == directedClient);

            var reqParams = new Dictionary<string, string>
            {
                {"otherClient", User.Identity.Name }
            };

            this.mDal.Add(new Message(User.Identity.Name, directedClient, message));
            return Ok($"message sent to {directedClient}");

        }
        [HttpGet("getAllMessages")]
        public IActionResult getMessages(string directedClient)
        {
            var msgs = this.mDal.GetAll();

            var relaventMsgs = msgs.Where((m) =>
            (m.Sender == User.Identity.Name && m.Receiver == directedClient) ||
            (m.Sender == directedClient && m.Receiver == User.Identity.Name));

            var orderedMsgs = relaventMsgs.OrderBy((m) => m.Date).ToList();
            return Ok(orderedMsgs);
        }
    }
}
