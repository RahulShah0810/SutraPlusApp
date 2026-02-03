using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SutraPlusApp_BAL.Service;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NLog;
using System.Collections.Generic;
using System;
namespace SutraPlusApp.Controllers
{
    //[EnableCors("CorsPolicy")]
    [Produces("application/json")]
    [Route("api/Controller")]
    [ApiController]
    [Route("[controller]")]
    public class SuperAdminSecurityController : ControllerBase
    {
        private SuperAdminSecurityService _superAdminService;
        private MasterDBContext _masterDBContext;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private IConfiguration _configuration { get; }
        public SuperAdminSecurityController(MasterDBContext MasterDBContext, IConfiguration configuration, ILogger<SuperAdminSecurityController> logger)
        {
            _masterDBContext = MasterDBContext;
            _logger = logger;
            _configuration= configuration;
            _superAdminService = new SuperAdminSecurityService(_masterDBContext, _configuration, _logger);

        }
        //Super Admin Section using master database
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Authenticate([FromBody] JObject Data)
        {
            try
            {
                var result = _superAdminService.Authenticate(Data);
                var login = JsonConvert.DeserializeObject<dynamic>(Data["UserDetails"].ToString());
                var encodedJwt = this.GenerateToken(result["UserEmailId"].ToString(), "SuperAdmin");
                var response = new { token = encodedJwt, result = result, Status = true };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }


        private async Task<IActionResult> GenerateToken(string userName, string roleCode)
        {
            try
            {
                if (userName != null && roleCode != null)
                {
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserName", userName),
                        new Claim("UserRole", roleCode),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    _logger.LogError("User Not Found (Password/Email wrong");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// forgot password will send the existing password if E-mail is correct
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        [HttpPost("Forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] JObject Data)
        {
            try
            {
                var result = _superAdminService.ForgotPassword(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}
