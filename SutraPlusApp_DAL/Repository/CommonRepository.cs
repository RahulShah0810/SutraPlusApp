using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutraPlusApp_DAL.Repository
{
    public class CommonRepository
    {
        private MasterDBContext _masterDBContext;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public CommonRepository(MasterDBContext masterDBContext, ILogger logger)
        {
            this._masterDBContext = masterDBContext;
            _logger = logger;
        }
        public JObject GetStates()
        {
            var response = new JObject();
            try
            {
                _logger.LogDebug("inside getstatus repository");
                var result = (from s in _masterDBContext.StateMaster
                              where s.IsActive == true
                              select new { s.Id, s.Statecode, s.Statename }).OrderBy(s => s.Statename).ToList();
                if (result != null)
                {
                    response.Add("StateList", JArray.FromObject(result));
                    return response;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }

        //TODO: Change this method as per GetStates (done)
        public JObject GetFinancialYears()
        {
            var response = new JObject();
            try
            {
                var result = (from s in _masterDBContext.YearMaster
                              where s.IsActive == true
                              select new { s.Id, s.Year, s.FinYear }).OrderBy(s => s.Year).ToList();
                if (result != null)
                {
                    response.Add("FinancialYear", JArray.FromObject(result));
                    return response;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }

        //TODO:: Change this method as per GetStates (done)
        public JObject GetCounties()
        {
            var response = new JObject();
            //var context = new MasterDBContext();
            try
            {
                var result = (from c in _masterDBContext.Countries
                              where c.IsActive == true
                              select new { c._Id, c.CountryName }).ToList().DistinctBy(c => new { c.CountryName });

                if (result != null)
                {
                    response.Add("CountryDDList", JArray.FromObject(result));
                    return response;
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}

