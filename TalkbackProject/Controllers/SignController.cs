using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Talkback.Common;
using Talkback.Server.Interfaces;

namespace Talkback.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private ITokenAuth auth;
        private IDal<Client> dal;
        private HttpHandler notifier;
        public SignController(ITokenAuth auth, IDal<Client> dal)
        {
            this.auth = auth;
            this.dal = dal;
            this.notifier = new HttpHandler();
        }
        [HttpGet("signin")]
        public IActionResult SignOn()
        {
            var clients = this.dal.GetAll();
            var onlineClients = clients.Where((c) => c.IsOnline == true);
            var client = this.dal.Get(User.Identity.Name);
            client.IsOnline = true;
            client.URL = Request.Host.ToString();
            this.dal.Update(client);
            
            // notify all online clients that the current user is online

            //var allClientsReceived = this.notifier.MakeHttpGetAll(onlineClients.Select((c) => c.URL), $"view/clientOnline?username={User.Identity.Name}");

            //if (allClientsReceived)
                 return Ok(clients);

           // return NotFound();

        }
        [HttpGet("signoff")]
        public IActionResult SignOff()
        {
            var onlineClients = this.dal.GetAll().Where((c) => c.IsOnline == true);
            this.dal.Update(User.Identity.Name, false);

            // notify all online clients that the current user is offline
            notifier.MakeHttpGetAll(onlineClients.Select((c) => c.URL), "view/clientOffline", new System.Collections.Generic.Dictionary<string, string>()
            {
                {"username",User.Identity.Name}
            });

            return Ok(onlineClients);

        }

        [AllowAnonymous]
        [HttpGet("getUsers")]
        public IActionResult GetSignedUsers()
        {
            return Ok(this.dal.GetAll());
        }
        [AllowAnonymous]
        [HttpPost("signup")]
        public IActionResult Authentication(string userName, string password)
        {
            if (userName == null || password == null)
                return BadRequest("no username or password");
            var token = auth.Authentication(userName, password);
            if (token == null)
                return Unauthorized();
            var clients = this.dal.GetAll().ToList();
            var client = clients.Find((c) => c.UserName == User.Identity.Name);
            if (client != null)
                return BadRequest("user already exists");

            this.dal.Add(new Client(userName, password, false, ""));
            return Ok(token);
        }
        [AllowAnonymous]
        [HttpGet("renewToken")]
        public IActionResult RenewToken(string userName, string password)
        {
            var token = auth.Authentication(userName, password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
    }
}
