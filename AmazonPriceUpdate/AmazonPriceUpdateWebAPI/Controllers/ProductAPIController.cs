using AWSProjects.CommonUtils;
using PriceUpdateWebAPIBL;
using ProductUpdateCatalogProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AmazonPriceUpdateWebAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProductAPIController : ApiController
    {
        WebAPIManager apiManager;
        public ProductAPIController(WebAPIManager _apiManager)
        {
            this.apiManager = _apiManager;
        }


        // GET Api to get Heartbeat of service
        // Call to get service heartbeat. Done to ensure the app pool doesn't sleep.
        // RequestUri: http://localhost:59132/api/amazonproductapi?heartbeat=true
        public HttpResponseMessage Get(bool heartBeat)
        {
            return Request.CreateResponse(HttpStatusCode.OK,heartBeat);
        }

        //POST to get args and return the product details
        //Sample request
        // Uri: http://localhost:59132/api/amazonproductapi
        // Request body. Paste it as it is in Fiddler
        //{ ProductASIN:"0143066153", ProductRegion:"IN"}
        public HttpResponseMessage Post([FromBody]GetProductAPIArgs args)
        {
            HttpResponseMessage httpResponse;
            try
            {
                if (args == null)
                {
                    httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest,"Input arguments are null. Please check your request");
                }
                else
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    var response = this.apiManager.GetProductDetails(args, Request);
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please check your inputs and retry after sometime");
            }

            return httpResponse;

        }

        //PUT Will update the product with the rule given
        //Sample request
        // Uri: http://localhost:59132/api/amazonproductapi
        // Request body. Paste it as it is in Fiddler
        //{
        //"EmailId":"ragharamc@gmail.com",
        //"ProductASIN":"B00JG8FHJQ",
        //"ProductRegion":"IN",
        //"ToEmailEveryWeek":"false",
        //"ToEmailOnPrice":"true",
        //"PriceToEmail":"11500",
        //"EmailOnPriceDuration":"2/20/2015",
        //"ToEmailOnDate":"false",
        //"ProductPriceType":"All"
        //}
        public HttpResponseMessage Put([FromBody]UpdateProductAPIArgs args)
        {
            HttpResponseMessage httpResponse;
            try
            {
                if (args == null)
                {
                    httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "Input arguments are null. Please check your request");
                }
                else
                {
                    HttpRequest httpRequest = HttpContext.Current.Request;
                    var responseObj = this.apiManager.UpdateProductDetails(args, Request, APIConfigManager.BaseRequestUri);
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, responseObj);
                }
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please check your inputs and retry after sometime");
            }

            return httpResponse;
        }

        // Put will confirm, Confirmation through email
        // This is not yet used. Default everything is confirmed
        public HttpResponseMessage Put([FromBody]ProductIdArgs args, bool confirmed)
        {
            HttpResponseMessage httpResponse;
            if (args == null)
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "Input arguments are null. Please check your request");
            }
            else
            {
                HttpRequest httpRequest = HttpContext.Current.Request;
                httpResponse = this.apiManager.ConfirmProductDetails(args, confirmed, Request);
                //Fix this later. This is inconsistent
                httpResponse = Request.CreateResponse(httpResponse.StatusCode, "Confirmed!");
            }

            return httpResponse;
        }

        //DELETE Delete's the product rule and marks the product as deleted
        //Yet to be implemented
        //TODO: Figure out logical or hard deletion 
        public HttpResponseMessage Delete([FromBody]ProductIdArgs args)
        {
            return null;
        }

    }
}
