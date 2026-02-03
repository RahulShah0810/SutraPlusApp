using Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.HadrModel;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SutraPlus.Models;
using SutraPlusApp_DAL.Common;
using SutraPlusApp_DAL.Data;
using SutraPlusApp_DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;


namespace SutraPlusApp_DAL.Repository
{
    public class SalesRepository : BaseRepository
    {
        public IConfiguration _configuration;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private MasterDBContext _masterDBContext;

        public SalesRepository(int tenantID, MasterDBContext masterDBContext, IConfiguration configuration, ILogger logger) : base(tenantID, masterDBContext)
        {
            _logger = logger;
            _configuration = configuration;
            _masterDBContext = masterDBContext;
        }
        public long GetMaxInvoice()
        {
            //var result = _tenantDBContext.BillSummaries.Max(i => i._Id);
            //return _tenantDBContext.BillSummaries.Select(x => x.VochNo).ToList().Max()+1 ;
            return 0;
        }
        /// <summary>
        /// get Invoice NO for three parameter
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="LedgerId"></param>
        /// <param name="DealerType"></param>
        /// <returns></returns>
        public JObject Get(int CompanyId, int LedgerId, string DealerType)
        {
            var response = new JObject();
            try
            {
                var companyState = _tenantDBContext.Companies.Where(c => c.CompanyId == CompanyId).Select(c => c.State).SingleOrDefault();
                var ledgerState = _tenantDBContext.Ledgers.Where(l => l.LedgerId == LedgerId && l.CompanyId == CompanyId && l.DealerType == DealerType).Select(l => l.State).SingleOrDefault();
                var dealer_type = _tenantDBContext.Ledgers.Where(l => l.LedgerId == LedgerId && l.CompanyId == CompanyId).Select(l => l.DealerType).SingleOrDefault();
                var country_name = _tenantDBContext.Ledgers.Where(l => l.LedgerId == LedgerId && l.CompanyId == CompanyId).Select(l => l.Country).SingleOrDefault();
                if (companyState == ledgerState && dealer_type == "Registered Dealer" && country_name == "India")
                {
                    var voucherId = _tenantDBContext.VoucherTypes.Where(v => v.VoucherName == "Local Sale").Select(x => x.VoucherId).SingleOrDefault();
                    response.Add("VoucherType", new JValue("Local Sale"));
                    response.Add("InvoiceNo", new JValue(Convert.ToUInt32(_tenantDBContext.BillSummaries.Select(x => x.VochNo).ToList().Max()) + 1));
                    response.Add("VoucherId", new JValue(Convert.ToInt64(voucherId)));
                }
                else if (companyState != ledgerState && dealer_type == "Registered Dealer" && country_name == "India")
                {
                    var voucherId = _tenantDBContext.VoucherTypes.Where(v => v.VoucherName == "Interstate Sale").Select(x => x.VoucherId).SingleOrDefault();
                    response.Add("VoucherType", new JValue("Interstate Sale"));
                    response.Add("InvoiceNo", new JValue(Convert.ToUInt32(_tenantDBContext.BillSummaries.Select(x => x.VochNo).ToList().Max()) + 1));
                    response.Add("VoucherId", new JValue(Convert.ToInt64(voucherId)));
                }
                else if (dealer_type != "Register Dealer" && country_name == "India")
                {
                    var voucherId = _tenantDBContext.VoucherTypes.Where(v => v.VoucherName == "URD Sale").Select(x => x.VoucherId).SingleOrDefault();
                    response.Add("VoucherType", new JValue("URD Sale"));
                    response.Add("InvoiceNo", new JValue(Convert.ToUInt32(_tenantDBContext.BillSummaries.Select(x => x.VochNo).ToList().Max()) + 1));
                    response.Add("VoucherId", new JValue(Convert.ToInt64(voucherId)));
                }
                else if (country_name != "India")
                {
                    var voucherId = _tenantDBContext.VoucherTypes.Where(v => v.VoucherName == "Export Sale").Select(x => x.VoucherId).SingleOrDefault();
                    response.Add("VoucherType", new JValue("Export Sale"));
                    response.Add("InvoiceNo", new JValue(Convert.ToUInt32(_tenantDBContext.BillSummaries.Select(x => x.VochNo).ToList().Max()) + 1));
                    response.Add("VoucherId", new JValue(Convert.ToInt64(voucherId)));
                }
                else
                {
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
        /// <summary>
        /// GetItem by Commodity table
        /// </summary>
        /// <param name="name"></param>
        /// <param name="GST_type"></param>
        /// <returns></returns>
        public JObject GetItem(string name, string GST_type)
        {
            try
            {
                var response = new JObject();
                if (string.IsNullOrEmpty(GST_type))
                {
                    var result = (from c in _tenantDBContext.Commodities
                                  where c.CommodityName.Contains(name) && c.IsActive == true
                                  select new
                                  {
                                      c.CommodityId,
                                      c.CommodityName,
                                      c.IGST,
                                      c.SGST,
                                      c.CGST
                                  }).ToList();
                    response.Add("Commodities", JArray.FromObject(result));

                }
                else
                {
                    var result = (from c in _tenantDBContext.Commodities
                                  where c.SGST == Convert.ToDecimal(GST_type) || c.CGST == Convert.ToDecimal(GST_type) || c.IGST == Convert.ToDecimal(GST_type) && c.IsActive == true
                                  select new
                                  {
                                      c.CommodityId,
                                      c.CommodityName,
                                      c.IGST,
                                      c.SGST,
                                      c.CGST
                                  }).ToList();
                    response.Add("Commodities", JArray.FromObject(result));
                }
                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex; throw;
            }
        }

        /// <summary>
        /// NumberToWords(), this method is convert Digit into words number and method   
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// 
        public string AmountToWord(double number)
        {
            if (number == 0) return "Zero";

            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";

            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };

            string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };

            string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };

            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            num[0] = (int)number % 1000; // units
            num[1] = (int)number / 1000;
            num[2] = (int)number / 100000;
            num[1] = (int)num[1] - 100 * num[2]; // thousands
            num[3] = (int)number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;

                u = (int)num[i] % 10; // ones
                t = (int)num[i] / 10;
                h = (int)num[i] / 100; // hundreds
                t = t - 10 * h; // tens

                if (h > 0) sb.Append(words0[h] + "Hundred ");

                if (u > 0 || t > 0)
                {
                    if (h == 0)
                        sb.Append("");
                    else
                        if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd() + " Rupees only";
        }
        /// <summary>
        /// Add sales  for the Invoicedat ,LorryData And Item data 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public string AddInvoice(JObject Data)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(Data["InvioceData"].ToString());
                var LorryData = JsonConvert.DeserializeObject<dynamic>(Data["LorryDetails"].ToString());
                var itemdata = JsonConvert.DeserializeObject<dynamic>(Data["ItemData"].ToString());
                var invType = data["InvoiceType"];
                string TotalAmount = data["TotalAmount"];
                int companyid = data["CompanyId"];
                int invoiceNo = data["InoviceNo"];
                int ledgerid = data["LedgerId"];
               
                var ledgerName = (_tenantDBContext.Ledgers.
                                   Where(l => l.LedgerId == ledgerid).Select(l => l.LedgerName)).FirstOrDefault();
                var ledgerplace = (_tenantDBContext.Ledgers.
                                   Where(l => l.LedgerId == ledgerid && l.LedgerName == ledgerName).Select(l => l.Place)).FirstOrDefault();

                var InvoiceNo = _tenantDBContext.BillSummaries.Where(b => b.CompanyId == companyid && b.VochNo == invoiceNo).ToList();
                if (InvoiceNo.Count > 0)
                {
                    return "Duplicate InoviceNo...!";
                }
                else
                {
                    BillSummary billsummary = new BillSummary
                    {
                        CompanyId = data["CompanyId"], //selling party
                        LedgerId = data["LedgerId"],//party id
                        LedgerName = ledgerName + "-" + ledgerplace,
                        OriginalInvDate = data["OriginalInvDate"],
                        // CommodityID = itemdata["CommodityId"],
                        VochType = data["VochType"],
                        VoucherName = data["VoucherName"],
                        DealerType = data["DealerType"],
                        PAN = data["PAN"],
                        GST = data["GST"],
                        InvoiceType = invType,
                        VochNo = data["InoviceNo"],
                        State = data["State"],
                        //other charges block
                        ExpenseName1 = data["ExpenseName1"],
                        ExpenseName2 = data["ExpenseName2"],
                        ExpenseName3 = data["ExpenseName3"],
                        ExpenseAmount1 = data["ExpenseAmount1"],
                        ExpenseAmount2 = data["ExpenseAmount2"],
                        ExpenseAmount3 = data["ExpenseAmount3"],
                        //paymentDetails
                        TaxableValue = data["TaxableValue"],
                        Discount = data["Discount"],
                        SGSTValue = data["Sgstvalue"],
                        CSGSTValue = data["Csgstvalue"],
                        IGSTValue = data["Igstvalue"],
                        IsSEZ = data["IsSEZ"],
                        RoundOff = data["RoundOff"],
                        TotalAmount = data["TotalAmount"],
                        INWords = AmountToWord(Convert.ToDouble(TotalAmount)),// call amt here (write method to which pass amt & get return amt in words like Ruppes One Hundred & Fifty Five Only)                                                                     
                        DispatcherAddress1 = data["Address"],
                        FromPlace = data["FromPlace"],
                        ToPlace = data["ToPlace"],
                        //Lorray details
                        Ponumber = LorryData["PoNumber"],
                        EwayBillNo = LorryData["EwaybillNo"],
                        Transporter = LorryData["Transporter"],
                        LorryNo = LorryData["LorryNo"],
                        LorryOwnerName = LorryData["Owner"],
                        DriverName = LorryData["Driver"],
                        Dlno = LorryData["Dlno"],
                        CheckPost = LorryData["CheckPost"],
                        FrieghtPerBag = LorryData["FrieghtPerBag"],
                        TotalFrieght = LorryData["TotalFrieght"],
                        Advance = LorryData["AdvanceFrieght"],
                        Balance = LorryData["BalanceFrieght"],
                        IsLessOrPlus = LorryData["FrieghtPlus_Less"],//FrieghtPlus/Less field
                        tdsperc = LorryData["TDS"],
                        //  Dilivary Address Details                
                        DeliveryName = LorryData["DeliveryName"],
                        DeliveryAddress1 = LorryData["DeliveryAddress1"],
                        DeliveryAddress2 = LorryData["DeliveryAddress2"],
                        DeliveryPlace = LorryData["DeliveryPlace"],
                        DelPinCode = LorryData["DeliveryPin"],
                        DeliveryState = LorryData["DeliveryState"],
                        DeliveryStateCode = LorryData["DeliveryStateCode"],
                        Distance = LorryData["Distance"],
                        DCNote = LorryData["Dcnote"],
                        IsActive = true
                    };
                    //Item Details
                    //add List of Itemdata
                    List<Inventory> inventorylist = new List<Inventory>();
                   
                    foreach (var item in itemdata)
                    {
                        string commodityid = item["CommodityId"];

                        var commodityname = _tenantDBContext.Commodities
                        .Where(c => c.CommodityId == Int32.Parse(commodityid)).Select(c => c.CommodityName).FirstOrDefault();
                        Inventory inventory = new Inventory()
                        {
                            CompanyId = data["CompanyId"],
                            VochNo = data["InoviceNo"],
                            VochType = data["VochType"],
                            InvoiceType = invType,
                            LedgerId = data["LedgerId"],
                            CommodityId = item["CommodityId"],
                            CommodityName = commodityname,
                            TranctDate = data["OriginalInvDate"], //from data
                            PoNumber = LorryData["PoNumber"], //from lorry
                            EwaybillNo = LorryData["EwaybillNo"], //from lorry
                            WeightPerBag = item["WeightPerBag"],//1
                            NoOfBags = item["NoOfBags"],//2
                            TotalWeight = item["TotalWeight"],//3
                            Rate = item["Rate"],//4
                            Amount = item["Amount"],//5
                            Mark = item["Remark"],//6
                            Discount = data["Discount"],
                            NetAmount = data["TotalAmount"],
                            SGST = item["SgstAmount"],
                            CGST = item["CsgstAmount"],
                            IGST = item["IgstAmount"],
                            FreeQty = item["NoOfDocra"], //for item qty in table no field so this field is used
                            CreatedDate = DateTime.Now,
                            //FreeQty = item["FreeQty"],
                            IGSTRate = data["Igstvalue"],
                            SGSTRate = data["Sgstvalue"],
                            CGSTRate = data["Csgstvalue"],
                            Taxable = data["TaxableValue"],
                            IsActive = true
                        };
                        inventorylist.Add(inventory);
                    }
                    _tenantDBContext.Add(billsummary);
                    _tenantDBContext.SaveChanges();
                    _tenantDBContext.AddRange(inventorylist);
                    _tenantDBContext.SaveChanges();
                    _logger.LogDebug("salesRepo : InvoiceDetails Added");
                    return "Added Successfully...!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }

        public JObject GetsingleList(JObject Data)
        {
            var response = new JObject();
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(Data["SalesInvoice"].ToString());
                int companyid_Invoice = data["CompanyId"];
                int ledgerid_Invoice = data["LedgerId"];
                int vochNo_Invoice = data["InvoiceNO"];
                int vochtype_Invoice = data["VouchType"];
                string invoice_Type = data["InvoiceType"];
                //var LedgerName = _tenantDBContext.Ledgers
                //    .Where(l=>l.LedgerId== ledgerid_Invoice).Select(l=>l.LedgerName).FirstOrDefault();  

                var billresult = _tenantDBContext.BillSummaries
             .Where(x => x.CompanyId == companyid_Invoice && x.LedgerId == ledgerid_Invoice && x.VochNo == vochNo_Invoice && x.VochType == vochtype_Invoice && x.InvoiceType.Contains(invoice_Type))
              .Select(sale => new
              {
                  sale._Id,
                  sale.CompanyId,
                  sale.LedgerId,
                  sale.LedgerName,
                  sale.OriginalInvDate,
                  sale.VochType,
                  sale.VoucherName,
                  sale.VochNo,
                  sale.DealerType,
                  sale.PAN,
                  sale.GST,
                  sale.InvoiceType,
                  sale.EInvoiceNo,
                  sale.State,
                  sale.ExpenseName1,
                  sale.ExpenseName2,
                  sale.ExpenseName3,
                  sale.ExpenseAmount1,
                  sale.ExpenseAmount2,
                  sale.ExpenseAmount3,
                  sale.TaxableValue,
                  sale.Discount,
                  sale.SGSTValue,
                  sale.CSGSTValue,
                  sale.IGSTValue,
                  sale.IsSEZ,
                  sale.RoundOff,
                  sale.TotalAmount,
                  //address ??
                  sale.FromPlace,
                  sale.ToPlace,
                  //lorry details
                  sale.Ponumber,
                  sale.EwayBillNo,
                  sale.Transporter,
                  sale.LorryNo,
                  sale.LorryOwnerName,
                  sale.DriverName,
                  sale.Dlno,
                  sale.CheckPost,
                  sale.FrieghtPerBag,
                  sale.TotalFrieght,
                  sale.Advance,
                  sale.Balance,
                  sale.IsLessOrPlus,
                  sale.tdsperc,
                  sale.DeliveryName,
                  sale.DeliveryAddress1,
                  sale.DeliveryAddress2,
                  sale.DeliveryPlace,
                  sale.DelPinCode,
                  sale.DeliveryState,
                  sale.DeliveryStateCode,
                  sale.Distance,
                  sale.DCNote,
                  sale.IsActive
              }).ToList();
                var Itemresult = _tenantDBContext.Inventory
                    .Where(x => x.CompanyId == companyid_Invoice && x.LedgerId == ledgerid_Invoice && x.VochNo == vochNo_Invoice && x.VochType == vochtype_Invoice)
                     .Select(item => new
                     {
                         item.Id,
                         item.CompanyId,
                         item.VochType,
                         item.InvoiceType,
                         item.VochNo,
                         item.LedgerId,
                         item.CommodityId,
                         item.CommodityName,
                         item.TranctDate,
                         item.WeightPerBag,
                         item.NoOfBags,
                         item.TotalWeight,
                         item.Rate,
                         item.Amount,
                         item.Mark,
                         item.NetAmount,
                         item.SGST,
                         item.CGST,
                         item.IGST,
                         item.CreatedDate,
                         item.FreeQty,
                         //item.NoOfDocra,
                         item.IGSTRate,
                         item.CGSTRate,
                         item.SGSTRate,
                         item.Taxable,
                         item.IsActive
                     }).ToList();

                if (billresult != null && Itemresult != null)
                {
                    response.Add("InvoiceData", JArray.FromObject(billresult));
                    response.Add("ItemData", JArray.FromObject(Itemresult));
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
        /// <summary>
        /// Get by add list by search by LesgerId  filed
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public pagination<BillSummary> GetList(JObject Data)
        {
            try
            {
                var searchText = Convert.ToString(Data["SearchText"]);
                var page = JsonConvert.DeserializeObject<pagination<BillSummary>>(Convert.ToString(Data["Page"]));
                var data = JsonConvert.DeserializeObject<dynamic>(Data["InvoiceData"].ToString());
                int companyId = data["CompanyId"];
                string invoiceType = data["InvoiceType"];
                IEnumerable<BillSummary> objList1 = null;
                var list = new List<BillSummary>();

                if (searchText != null && searchText != string.Empty)
                {
                    // string ledgername = searchText;
                    objList1 = _tenantDBContext.BillSummaries.Where(e => e.LedgerName.Contains(searchText) && e.IsActive == true && e.InvoiceType.Contains(invoiceType)).OrderBy(i => i.OriginalInvDate).ToList();
                    page.TotalCount = objList1.Count();
                }
                else
                {
                    objList1 = this._tenantDBContext.BillSummaries.Where(e => e.CompanyId == companyId && e.InvoiceType == invoiceType && e.IsActive == true).OrderBy(i=>i.VochNo).ToList();
                    page.TotalCount = objList1.Count();
                }

                foreach (var obj in objList1.ToList())
                {
                    var salesInvoice = new BillSummary();
                    salesInvoice._Id = obj._Id;
                    salesInvoice.CompanyId = obj.CompanyId;
                    salesInvoice.LedgerId = obj.LedgerId;
                    salesInvoice.LedgerName = obj.LedgerName;
                    salesInvoice.OriginalInvDate = obj.OriginalInvDate;
                    salesInvoice.VochType = obj.VochType;
                    salesInvoice.VoucherName = obj.VoucherName;
                    salesInvoice.VochNo = obj.VochNo;
                    salesInvoice.DealerType = obj.DealerType;
                    salesInvoice.PAN = obj.PAN;
                    salesInvoice.GST = obj.GST;
                    salesInvoice.InvoiceType = obj.InvoiceType;
                    salesInvoice.State = obj.State;
                    salesInvoice.ExpenseName1 = obj.ExpenseName1;
                    salesInvoice.ExpenseName2 = obj.ExpenseName2;
                    salesInvoice.ExpenseName3 = obj.ExpenseName3;
                    salesInvoice.ExpenseAmount1 = obj.ExpenseAmount1;
                    salesInvoice.ExpenseAmount2 = obj.ExpenseAmount2;
                    salesInvoice.ExpenseAmount3 = obj.ExpenseAmount3;
                    salesInvoice.TaxableValue = obj.TaxableValue;
                    salesInvoice.Discount = obj.Discount;
                    salesInvoice.SGSTValue = obj.SGSTValue;
                    salesInvoice.CSGSTValue = obj.CSGSTValue;
                    salesInvoice.IGSTValue = obj.IGSTValue;
                    salesInvoice.IsSEZ = obj.IsSEZ;
                    salesInvoice.RoundOff = obj.RoundOff;
                    salesInvoice.TotalAmount = obj.TotalAmount;
                    salesInvoice.FromPlace = obj.FromPlace;
                    salesInvoice.ToPlace = obj.ToPlace;
                    salesInvoice.Ponumber = obj.Ponumber;
                    salesInvoice.EwayBillNo = obj.EwayBillNo;
                    salesInvoice.Transporter = obj.Transporter;
                    salesInvoice.LorryNo = obj.LorryNo;
                    salesInvoice.LorryOwnerName = obj.LorryOwnerName;
                    salesInvoice.DriverName = obj.DriverName;
                    salesInvoice.Dlno = obj.Dlno;
                    salesInvoice.CheckPost = obj.CheckPost;
                    salesInvoice.FrieghtPerBag = obj.FrieghtPerBag;
                    salesInvoice.TotalFrieght = obj.TotalFrieght;
                    salesInvoice.Advance = obj.Advance;
                    salesInvoice.Balance = obj.Balance;
                    salesInvoice.IsLessOrPlus = obj.IsLessOrPlus;
                    salesInvoice.tdsperc = obj.tdsperc;
                    salesInvoice.DeliveryName = obj.DeliveryName;
                    salesInvoice.DeliveryAddress1 = obj.DeliveryAddress1;
                    salesInvoice.DeliveryAddress2 = obj.DeliveryAddress2;
                    salesInvoice.DeliveryPlace = obj.DeliveryPlace;
                    salesInvoice.DelPinCode = obj.DelPinCode;
                    salesInvoice.DeliveryState = obj.DeliveryState;
                    salesInvoice.DispatcherStatecode = obj.DispatcherStatecode;
                    salesInvoice.Distance = obj.Distance;
                    salesInvoice.DCNote = obj.DCNote;
                    salesInvoice.DispatcherAddress1 = obj.DispatcherAddress1;
                    salesInvoice.IsActive = obj.IsActive;
                    list.Add(salesInvoice);
                }
                page.Records = list.OrderByDescending(s => s.OriginalInvDate).Skip((page.PageNumber - 1) * page.PageSize).Take(page.PageSize).ToList();
                if (!string.IsNullOrEmpty(searchText))
                {
                    page.Records = list.OrderByDescending(s => s.OriginalInvDate).Skip((page.PageNumber - 1) * page.PageSize).Take(page.PageSize).ToList();
                }
                else
                {
                    page.Records = list.OrderByDescending(s => s.OriginalInvDate).Skip((page.PageNumber - 1) * page.PageSize).Take(page.PageSize).ToList();
                }
                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
        public bool Update(JObject Data) //TODO Panding for UI logic as per required chenges
        {
            try
            {
                var data = JsonConvert.DeserializeObject<dynamic>(Data["InvoiceData"].ToString());
                var LorryData = JsonConvert.DeserializeObject<dynamic>(Data["LorryDetails"].ToString());
                var itemdata = JsonConvert.DeserializeObject<dynamic>(Data["ItemData"].ToString());
                string Inword = data["TotalAmount"];
                if (data != null)
                {

                    int id = data["_Id"];
                    var entity = _tenantDBContext.BillSummaries.FirstOrDefault(item => item._Id == id);
                    if (entity != null)
                    {
                        entity.CompanyId = data["CompanyId"];
                        entity.LedgerId = data["LedgerId"];
                        entity.OriginalInvDate = data["OriginalInvDate"];
                        entity.VochType = data["VochType"];
                        entity.DealerType = data["DealerType"];
                        entity.PAN = data["PAN"];
                        entity.GST = data["GST"];
                        entity.InvoiceType = data["InvoiceType"];
                        entity.VochNo = data["InoviceNo"];
                        entity.State = data["State"];
                        //other charges block
                        entity.ExpenseName1 = data["ExpenseName1"];
                        entity.ExpenseName2 = data["ExpenseName2"];
                        entity.ExpenseName3 = data["ExpenseName3"];
                        entity.ExpenseAmount1 = data["ExpenseAmount1"];
                        entity.ExpenseAmount2 = data["ExpenseAmount2"];
                        entity.ExpenseAmount3 = data["ExpenseAmount3"];
                        //paymentDetails
                        entity.TaxableValue = data["TaxableValue"];
                        entity.Discount = data["Discount"];
                        entity.SGSTValue = data["Sgstvalue"];
                        entity.CSGSTValue = data["Csgstvalue"];
                        entity.IGSTValue = data["Igstvalue"];
                        entity.IsSEZ = data["IsSEZ"];
                        entity.RoundOff = data["RoundOff"];
                        entity.TotalAmount = Int32.Parse(Inword);
                        entity.INWords = AmountToWord(Convert.ToDouble(Inword));
                        //entity.INWords = TODO : amount in words
                        //address block 
                        entity.DispatcherAddress1 = data["Address"];
                        entity.FromPlace = data["FromPlace"];
                        entity.ToPlace = data["ToPlace"];
                        //Lorray details
                        entity.Ponumber = LorryData["Ponumber"];
                        entity.EwayBillNo = LorryData["EwayBillNo"];
                        entity.Transporter = LorryData["Transporter"];
                        entity.LorryNo = LorryData["LorryNo"];
                        entity.LorryOwnerName = LorryData["Owner"];
                        entity.DriverName = LorryData["Driver"];
                        entity.Dlno = LorryData["Dlno"];
                        entity.CheckPost = LorryData["CheckPost"];
                        entity.FrieghtPerBag = LorryData["FrieghtPerBag"];
                        entity.TotalFrieght = LorryData["TotalFrieght"];
                        entity.Advance = LorryData["AdvanceFrieght"];
                        entity.Balance = LorryData["BalanceFrieght"];
                        entity.IsLessOrPlus = LorryData["FrieghtPlus_Less"];//FrieghtPlus/Less field
                        entity.tdsperc = LorryData["TDS"];
                        //  Dilivary Address Details                
                        entity.DeliveryName = LorryData["DeliveryName"];
                        entity.DeliveryAddress1 = LorryData["DeliveryAddress1"];
                        entity.DeliveryAddress2 = LorryData["DeliveryAddress2"];
                        entity.DeliveryPlace = LorryData["DeliveryPlace"];
                        entity.DelPinCode = LorryData["DelPinCode"];
                        entity.DeliveryState = LorryData["DeliveryState"];
                        entity.DeliveryStateCode = LorryData["DeliveryStateCode"];
                        entity.Distance = LorryData["Distance"];
                        entity.DCNote = LorryData["Dcnote"];
                        entity.IsActive = true;
                    }
                    ///List<Inventory> inventorylist = new List<Inventory>();
                    _tenantDBContext.Update(entity);
                    _tenantDBContext.SaveChanges();

                    foreach (var item in itemdata)
                    {
                        //var Itementity = _tenantDBContext.Inventory.FirstOrDefault(Inv => Inv.CompanyId == entity.CompanyId && Inv.VochType == entity.VochType && Inv.InvoiceType == entity.InvoiceType && Inv.VochNo == entity.VochNo && Inv.LedgerId == entity.LedgerId && Inv.CommodityId == entity.CommodityID);
                        //var Itementity = _tenantDBContext.Inventory.FirstOrDefault(Inv => Inv.CompanyId == entity.CompanyId && Inv.VochType == entity.VochType && Inv.InvoiceType == entity.InvoiceType && Inv.VochNo == entity.VochNo && Inv.LedgerId == entity.LedgerId && Inv._Id == item._Id);
                        int _Id = Convert.ToInt32(item._Id);
                        var Itementity = _tenantDBContext.Inventory.FirstOrDefault(Inv => Inv._Id == _Id);
                        if (Itementity != null)
                        {

                            Itementity.CompanyId = data["CompanyId"];
                            Itementity.VochNo = data["InoviceNo"];
                            Itementity.VochType = data["VochType"];
                            Itementity.InvoiceType = data["InvoiceType"];
                            Itementity.LedgerId = data["LedgerId"];
                            Itementity.CommodityId = item["CommodityId"];
                            Itementity.TranctDate = data["OriginalInvDate"];
                            Itementity.PoNumber = LorryData["Ponumber"];
                            Itementity.EwaybillNo = LorryData["EwayBillNo"];
                            //LotNo = data["LotNo"];
                            Itementity.WeightPerBag = item["WeightPerBag"];
                            Itementity.NoOfBags = item["NoOfBags"];
                            Itementity.TotalWeight = item["TotalWeight"];
                            Itementity.Rate = item["Rate"];
                            Itementity.Amount = item["Amount"];
                            Itementity.Mark = item["Remark"];
                            Itementity.Discount = LorryData["Discount"];
                            Itementity.NetAmount = item["GrandTotal"];
                            Itementity.SGST = item["SgstAmount"];
                            Itementity.CGST = item["CsgstAmount"];
                            Itementity.IGST = item["IgstAmount"];
                            Itementity.NoOfDocra = item["NoOfDocra"];
                            Itementity.UpdatedDate = DateTime.Now;
                            Itementity.UpdatedBy = item["UpdatedBy"];
                            Itementity.IGSTRate = data["Igstvalue"];
                            Itementity.SGSTRate = data["Sgstvalue"];
                            Itementity.CGSTRate = data["Csgstvalue"];
                            Itementity.Taxable = data["TaxableValue"];
                            Itementity.IsActive = true;
                            _tenantDBContext.UpdateRange(Itementity);
                            _tenantDBContext.SaveChanges();
                        };
                    }
                    _logger.LogDebug("salesRepo : BillSummry/Inventory Updated");
                    return true;
                }
                else
                {
                    _logger.LogDebug("salesRepo : BillSummry/Inventory Updated Failed");
                    return false;
                }
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
                var data = JsonConvert.DeserializeObject<dynamic>(Data.ToString());
                int id = data["Id"];

                if (data != null)
                {
                    using (var context = new MasterDBContext())
                    {
                        var activeentity = _tenantDBContext.BillSummaries.SingleOrDefault(item => item._Id == id && item.IsActive == true);
                        var deactivateentity = _tenantDBContext.BillSummaries.SingleOrDefault(item => item._Id == id && item.IsActive == false);
                        if (activeentity != null)
                        {
                            activeentity.IsActive = false;
                            _tenantDBContext.SaveChanges();
                            _tenantDBContext.Update(activeentity);
                            _logger.LogDebug("SalesReo : sales Deactivated");
                        }
                        else if (deactivateentity != null)
                        {
                            deactivateentity.IsActive = true;
                            _tenantDBContext.SaveChanges();
                            _tenantDBContext.Update(deactivateentity);
                            _logger.LogDebug("SalesRepo : sales Activated");
                        }
                    }
                }
                return true;
                _logger.LogDebug("Delete sales done : ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw ex;
            }
        }
    }
}
