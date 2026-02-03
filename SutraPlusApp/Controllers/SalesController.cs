using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SutraPlusApp_BAL.Service;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;

namespace SutraPlusApp.Controllers
{
    [Route("Sales")]
    [ApiController]
    public class SalesController : CommonTenantController
    {
        private SalesService _salesService;
        private MasterDBContext _masterDBContext;
        public IConfiguration _configuration;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public SalesController(MasterDBContext MasterDBContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<SalesController> logger) : base(httpContextAccessor)
        {
            _masterDBContext = MasterDBContext;
            _configuration = configuration;
            _logger = logger;
            _salesService = new SalesService(Convert.ToInt32(base.tenantId), _masterDBContext, _configuration, _logger);

        }
        /// <summary>
        /// Get Voucher Type & next Invoice No (max+1)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost("Get")]
        [AllowAnonymous]
        public async Task<ActionResult> Get([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.Get(Data);
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Get Commodity Items with name & gst type (same invoice should have same gst rate items 
        /// this api will return only those items having same gst rate (pass name & gstrate)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost("GetList")]
        [AllowAnonymous]
        public IActionResult GetItem([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.GetItem(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Add Invoice Data (Invoice Data + Items Data + Lorry Details) based on Invoice Type
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [AllowAnonymous]
        public IActionResult AddInvoice([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.AddInvoice(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }

        }
        /// <summary>
        /// Get single Invoice Data (Invoice Data + Items Data + Lorry Details) based on Invoice Type
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost("Getsingle")]
        [AllowAnonymous]
        public IActionResult GetsingleList([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.GetsingleList(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Get all invoice list based on company id & invoice type
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [HttpPost("GetAll")]
        [AllowAnonymous]
        public async Task<ActionResult> GetList([FromBody] JObject Data)
        {
            try
            {
                 var result = _salesService.GetList(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        //TODO Panding for UI logic as per required chenges
        [HttpPost("Update")]
        [AllowAnonymous]
        public IActionResult Update([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.Update(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }
        //TODO after confirmation use this with logic check
        [HttpPost("Delete")]
        [AllowAnonymous]
        public async Task<ActionResult>Delete([FromBody] JObject Data)
        {
            try
            {
                var result = _salesService.Delete(Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }


    }
}