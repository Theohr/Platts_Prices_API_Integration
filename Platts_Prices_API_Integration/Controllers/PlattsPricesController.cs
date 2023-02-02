using Danaos.Shared;
using Danaos.TRD.App.Entities;
using Danaos.TRD.App.Repositories;
using Danaos.TRD.App.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Danaos.TRD.App.Controllers
{
    [RoutePrefix("api/v1/TRD/platts-prices")]
    public class PlattsPricesController : ControllerBase<PlattsPricesService, RepositoryBase<PlattsPrices>, PlattsPrices>

    {
        public PlattsPricesController(PlattsPricesService service) : base(service)
        {
        }


        public override async Task<HttpResponseMessage> Get(HttpRequestMessage request)
        {
            var user = SessionVars.UserID;
            //ServiceBase.addLog(user, " [PlattsPricesController/Get] Called");
            if (String.IsNullOrWhiteSpace(request.RequestUri.Query)) return await base.Get(request);

            string dateFrom = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("dateFrom");
            string dateTo = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("dateTo");
            
            return Request.CreateResponse(HttpStatusCode.OK, await service.GetFromParent(dateFrom, dateTo));
        }

        [HttpPost]
        [Route("importPlattsPrices")]
        public  async Task<HttpResponseMessage> UploadPlattsPrices(System.Web.Mvc.FormCollection file)
         {
             return Request.CreateResponse(HttpStatusCode.OK, await service.SavePlattsPrices(file));
         }

        [HttpPost]
        [Route("uploadExcelPlatts")]
        public async Task<HttpResponseMessage> UploadExcelPlatts(HttpRequestMessage request)
        {
            var user = SessionVars.UserID;
            //ServiceBase.addLog(user, " [TrdAttachmentController/UploadFile] Called");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ?
            HttpContext.Current.Request.Files[0] : null;

            string id = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("id");
            string fileName = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("fileName");
            string type = HttpUtility.ParseQueryString(request.RequestUri.Query).Get("type");

            return Request.CreateResponse(HttpStatusCode.OK, await service.saveUploadedFile(file.InputStream, file.FileName, type));

            //else { resultUpload.status = resultMessage.resultValue.error; resultUpload.message = "Oups, ou sont les fichiers"; }
            //return JsonNetResult.JsonNet(resultUpload);
        }

        [HttpPost]
        [Route("syncPlatts")]
        public async Task<HttpResponseMessage> syncPlatts(HttpRequestMessage request)
        {
            return Request.CreateResponse(HttpStatusCode.OK, await service.syncPlattsPrices());
        }

    }
}



