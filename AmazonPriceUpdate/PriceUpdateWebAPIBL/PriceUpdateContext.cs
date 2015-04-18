using Amazon.DynamoDBv2;
using AmazonProductAPIWrapper;
using AWSProjects.CommonUtils;
using EmailUtils;
using ProductUpdateCatalogProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PriceUpdateWebAPIBL
{
    public class PriceUpdateContext
    {
        private string aws_ACCESS_KEY;
        private string aws_SECRET_KEY;
        private CatalogProviderFactory catalogFactory;

        private static PriceUpdateContext instance;

        public string AWS_ACCESS_KEY
        {
            get
            {
                return this.aws_ACCESS_KEY;
            }
        }

        public string AWS_SECRET_KEY
        {
            get
            {
                return this.aws_SECRET_KEY;
            }
        }

        public CatalogProviderFactory CatalogFactory
        {
            get
            {
                return this.catalogFactory;
            }
        }

        public static PriceUpdateContext Instance
        {
            get
            {
                if(instance == null)
                {
                    throw new ArgumentNullException("PriceUpdateContext is not initialized. Value is null");
                }

                return instance;
            }
        }

        public static void Initialize(string accessKey, string secretKey, string emailId, string emailIdPassword, string host, string displayName, List<string> tablesToInit)
        {
            Debug.Assert(!string.IsNullOrEmpty(accessKey), "AWS Access Key is null");
            Debug.Assert(!string.IsNullOrEmpty(secretKey), "AWS Secret Key is null");
            Debug.Assert(!string.IsNullOrEmpty(emailId), "Email Id is null");
            Debug.Assert(!string.IsNullOrEmpty(emailIdPassword), "EmailId Password is null");
            Debug.Assert(tablesToInit != null, "TablesToInit iss null");

            if (instance == null)
            {
                instance = new PriceUpdateContext(accessKey, secretKey, emailId, emailIdPassword, host, displayName, tablesToInit);
            }


            DynamoDBTracer.Tracer.Write("Initialization successful");
            //. Trace non secrets

        }

        private PriceUpdateContext (string accessKey, string secretKey, string emailId, string emailIdPassword, string host, string displayName, List<string> tablesToInit)
        {
            this.aws_ACCESS_KEY = accessKey;
            this.aws_SECRET_KEY = secretKey;


            DynamoDBTracer.InitializeListener(this.aws_ACCESS_KEY, this.aws_SECRET_KEY);

            DynamoDBTracer.Tracer.Write("Initalizing Price update context");
            EmailManagerContext.Initialize(emailId, emailIdPassword, host, displayName);
            AmazonProductAPIContext.Initialize(this.aws_ACCESS_KEY, this.aws_SECRET_KEY);

            CatalogProviderFactoryArgs factoryArgs = new CatalogProviderFactoryArgs 
            { 
                DynamoDBClient = new AmazonDynamoDBClient(this.aws_ACCESS_KEY, this.aws_SECRET_KEY,Amazon.RegionEndpoint.USWest2) 
            };
            this.catalogFactory = new CatalogProviderFactory(factoryArgs);

            this.catalogFactory.InitializeCatalogs(tablesToInit);
        }

    }

    public class HeartbeatManager
    {
        string serviceApiEndpoint;
        TimeSpan waitTimespan;
        public HeartbeatManager(string endPoint, TimeSpan timespan)
        {
            this.serviceApiEndpoint = endPoint;
            this.waitTimespan = timespan;
        }

        public void CheckServiceHeartbeat()
        {
            while(true)
            {
                string requestUri = string.Format("{0}?heartbeat=true", this.serviceApiEndpoint);
                var response = RestApiUtils.GetHttpResponse(requestUri);
                
                if(response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    //Send service heartbeat not working mail
                }

                Thread.Sleep(waitTimespan);
            }
        }
    }
}
