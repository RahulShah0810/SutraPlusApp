using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SutraPlusApp_DAL.Common;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SutraPlusApp_DAL.Repository
{
    public class SuperAdminSecurityRepository
    {
        private MasterDBContext _masterDBContext;
        private EmailSender _emailSender = null;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public SuperAdminSecurityRepository(MasterDBContext masterDB, IConfiguration _configuration, ILogger logger)
        {
            _logger = logger;
            _masterDBContext = masterDB;
            _emailSender = new EmailSender(_configuration);
        }
        public JObject Authenticate(string userEmail, string password)
        {
            var response = new JObject();
            try
            {
                _logger.LogDebug("Super Admin Login Successfully " + userEmail);
                var result = _masterDBContext.SuperAdminLogins.Where(a => a.UserName == userEmail
                && a.Password == password).FirstOrDefault();

                if (result != null)
                {
                    response.Add("UserEmailId", result.UserName);
                    response.Add("UserType", result.UserType);
                    response.Add("IsSuccess", true);
                    return response;
                }
                response.Add("UserEmailId", userEmail);
                response.Add("IsSuccess", false);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public JObject ForgotPassword(string Email)
        {
            var response = new JObject();
            try
            {
                _logger.LogDebug("Forgot Password");
                var result = _masterDBContext.SuperAdminLogins.Where(a => a.UserName == Email).FirstOrDefault();
                if (result != null)
                {
                    _emailSender.SendMailMessage(Email, "Your Password", "", result.Password);
                    _logger.LogDebug("Password sent over mail Successfully");
                    response.Add("UserName", Email);
                    response.Add("IsSuccess", true);
                    return response;
                }
                response.Add("IsSuccess", false);
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
