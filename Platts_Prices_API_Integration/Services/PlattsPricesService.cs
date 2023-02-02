using Danaos.TRD.App.Entities;
using Danaos.TRD.App.Repositories;
using System;
using Danaos.TRD.App.Dto;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.XlsIO;
using System.Data;
using Danaos.Shared;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;

namespace Danaos.TRD.App.Services
{
    public class PlattsPricesService : ServiceBase<PlattsPricesRepository, PlattsPrices>
    {
        protected PlattsCodesService plattsCodesService;
        public PlattsPricesService(PlattsPricesRepository repository, PlattsCodesService plattsCodesService) : base(repository)
        {
            this.plattsCodesService = plattsCodesService;
        }

        protected override PlattsPrices ConstructEntityWithID(params object[] ids) =>//WARNING: SHUOULD IMPLEMENT ConstructEntityWithID
            new PlattsPrices()
            {
                Id = (int)ids[0],
                Dt = (DateTime)ids[1],
                PlattsCode = (string)ids[2]
            };

        internal Task<IEnumerable<PlattsPrices>> GetFromParent(string dateFrom, string dateTo) =>
            repository.FetchFromParent(dateFrom, dateTo);



        internal async Task<IEnumerable<PlattsPrices>> SavePlattsPrices(System.Web.Mvc.FormCollection file)
        {
            IEnumerable <PlattsCodes> platssCodes = await plattsCodesService.GetAll();
            List<PlattsCodes> plattsCodesList = platssCodes.ToList();
            List<String> plattsCodes = new List<String>();

            foreach (PlattsCodes plattsCode in plattsCodesList)
            {
                plattsCodes.Add(plattsCode.Code);
            }


            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Loads or open an existing workbook
                
                FileStream inputStream = new FileStream("C:\\Users\\user\\Desktop\\forDelete\\EB_hist.csv", FileMode.Open);
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(inputStream);
                IWorksheet worksheet = workbook.Worksheets[0];

                String test1 = worksheet.Range["B4"].Text;
                worksheet.UsedRangeIncludesFormatting = true;

                System.Diagnostics.Debug.WriteLine("Testtttttttttt");
                System.Diagnostics.Debug.WriteLine(worksheet.Range["B4"].Text);
                System.Diagnostics.Debug.WriteLine(worksheet.UsedRange.Text);
                System.Diagnostics.Debug.WriteLine(test1);

               //DataTable plattsPriceTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                DataTable plattsPriceTable = worksheet.ExportDataTable(worksheet.Range["A1:R5051"], ExcelExportDataTableOptions.ColumnNames);

                List<PlattsPrices> plattsPricesList = new List<PlattsPrices>();
                //PlattsPrices[] plattsPricesArr = new PlattsPrices[200];

                //PlattsCodesService plattsCodesService = new PlattsCodesService(new RepositoryBase<PlattsCodes>(DapperConnection dbContext));
                //var list = await PlattsCodesService.GetAll();

                System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                int tmpId = repository.FetchMax("ID");

                for (int i = 1; i < plattsPriceTable.Rows.Count ; i++)
                {
                    PlattsPrices pp = new PlattsPrices();
                    String tmpPlattsCode= plattsPriceTable.Rows[i][1].ToString();
                    if (plattsCodes.Contains(tmpPlattsCode))
                    {
                        try
                        {
                            string platts_Code= plattsPriceTable.Rows[i][1].ToString();
                            string Dt = plattsPriceTable.Rows[i][5].ToString();
                           
                            pp.PlattsCode = plattsPriceTable.Rows[i][1].ToString();
                            pp.Currency = plattsPriceTable.Rows[i][3].ToString();
                            string iDate = plattsPriceTable.Rows[i][5].ToString();
                            DateTime oDate = DateTime.ParseExact(iDate, "yyyy/MM/dd", provider);
                            pp.Dt = oDate;

                            string teststr = $@"PLATTS_CODE='" + platts_Code + "' AND  DT=TO_DATE('" + Dt + "', 'DD/MM/YYYY')";
                            //int numOfInstances = repository.FetchCount($"PLATTS_CODE='AAIDL00' AND  DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                            int numOfInstances = repository.FetchCount($@"PLATTS_CODE='" + platts_Code+ "' AND  DT=TO_DATE('"+Dt+ "', 'YYYY/MM/DD')", new {platts_Code, Dt});



                            //DateTime tmpDtDateTime = Convert.ToDateTime(tmpDt);
                            //DateTime tmpDtDateTime = (DateTime)plattsPriceTable.Rows[i][5].ToDateTime();

                            //System.Diagnostics.Debug.WriteLine(tmpDtDateTime.ToString());
                            //pp.Dt = (DateTime)plattsPriceTable.Rows[i][5].ToDateTime();

                            //string iDate = plattsPriceTable.Rows[i][5].ToString();
                            //string iDate = "15/07/2021";
                            //iDate = Regex.Replace(iDate, @"\s+", "");
                            //DateTime oDate = Convert.ToDateTime(iDate);

                            //string iDate2 = "2021/07/15";
                            //DateTime oDate2 = Convert.ToDateTime(iDate2);





                            pp.LowPrice = Convert.ToDecimal(plattsPriceTable.Rows[i][6].ToString());
                            pp.HighPrice = Convert.ToDecimal(plattsPriceTable.Rows[i][7].ToString());
                            pp.ClosePrice = Convert.ToDecimal(plattsPriceTable.Rows[i][8].ToString());
                            
                            if (numOfInstances > 0)
                            {
                                PlattsPrices ppUpdate = new PlattsPrices();
                                ppUpdate = repository.QuerySingle("SELECT ID As Id, DT As Dt, PLATTS_CODE As PlattsCode, CURRENCY As Currency, LOW_PRICE As LowPrice, HIGH_PRICE As HighPrice, CLOSE_PRICE As ClosePrice FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='AAIDL00' AND DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                                ppUpdate.Original = JsonConvert.SerializeObject(ppUpdate);
                                //ppUpdate = await repository.QuerySingleAsync("SELECT * FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='AAIDL00' AND DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                                //ppUpdate = await repository.QuerySingleAsync("SELECT ID, DT, PLATTS_CODE, CURRENCY, LOW_PRICE, HIGH_PRICE, CLOSE_PRICE FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='AAIDL00' AND DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                                //IEnumerable<PlattsPrices> ppUpdateIE;
                                //List<PlattsPrices> plattsPricesList2 = new List<PlattsPrices>();
                                //ppUpdateIE = repository.SelectRows("SELECT * FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='AAIDL00' AND DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                                //plattsPricesList2 = ppUpdateIE.ToList();

                                int ppUpdateId = ppUpdate.Id;
                                System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                                ppUpdate.LowPrice = pp.LowPrice;
                                ppUpdate.HighPrice = pp.HighPrice ;
                                ppUpdate.ClosePrice = pp.ClosePrice;
                                

                                
                                var res = Update(ppUpdate);
                                System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                            }
                            else
                            {
                                tmpId = tmpId + 1;
                                pp.Id = tmpId;
                                plattsPricesList.Add(pp);
                            }
                            
                        }catch (Exception e){
                            System.Diagnostics.Debug.WriteLine(e.ToString());
                        }
                    }
                }

                if (plattsPricesList.Count>0)
                {
                    var res = await InsertAllAsync(plattsPricesList.ToArray());
                }

                //Close the instance of IWorkbook
                workbook.Close();

                //Dispose the instance of ExcelEngine
                excelEngine.Dispose();



             
            }
            return null;
        }


        internal async Task<IEnumerable<PlattsPrices>> saveUploadedFile(Stream file, string fileName, string type)
        {
            try
            {
                IEnumerable<PlattsCodes> platssCodes = await plattsCodesService.GetAll();
                List<PlattsCodes> plattsCodesList = platssCodes.ToList();
                List<String> plattsCodes = new List<String>();

                foreach (PlattsCodes plattsCode in plattsCodesList)
                {
                    plattsCodes.Add(plattsCode.Code);
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    //Loads or open an existing workbook

                    //FileStream inputStream = new FileStream(file., FileMode.Open);
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Open(file);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    String test1 = worksheet.Range["B4"].Text;
                    worksheet.UsedRangeIncludesFormatting = true;

                    System.Diagnostics.Debug.WriteLine("Testtttttttttt");
                    System.Diagnostics.Debug.WriteLine(worksheet.Range["B4"].Text);
                    System.Diagnostics.Debug.WriteLine(worksheet.UsedRange.Text);
                    System.Diagnostics.Debug.WriteLine(test1);

                    //DataTable plattsPriceTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                    DataTable plattsPriceTable = worksheet.ExportDataTable(worksheet.Range["A1:R5051"], ExcelExportDataTableOptions.ColumnNames);

                    List<PlattsPrices> plattsPricesList = new List<PlattsPrices>();
                    //PlattsPrices[] plattsPricesArr = new PlattsPrices[200];

                    //PlattsCodesService plattsCodesService = new PlattsCodesService(new RepositoryBase<PlattsCodes>(DapperConnection dbContext));
                    //var list = await PlattsCodesService.GetAll();

                    System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
                    int tmpId = repository.FetchMax("ID");

                    for (int i = 1; i < plattsPriceTable.Rows.Count; i++)
                    {
                        PlattsPrices pp = new PlattsPrices();
                        String tmpPlattsCode = plattsPriceTable.Rows[i][1].ToString();
                        if (plattsCodes.Contains(tmpPlattsCode))
                        {
                            try
                            {
                                string platts_Code = plattsPriceTable.Rows[i][1].ToString();
                                string Dt = plattsPriceTable.Rows[i][5].ToString();

                                pp.PlattsCode = plattsPriceTable.Rows[i][1].ToString();
                                pp.Currency = plattsPriceTable.Rows[i][3].ToString();
                                string iDate = plattsPriceTable.Rows[i][5].ToString();
                                DateTime oDate = DateTime.ParseExact(iDate, "yyyy/MM/dd", provider);
                                pp.Dt = oDate;

                                string teststr = $@"PLATTS_CODE='" + platts_Code + "' AND  DT=TO_DATE('" + Dt + "', 'DD/MM/YYYY')";

                                int numOfInstances = repository.FetchCount($@"PLATTS_CODE='" + platts_Code + "' AND  DT=TO_DATE('" + Dt + "', 'YYYY/MM/DD')", new { platts_Code, Dt });

                                pp.LowPrice = Convert.ToDecimal(plattsPriceTable.Rows[i][6].ToString());
                                pp.HighPrice = Convert.ToDecimal(plattsPriceTable.Rows[i][7].ToString());
                                pp.ClosePrice = Convert.ToDecimal(plattsPriceTable.Rows[i][8].ToString());

                                if (numOfInstances > 0)
                                {
                                    PlattsPrices ppUpdate = new PlattsPrices();
                                    ppUpdate = repository.QuerySingle("SELECT ID As Id, DT As Dt, PLATTS_CODE As PlattsCode, CURRENCY As Currency, LOW_PRICE As LowPrice, HIGH_PRICE As HighPrice, CLOSE_PRICE As ClosePrice FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='AAIDL00' AND DT=TO_DATE('14/07/2021', 'DD/MM/YYYY')");
                                    ppUpdate.Original = JsonConvert.SerializeObject(ppUpdate);

                                    int ppUpdateId = ppUpdate.Id;
                                    System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                                    ppUpdate.LowPrice = pp.LowPrice;
                                    ppUpdate.HighPrice = pp.HighPrice;
                                    ppUpdate.ClosePrice = pp.ClosePrice;

                                    var res = Update(ppUpdate);
                                    System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                                }
                                else
                                {
                                    tmpId = tmpId + 1;
                                    pp.Id = tmpId;
                                    plattsPricesList.Add(pp);
                                }

                            }
                            catch (Exception e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.ToString());
                            }
                        }
                    }

                    if (plattsPricesList.Count > 0)
                    {
                        var res = await InsertAllAsync(plattsPricesList.ToArray());
                    }

                    //Close the instance of IWorkbook
                    workbook.Close();

                    //Dispose the instance of ExcelEngine
                    excelEngine.Dispose();
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal async Task<PlattsPrices> syncPlattsPrices()
        {
            var bearerKey = authenticationPlattsAPI();

            var data = (JObject)JsonConvert.DeserializeObject(bearerKey);

            string accessToken = "Bearer " + data["access_token"].Value<string>();

            //Console.WriteLine(accessToken);

            plattsMarketData(accessToken);

            return null;
        }

        private string authenticationPlattsAPI()
        {
            try
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                //RestClient client = new RestClient(url);
                RestClient client = new RestClient("https://api.platts.com/auth/api");
                //client.Authenticator = new HttpBasicAuthenticator("nKTBgkEVCrieylmxTVmo","theodoros.h@island-oil.com", "TH23hr1998*");

                //RestRequest request = new RestRequest("/", Method.Post);
                //{
                //    Resource = "https://api.platts.com/auth/api"
                //};

                RestRequest request = new RestRequest()
                {
                    Method = Method.Post
                };

                request.AddHeader("accept", "application/json");
                request.AddHeader("appkey", "plattsAPIKey");
                //request.AddHeader("appkey", "BPKXYyGxYYszGMoLNAXW");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("username", "user email");
                request.AddParameter("password", "user password");

                //RestResponse response = client.Post(request);
                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return e.ToString();
            }
        }

        private async void plattsMarketData(string bearerKeyVar)
        {
            try
            {
                var symbolsURL = "https://api.platts.com/market-data/v3/value/current/symbol?Filter=symbol%20IN%20%28";
                //var symbolsURL = "https://api.platts.com/market-data/v3/value/history/symbol?Filter=symbol%20IN%20%28";

                // Get PLATTS CODES IN A LIST
                IEnumerable<PlattsCodes> platssCodes = await plattsCodesService.GetAll();
                List<PlattsCodes> plattsCodesList = platssCodes.ToList();
                List<String> plattsCodes = new List<String>();
                List<PlattsPrices> plattsPricesList = new List<PlattsPrices>();
                int tmpId = repository.FetchMax("ID");

                foreach (PlattsCodes plattsCode in plattsCodesList)
                {
                    plattsCodes.Add(plattsCode.Code);
                }

                var lastItem = plattsCodes.Last();
                // GET LAST WEEKS DATE
                string dateTodaytmp = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");

                for (int i = 1; i < plattsCodes.Count; i++)
                {
                    if (plattsCodes[i] == lastItem)
                    {
                        symbolsURL = symbolsURL + "%22" + plattsCodes[i] + "%22%29%20AND%20modDate%3E%3D%22" + dateTodaytmp + "%22";
                        //symbolsURL = symbolsURL + "%22" + plattsCodes[i] + "%22%29%20AND%20assessDate%3E%222021-01-01%22%20";
                    }
                    else
                    {
                        symbolsURL = symbolsURL + "%22" + plattsCodes[i] + "%22%2C%20";
                    }
                }

                // GET ALL SYMBOLS IN TABLE AND SEARCH FOR THE PRICES WITH BEARER KEY
                //var client = new RestClient("https://api.platts.com/market-data/v3/value/current/modified-date?Filter=modDate%3E%3D%22" + dateTodaytmp + "%22");
                var client = new RestClient(symbolsURL);
                var request = new RestRequest()
                {
                    Method = Method.Get
                };

                request.AddHeader("accept", "application/json");
                request.AddHeader("appkey", "nKTBgkEVCrieylmxTVmo");
                request.AddHeader("Authorization", bearerKeyVar);

                RestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                // Create a platts prices list and the table from the response content
                var dataPlatts = JsonConvert.DeserializeObject<PlattsPricesData>(response.Content);

                var objRows = dataPlatts.results.Count();

                for (int i = 0; i < objRows; i++)
                {
                    //Get the Description of the Symbol
                    var symbolDescURL = "https://prod.api.platts.com/market-data/reference-data/v3/search?Filter=symbol%20IN%20%28%22" + dataPlatts.results[i].symbol + "%22%29";

                    var client1 = new RestClient(symbolDescURL);
                    var request1 = new RestRequest()
                    {
                        Method = Method.Get
                    };

                    request1.AddHeader("accept", "application/json");
                    request1.AddHeader("appkey", "nKTBgkEVCrieylmxTVmo");
                    request1.AddHeader("Authorization", bearerKeyVar);

                    RestResponse response1 = client1.Execute(request1);

                    var symbolDesc = JsonConvert.DeserializeObject<plattsCodesDesc>(response1.Content);

                    //Get the History of the Symbol
                    var symbolHistURL = "https://api.platts.com/market-data/v3/value/history/symbol?Filter=symbol%20IN%20%28%22" + dataPlatts.results[i].symbol + "%22%29%20AND%20assessDate%3E%222021-01-01%22%20";

                    var client2 = new RestClient(symbolHistURL);
                    var request2 = new RestRequest()
                    {
                        Method = Method.Get
                    };

                    request2.AddHeader("accept", "application/json");
                    request2.AddHeader("appkey", "nKTBgkEVCrieylmxTVmo");
                    request2.AddHeader("Authorization", bearerKeyVar);

                    RestResponse response2 = client2.Execute(request2);

                    var symbolHist = JsonConvert.DeserializeObject<PlattsPricesData>(response2.Content);
                    var dataCount = symbolHist.results[0].data.Count();

                    for (int j = 0; j < dataCount; j++)
                    {
                        PlattsPrices pp = new PlattsPrices();

                        if (symbolHist.results[0].data[j].bate.Equals("c"))
                        {
                            var newDate = symbolHist.results[0].data[j].assessDate.ToString("dd/MM/yyyy");
                            pp.PlattsCode = symbolHist.results[0].symbol;
                            try
                            {
                                pp.Currency = symbolDesc.results[0].currency;
                            }
                            catch
                            {
                                pp.Currency = "USD";
                            }
                            pp.Dt = symbolHist.results[0].data[j].assessDate;
                            pp.ClosePrice = symbolHist.results[0].data[j].value.ToDecimal();
                            try
                            {
                                if (symbolHist.results[0].data[j + 1].bate.Equals("h"))
                                {
                                    pp.HighPrice = symbolHist.results[0].data[j + 1].value.ToDecimal();
                                }
                            }
                            catch
                            {
                                pp.HighPrice = 0;
                            }
                            try
                            {
                                if (symbolHist.results[0].data[j + 2].bate.Equals("l"))
                                {
                                    pp.LowPrice = symbolHist.results[0].data[j + 2].value.ToDecimal();
                                }
                            }
                            catch
                            {
                                pp.LowPrice = 0;
                            }

                            int numOfInstances = repository.FetchCount($@"PLATTS_CODE='" + symbolHist.results[0].symbol + "' AND  DT=TO_DATE('" + newDate + "', 'DD/MM/YYYY')", new { symbolHist.results[0].symbol, newDate });

                            if (numOfInstances > 0)
                            {
                                PlattsPrices ppUpdate = new PlattsPrices();
                                ppUpdate = repository.QuerySingle("SELECT ID As Id, DT As Dt, PLATTS_CODE As PlattsCode, CURRENCY As Currency, LOW_PRICE As LowPrice, HIGH_PRICE As HighPrice, CLOSE_PRICE As ClosePrice FROM TRD_PLATTS_PRICES WHERE PLATTS_CODE='" + symbolHist.results[0].symbol + "' AND DT=TO_DATE('" + newDate + "', 'DD/MM/YYYY')");
                                ppUpdate.Original = JsonConvert.SerializeObject(ppUpdate);

                                int ppUpdateId = ppUpdate.Id;
                                System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                                ppUpdate.LowPrice = pp.LowPrice;
                                ppUpdate.HighPrice = pp.HighPrice;
                                ppUpdate.ClosePrice = pp.ClosePrice;

                                var res = Update(ppUpdate);
                                System.Diagnostics.Debug.WriteLine("ppUpdateId=" + ppUpdateId);
                            }
                            else
                            {
                                tmpId += 1;
                                pp.Id = tmpId;
                                plattsPricesList.Add(pp);
                            }
                        }
                    }
                }

                if (plattsPricesList.Count > 0)
                {
                    var res = await InsertAllAsync(plattsPricesList.ToArray());
                    System.Diagnostics.Debug.WriteLine("Data Inserted Succesfully!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}