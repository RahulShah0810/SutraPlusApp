using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SutraPlus.Models;
using SutraPlusApp_DAL.Common;
using SutraPlusApp_DAL.Models;
using SutraPlusApp_DAL.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutraPlusApp_BAL.Service
{
    public class SalesService
    {
        public SalesRepository _salesRepository = null;
        private MasterDBContext _masterDBContext;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public IConfiguration _configuration { get; }
        private EmailSender _emailSender = null;
        public SalesService(int tenantID, MasterDBContext masterDBContext, IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _masterDBContext = masterDBContext;
            _logger = logger;
            _configuration = configuration;
            _emailSender = new EmailSender(_configuration);
            _salesRepository = new SalesRepository(tenantID, masterDBContext, _configuration, _logger);
        }
        public JObject Get(JObject Data)
        {
            try
            {
                var response = new JObject();
                var data = JsonConvert.DeserializeObject<dynamic>(Data["SalesDetails"].ToString());
                int CompanyId = data["CompanyId"];
                int LedgerId = data["LedgerId"];
                string DealerType = data["DealerType"];
                response = _salesRepository.Get(CompanyId, LedgerId, DealerType);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public JObject GetItem(JObject Data)
        {
            try
            {
                var response = new JObject();
                var data = JsonConvert.DeserializeObject<dynamic>(Data["SelesItem"].ToString());
                string name = data["Name"];
                string GST_type = data["GSTType"];
                response = _salesRepository.GetItem(name, GST_type);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            }
        }
        public string AddInvoice(JObject Data)
        {
            try
            {               
                return _salesRepository.AddInvoice(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
       
        public JObject GetsingleList(JObject Data)
        {
            try
            {
                return _salesRepository.GetsingleList(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
      
        public pagination<BillSummary> GetList(JObject Data)
        {
            try
            {
                return _salesRepository.GetList(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public Boolean Update(JObject Data)
        {
            try
            {
                return _salesRepository.Update(Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public bool Delete(JObject Data)
        {
            try
            {
                _salesRepository.Delete(Data);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}
