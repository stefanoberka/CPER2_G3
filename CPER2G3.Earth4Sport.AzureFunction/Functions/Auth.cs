using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CPER2G3.Earth4Sport.AzureFunction.Models;
using CPER2G3.Earth4Sport.AzureFunction.JwtUtils;
using MongoDB.Driver;
using CPER2G3.Earth4Sport.AzureFunction.Service;
using Microsoft.VisualBasic;
using System.Web.Http;
using Microsoft.Extensions.Configuration;

namespace CPER2G3.Earth4Sport.AzureFunction.Functions {
    public class Auth {
        private IUserService _userService { get; set; }
        private JwtMethods _jwtMethods { get; set; }
        public Auth(IUserService userService, IConfiguration conf) {
            _userService = userService;
            _jwtMethods = new JwtMethods(conf);
        }
        [FunctionName("register")]
        public async Task<IActionResult> Insert(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log) {
            try {

                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body)) {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string clockUuid = data.uuid;
                User user = new User() {
                    Username = data.username,
                    Password = data.password,
                };
                var res = await _userService.Register(user, clockUuid);
                return new OkObjectResult(res);
            }
            catch (Exception ex) {
                log.LogError(ex.Message);
                return new InternalServerErrorResult();

            }
        }

        [FunctionName("login")]
        public async Task<IActionResult> Login(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log) {
            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body)) {
                requestBody = await streamReader.ReadToEndAsync();
            }
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            User user = new User() {
                Username = data.username,
                Password = data.password,
            };
            var res = await _userService.Login(user.Username, user.Password);
            if (res.Authorized) {
                string token = _jwtMethods.GenerateToken(res.Uuid);
                log.LogInformation(token);
                return new OkObjectResult(token);
            }
            else {
                return new UnauthorizedObjectResult("Username e/o Password non validi!");
            }

        }
    }
}
