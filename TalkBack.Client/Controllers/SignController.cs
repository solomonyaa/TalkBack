using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Talkback.Common;

namespace Talkback.Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private HttpHandler notifier;
        private string serverURL;
        private ClientState state;
        public SignController(Config config, [FromServices]ClientState state)
        {
            this.serverURL = config.ServerUrl;
            this.notifier = new HttpHandler();
            this.state = state;
        }
        [HttpGet("signup")]
        public IActionResult SignUp(string userName, string password)
        {
            var reqParams = new Dictionary<string, string>
            {
                {"username", userName },
                {"password",password }
            };
             var tokenString = notifier.MakeHttpPost(serverURL, "sign/signup", reqParams).Content.ReadAsStringAsync().Result;

            state.token.setToken(tokenString);
            var signedUsersResp = notifier.MakeHttpGet(serverURL, "sign/getusers");
            if (signedUsersResp.IsSuccessStatusCode)
            {
                var s = signedUsersResp.Content.ReadAsStringAsync().Result;
                var ClientArr = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Common.Client>>(s);
                return Ok(ClientArr);
            }
            return BadRequest(new List<Common.Client>());

        }

        [HttpGet("signin")]
        public IActionResult SignOn()
        {
            var onlineClientResp = notifier.MakeHttpGet(serverURL, "sign/signin", null, token: state.token.getToken());
            var s = onlineClientResp.Content.ReadAsStringAsync().Result;
            IEnumerable<Common.Client> ClientArr = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Common.Client>>(s);
            return Ok(ClientArr);
        }

        [HttpGet("signoff")]
        public IActionResult SignOff()
        {
           _ = notifier.MakeHttpGet(serverURL, "sign/signoff", null, token: state.token.getToken());;

            return Ok();
        }
        [HttpGet("exist")]
        public IActionResult Exists(string username, string password)
        {
            if (username == null || password == null)
                return BadRequest("no username or password");

            if (!state.token.isValid())
                this.state.token.renewToken(username, password);
            var regUsers = notifier.MakeHttpGet(serverURL,"sign/getusers");
            if(regUsers.IsSuccessStatusCode )
            {
                var s = regUsers.Content.ReadAsStringAsync().Result;
                var ClientArr = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Common.Client>>(s);
                return Ok(ClientArr.ToList().Exists((c) => c.UserName == username));
            }
            return Ok(false);

        }
        [HttpGet("renewToken")]
        public IActionResult GetToken(string userName, string password)
        {
            this.state.token.renewToken(userName, password);
           
            return Ok("Token updated Successfully");
        }
        [HttpGet("clientOnline")]
        public IActionResult ClientConnected(string username)
        {
            state.SetClientState(username, true);
            return Ok();
        }
        [HttpGet("clientOffline")]
        public IActionResult ClientDiConnected(string username)
        {
            state.SetClientState(username, false);
            return Ok();
        }
        [HttpGet("updateClientsView")]
        public IActionResult UpdateView()
        {
            return Ok(this.state.GetOnlineClients());
        }
    }
}
