using System.Net.Mime;
using System.Threading.Tasks;
using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using Jellyfin.Plugin.JellyfinHeaderAuth.Helpers;
using Jellyfin.Plugin.JellyfinHeaderAuth.Models;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JellyfinHeaderAuth.Api
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, ISessionManager sessionManager, IUserManager userManager)
        {
            _sessionManager = sessionManager;
            _userManager = userManager;
            _logger = logger;
            _logger.LogInformation("Auth Controller initialized");
        }

        [HttpGet]
        public ActionResult GetAuthPage()
        {
            var requestBase = Request.Scheme + "://" + Request.Host + Request.PathBase;

            return Content(WebResponse.Generator(requestBase), MediaTypeNames.Text.Html);
        }

        [HttpPost]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> StartAuth([FromBody] AuthPayload payload)
        {
            if (Request.Headers.TryGetValue("X-Forwarded-UserName", out var headerUsername)) {
                var authenticationResult = await Authenticate(headerUsername, payload).ConfigureAwait(false);
                return Ok(authenticationResult);
            } else {
                return BadRequest("The UserName header was not found.");
            }
        }

        private async Task<AuthenticationResult> Authenticate(string username, AuthPayload authPayload)
        {
            User user = null;
            user = _userManager.GetUserByName(username);

            if (user == null)
            {
                _logger.LogInformation("Header user doesn't exist, creating...");
                user = await _userManager.CreateUserAsync(username).ConfigureAwait(false);
                user.SetPermission(PermissionKind.IsAdministrator, false);
                user.SetPermission(PermissionKind.EnableAllFolders, false);
            }

            user.AuthenticationProviderId = "Jellyfin.Server.Implementations.Users.DefaultAuthenticationProvider";
            await _userManager.UpdateUserAsync(user).ConfigureAwait(false);

            var authRequest = new AuthenticationRequest();
            authRequest.UserId = user.Id;
            authRequest.Username = user.Username;
            authRequest.App = authPayload.AppName;
            authRequest.AppVersion = authPayload.AppVersion;
            authRequest.DeviceId = authPayload.DeviceID;
            authRequest.DeviceName = authPayload.DeviceName;
            _logger.LogInformation("Auth request created...");

            return await _sessionManager.AuthenticateNewSession(authRequest).ConfigureAwait(false);
        }
    }
}
