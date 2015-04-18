using AmazonProductAPIWrapper.External;
using AWSProjects.CommonUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonProductAPIWrapper
{
    public static class AmazonProductAPIConstants
    {
        public const string PrmSre = "Service";
        public const string PrmCondition = "Condition";
        public const string PrmOperation = "Operation";
        public const string PrmItemId = "ItemId";
        public const string PrmResponseGroup = "ResponseGroup";
        public const string PrmAssociateTag = "AssociateTag";
        public const string PrmIdType = "ASIN";
        public const string PrmVersion = "Version";

        public static Dictionary<AmazonProductAPIContext.Regions, string> RegionMapping = new Dictionary<AmazonProductAPIContext.Regions, string>
        {
            {AmazonProductAPIContext.Regions.BR,"Brazil"},
            {AmazonProductAPIContext.Regions.CA,"Canada"},
            {AmazonProductAPIContext.Regions.CN,"China"},
            {AmazonProductAPIContext.Regions.DE,"Germany"},
            {AmazonProductAPIContext.Regions.ES,"Spain"},
            {AmazonProductAPIContext.Regions.FR,"France"},
            {AmazonProductAPIContext.Regions.IN,"India"},
            {AmazonProductAPIContext.Regions.IT,"Italy"},
            {AmazonProductAPIContext.Regions.JP,"Japan"},
            {AmazonProductAPIContext.Regions.UK,"United Kingdom"},
            {AmazonProductAPIContext.Regions.US,"USA"},
        };

        public static Dictionary<AmazonProductAPIContext.Regions, string> RegionPublicURLMapping = new Dictionary<AmazonProductAPIContext.Regions, string>
        {
            {AmazonProductAPIContext.Regions.BR,"http://www.amazon.br"},
            {AmazonProductAPIContext.Regions.CA,"http://www.amazon.ca"},
            {AmazonProductAPIContext.Regions.CN,"http://www.amazon.cn"},
            {AmazonProductAPIContext.Regions.DE,"http://www.amazon.de"},
            {AmazonProductAPIContext.Regions.ES,"http://www.amazon.es/"},
            {AmazonProductAPIContext.Regions.FR,"http://www.amazon.fr/"},
            {AmazonProductAPIContext.Regions.IT,"http://www.amazon.it/"},
            {AmazonProductAPIContext.Regions.IN,"http://www.amazon.in/"},
            {AmazonProductAPIContext.Regions.JP,"http://www.amazon.co.jp/"},
            {AmazonProductAPIContext.Regions.UK,"http://www.amazon.co.uk/"},
            {AmazonProductAPIContext.Regions.US,"http://www.amazon.com/"},
        };

        public const string DfltVersion = "2011-08-01";
        public const string DfltCondition = "All";
    }

    public class AmazonProductAPIContext
    {
        public enum Regions
        {
            BR,
            CA,
            CN,
            DE,
            ES,
            FR,
            IN,
            IT,
            JP,
            UK,
            US
        }
        public static Dictionary<Regions, string> RegionUrlMapping = new Dictionary<Regions,string>
        {
            {Regions.BR,"webservices.amazon.com.br"},
            {Regions.CA,"webservices.amazon.ca"},
            {Regions.CN,"webservices.amazon.cn"},
            {Regions.DE,"webservices.amazon.de"},
            {Regions.ES,"webservices.amazon.es"},
            {Regions.FR,"webservices.amazon.fr"},
            {Regions.IN,"webservices.amazon.in"},
            {Regions.IT,"webservices.amazon.it"},
            {Regions.JP,"webservices.amazon.co.jp"},
            {Regions.UK,"webservices.amazon.co.uk"},
            {Regions.US,"webservices.amazon.com"},
        };

        public static Dictionary<Regions, string> RegionAssociateIdMapping = new Dictionary<Regions, string>
        {
            {Regions.BR,"NA"},
            {Regions.CA,"NA"},
            {Regions.CN,"NA"},
            {Regions.DE,"NA"},
            {Regions.ES,"NA"},
            {Regions.FR,"NA"},
            {Regions.IN,"n08fe-21"},
            {Regions.IT,"NA"},
            {Regions.JP,"NA"},
            {Regions.UK,"NA"},
            {Regions.US,"amazonp01-20"},
        };

        public string AWS_ACCESS_KEY;
        public string AWS_ASSOCIATE_TAG;
        public string AWS_SECRET_KEY;

        private static AmazonProductAPIContext instance;

        private AmazonProductAPIContext(string accessKey, string secretKey)
        {
            Debug.Assert(!string.IsNullOrEmpty(accessKey), "AWS Access Key cannot be null or empty");
            Debug.Assert(!string.IsNullOrEmpty(secretKey), "AWS Secret Key cannot be null or empty");

            this.AWS_ACCESS_KEY = accessKey;
            this.AWS_SECRET_KEY = secretKey;
        }

        public static AmazonProductAPIContext Instance
        {
            get
            {
                if(instance==null)
                {
                    DynamoDBTracer.Tracer.Write("Context not initialized. AmazonProductAPIContext value is null");
                    throw new ArgumentNullException("AmazonProductAPIWrapper is null or not initialized");
                }

                return instance; 
            }
        }

        public static void Initialize(string accessKey, string secretKey)
        {
            DynamoDBTracer.Tracer.Write("Initializing AmazonProductAPIWrapper");

            instance =  new AmazonProductAPIContext(accessKey, secretKey);

            DynamoDBTracer.Tracer.Write("AmazonProductAPIWrapper initialized successfully");
        }
        
        public string GetOffersForItemId(string itemId, string regionString, string condition = AmazonProductAPIConstants.DfltCondition, string version = AmazonProductAPIConstants.DfltVersion)
        {
            DynamoDBTracer.Tracer.Write(string.Format("GetOffersForItemId called for id {0}, region {1}", itemId, regionString));
            Regions region = ValidateAndGetRegion(regionString);

            this.AWS_ASSOCIATE_TAG = RegionAssociateIdMapping[region];
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("Service", "AWSECommerceService");
            request.Add("Condition", condition);
            request.Add("Operation", "ItemLookup"); /* Operation for get items*/
            request.Add("ItemId", itemId);
            request.Add("ResponseGroup", "OfferSummary"); /* Since we are getting offers */
            request.Add("AssociateTag", this.AWS_ASSOCIATE_TAG);
            request.Add("IdType", "ASIN"); /* Because we are querying based on ASIN*/
            request.Add("Version", version);

            DynamoDBTracer.Tracer.Write(string.Format("Executing GetOffersForItemId for itemId {0} in region {1}", itemId, region));
            string response = this.ExecuteRequest(request,region);
            return response;
        }

        public string GetDetailsForItemId(string itemId, string regionString, string condition = AmazonProductAPIConstants.DfltCondition, string version = AmazonProductAPIConstants.DfltVersion)
        {
            DynamoDBTracer.Tracer.Write(string.Format("GetDetailsForItemId called for id {0}, region {1}", itemId, regionString));
            Regions region = ValidateAndGetRegion(regionString);
            this.AWS_ASSOCIATE_TAG = RegionAssociateIdMapping[region];
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("Service", "AWSECommerceService");
            request.Add("Condition", condition);
            request.Add("Operation", "ItemLookup"); /* Operation for get items*/
            request.Add("ItemId", itemId);
            request.Add("ResponseGroup", "Small"); /* Since we are getting details */
            request.Add("AssociateTag", this.AWS_ASSOCIATE_TAG);
            request.Add("IdType", "ASIN"); /* Because we are querying based on ASIN*/
            request.Add("Version", version);

            string response = this.ExecuteRequest(request, region);
            return response;
        }

        public string GetCartDetails(string itemId, string regionString, string condition = AmazonProductAPIConstants.DfltCondition, string version = AmazonProductAPIConstants.DfltVersion)
        {
            DynamoDBTracer.Tracer.Write(string.Format("GetCartDetails called for id {0}, region {1}", itemId, regionString));
            Regions region = ValidateAndGetRegion(regionString);

            this.AWS_ASSOCIATE_TAG = RegionAssociateIdMapping[region];
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("Service", "AWSECommerceService");
            request.Add("Operation", "CartCreate"); /* Operation for get items*/
            request.Add("Item.1.ASIN", itemId);
            request.Add("Item.1.Quantity", "1"); /* Since we are adding to cart to get purchase url details */
            request.Add("AssociateTag", this.AWS_ASSOCIATE_TAG);
            request.Add("Version", version);
            request.Add("Marketplace", regionString.ToLower());

            string response = this.ExecuteRequest(request, region);
            return response;
        }

        private Regions ValidateAndGetRegion(string regionString)
        {
            Regions region;
            if (!Enum.TryParse(regionString.ToUpper(), out region))
            {
                //Trace: Invalid region. Region {0}
                throw new InvalidCastException(string.Format("Invalid region {0}", regionString));
            }
            return region;
        }

        private string ExecuteRequest(Dictionary<string,string> request,Regions region)
        {
            SignedRequestHelper helper = new SignedRequestHelper(this.AWS_ACCESS_KEY, this.AWS_SECRET_KEY, RegionUrlMapping[region]);
            string requestUri = helper.Sign(request);

            string response = RestApiUtils.ExecuteRestApi(requestUri);

            return response;
        }
    }
}
