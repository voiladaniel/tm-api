using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TravianManager.Core;
using TravianManager.Core.Data;
using TravianManager.Core.Managers;

namespace TravianManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManager _accountManager;
        private readonly ILogger _logger;
        private readonly IHelpers _helpers;

        public AccountController(IAccountManager accountManager, ILogger logger, IHelpers helpers)
        {
            _helpers = helpers;
            _accountManager = accountManager;
            _logger = logger;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] User user)
        {
            try
            {
                var (loginResult, loginUser) = await _accountManager.Login(user);

                if (!loginResult)
                    return Unauthorized();

                return Ok(new User {
                    Id = loginUser.Id,
                    Username = loginUser.Username
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: {user.Username}", ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Route("Encrypt")]
        public async Task<ActionResult> Encrypt(string password)
        {
            try
            {
                return Ok(_helpers.Encrypt(password));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while encrypting passowrd, for user: {password}", ex.Message);
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("UpdateDefender")]
        public async Task<ActionResult> UpdateDefender([FromBody] Defender defender)
        {
            try
            {
                var result = await _accountManager.UpdateDefender(defender);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("DeleteDefender")]
        public async Task<ActionResult> DeleteDefender([FromBody] Defender defender)
        {
            try
            {
                var result = await _accountManager.DeleteDefender(defender);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR ERROR ERROR ---> Error while trying to login, for user: ", ex.Message);
                return BadRequest(ex);
            }
        }
    }
}