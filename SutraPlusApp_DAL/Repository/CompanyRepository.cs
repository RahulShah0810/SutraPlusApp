using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class CompanyRepository : BaseRepository
    {
        public IConfiguration _configuration;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private MasterDBContext _masterDBContext;
        public CompanyRepository(int tenantID, MasterDBContext masterDBContext, IConfiguration configuration, ILogger logger) : base(tenantID, masterDBContext)
        {
            _logger = logger;
            _configuration = configuration;
            _masterDBContext = masterDBContext;
        }
        /// <summary>
        /// Get all company list 
        /// </summary>
        /// <returns></returns>
        public JObject List()
        {
            var response = new JObject();
            try
            {
                _logger.LogDebug("Company Repo : List Companies");
                var result = _tenantDBContext.Companies.Where(a => a.IsActive == true).ToList();
                if (result != null)
                {
                    response.Add("CompanyList", JArray.FromObject(result));
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

        public int GetMax()
        {
            try
            {
                return _tenantDBContext.Companies.Select(x => x.CompanyId).ToList().Max();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public bool Add(Company company)
        {
            try
            {
                company.CompanyId = GetMax() + 1;
                _tenantDBContext.Add(company);
                _tenantDBContext.SaveChanges();
                _logger.LogDebug("Company Repo : Add Company Successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public JObject Get(int companyId)
        {
            var response = new JObject();
            try
            {
                var result = _tenantDBContext.Companies.Where(a => a.CompanyId == companyId).ToList();
                if (result != null)
                {
                    response.Add("CompanyList", JArray.FromObject(result));
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
        
        public bool Update(JObject Data)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(Data["CompanyData"].ToString());
                if (data != null)
                {
                    int id = data["CompanyId"];
                    var entity = _tenantDBContext.Companies.FirstOrDefault(item => item.CompanyId == id);
                    if (entity != null)
                    {
                        entity.KannadaName = data["KannadaName"];
                        entity.Shree = data["Shree"];
                        entity.CompanyName = data["CompanyName"];
                        entity.Title = data["Title"];
                        entity.AddressLine1 = data["AddressLine1"];
                        entity.Pan = data["Pan"];
                        entity.Tan = data["Tan"];
                        entity.Place = data["Place"];
                        entity.Fln = data["Fln"];
                        entity.Bin = data["Bin"];
                        entity.District = data["District"];
                        entity.Email = data["Email"];
                        entity.State = data["State"];
                        entity.CellPhone = data["CellPhone"];
                        entity.ContactDetails = data["ContactDetails"];
                        entity.Gstin = data["Gstin"];
                        entity.FirmCode = data["FirmCode"];
                        entity.Apmccode = data["Apmccode"];
                        entity.Iec = data["Iec"];
                        entity.Bank1 = data["Bank1"];
                        entity.Ifsc1 = data["Ifsc1"];
                        entity.AccountNo1 = data["AccountNo1"];
                        entity.Bank2 = data["Bank2"];
                        entity.Ifsc2 = data["Ifsc2"];
                        entity.AccountNo2 = data["AccountNo2"];
                        entity.Bank3 = data["Bank3"];
                        entity.Ifsc3 = data["Ifsc3"];
                        entity.Account3 = data["Account3"];
                        entity.CreatedDate = DateTime.Now;
                        entity.Logo = data["Logo"];
                        entity.IsActive = true;
                        _tenantDBContext.SaveChanges();
                        _tenantDBContext.Update(entity);
                    }
                    _logger.LogDebug("Company Repo : Companies Updated");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public JObject Search(string searchvalue)
        {
            var response = new JObject();
            try
            {
                var result = (from c in _tenantDBContext.Companies
                              where (c.CompanyName.Contains(searchvalue) || c.Place.Contains(searchvalue) || c.Pan.Contains(searchvalue) || c.Email.Contains(searchvalue)
                              || c.Tan.Contains(searchvalue) || c.District.Contains(searchvalue) || c.CellPhone.Contains(searchvalue) || c.Gstin.Contains(searchvalue))
                              select new
                              {
                                  c._Id,
                                  c.KannadaName,
                                  c.Shree,
                                  c.CompanyName,
                                  c.AddressLine1,
                                  c.Pin,
                                  c.Tan,
                                  c.Apmccode,
                                  c.Place,
                                  c.Fln,
                                  c.Bin,
                                  c.District,
                                  c.Email,
                                  c.State,
                                  c.CellPhone,
                                  c.ContactDetails,
                                  c.Gstin,
                                  c.FirmCode,
                                  c.IsShopSent,
                                  c.Iec,
                                  c.IsActive
                              }).ToList();
                if (result != null)
                {
                    response.Add("SearchList", JArray.FromObject(result));
                    return response;
                }
                else
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}
