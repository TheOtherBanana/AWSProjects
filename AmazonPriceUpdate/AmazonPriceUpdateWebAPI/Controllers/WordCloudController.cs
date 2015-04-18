using QuoraWCGeneratorBL;
using QuoraWCGeneratorBL.DataContracts;
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
    public class WordCloudController : ApiController
    {
        private WordCloudGenerator wordCloudGenerator;

        public WordCloudController(WordCloudGenerator _wordCloudGenerator)
        {
            this.wordCloudGenerator = _wordCloudGenerator;
            this.wordCloudGenerator.AlchemyApiKey = APIConfigManager.AlchemyApiPricateKey;
            this.wordCloudGenerator.ImgurClientId = APIConfigManager.ImgurClientId;
        }
        
        public HttpResponseMessage Post([FromBody]GenerateWordCloudArgs args)
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
                    var response = this.wordCloudGenerator.GenerateWordCloud(args);
                    httpResponse = Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch
            {
                httpResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong. Please check your inputs and retry after sometime");
            }

            return httpResponse;
        }
    }
}
