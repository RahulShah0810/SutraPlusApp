using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SutraPlusApp.Utilities;
using SutraPlusApp_BAL.Service;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SutraPlusApp.Controllers
{
    [Route("Security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private SecurityService _securityService;
        private DBContext _DBContext;
        private MasterDBContext _masterDBContext;
        public SecurityController(DBContext DBContext, MasterDBContext MasterDBContext)
        {
            _masterDBContext = MasterDBContext;
            _DBContext = DBContext;
            _securityService = new SecurityService(_DBContext, _masterDBContext);
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] JObject responseData)
        {
            var result = _securityService.Authenticate(responseData);
            return Ok(result);
        }       
    }
}
