using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;
using SutraPlusApp_DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutraPlusApp_BAL.Service
{
    public class CommonService
    {
        public CommonRepository _commonRepository = null;
        private MasterDBContext _masterDBContext;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public CommonService(MasterDBContext masterDB, ILogger logger)
        {
            _logger = logger;   
            _commonRepository = new CommonRepository(masterDB, _logger);
        }
        public JObject GetStates()
        {
            var result = new JObject();
            try
            {
                _logger.LogDebug("inside getstatus service");
                 result = _commonRepository.GetStates();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
            return result;
        }
        //TODO: Change per GetStates (Done)
        public JObject GetFinancialYears()
        {
            var result = new JObject();
            try
            {
                result = _commonRepository.GetFinancialYears();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
            return result;
        }
        //TODO: Change per GetStates (Done)
        public JObject GetCounties()
        {
            var result = new JObject();
            try
            {
                result = _commonRepository.GetCounties();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
            return result;
        }
    }
}
