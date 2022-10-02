﻿using Microsoft.AspNetCore.Mvc;

using jcGAI.WebAPI.Controllers.Base;
using jcGAI.WebAPI.Objects.Json;
using jcGAI.WebAPI.Objects.NonRelational;
using jcGAI.WebAPI.Services;

using jcGAI.WebAPI.Common;

namespace jcGAI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/account")]
    public class AccountController : BaseController
    {
        public AccountController(ILogger<AccountController> logger, MongoDbService mongo) : base(logger, mongo)
        {
        }

        [HttpGet]
        public ActionResult<string> Login(string username, string password)
        {
            var existingUser = Mongo.GetOne<Users>(a => a.Username == username && a.Password == password.ToSHA256());

            if (existingUser == null)
            {
                return BadRequest("Invalid username or password");
            }

            return string.Empty;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser(UserRequestItem userRequestItem)
        {
            var existingUser = Mongo.GetOne<Users>(a => a.Username == userRequestItem.Username);

            if (existingUser != null)
            {
                return BadRequest($"Existing username ({userRequestItem.Username}) was found");
            }

            var result = await Mongo.InsertUserAsync(new Users
            {
                Username = userRequestItem.Username,
                Password = userRequestItem.Password.ToSHA256()
            });

            return result != Guid.Empty;
        }
    }
}