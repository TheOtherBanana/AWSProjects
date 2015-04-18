using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductUpdateCatalogProvider.CatalogEntities
{
    [DynamoDBTable(CatalogConstants.RegisteredProductsTableName)]
    public class ProductCatalogEntity
    {
        [DynamoDBHashKey]
        public string EmailId { get; set; }
        [DynamoDBRangeKey]
        public string ASIN { get; set; }
        public string Country { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDeleted { get; set; }
        public bool ToEmailEveryWeek { get; set; }
        public string DayToEmail { get; set; }
        public DateTime EmailEveryWeekDuration { get; set; }
        public bool ToEmailOnPrice { get; set; }
        public string PriceToEmail { get; set; }
        public DateTime EmailOnPriceDuration { get; set; }
        public bool ToEmailOnDate { get; set; }
        public DateTime DateToEmail { get; set; }
        public string ProductPriceType { get; set; }
        public DateTime EmailSentOn { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public DateTime DateDeleted { get; set; }
        public string InitialPrice { get; set; }
        public string CurrentPrice { get; set; }
        public string CurrencyCode { get; set; }
        public double AmountSaved { get; set; }

        public ProductCatalogEntity()
        {
            IsConfirmed = IsDeleted = ToEmailEveryWeek = ToEmailOnPrice = ToEmailOnDate = false;
            Country = ASIN = EmailId = DayToEmail = PriceToEmail = ProductPriceType = string.Empty;
        }
    }

    [DynamoDBTable(CatalogConstants.DoSLimitTableName)]
    public class DoSLimitEntity
    {
        public string EmailId { get; set; }
        [DynamoDBHashKey]
        public string UserIPAddress { get; set; }
        public long Count { get; set; }

        public DoSLimitEntity()
        {
            EmailId = UserIPAddress = string.Empty;
            Count = 0;
        }
    }
}
