using AmazonProductAPIWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PriceUpdateWebAPIBL
{
    interface IWebAPIArgs
    {
        void Validate();
    }

    [DataContract]
    public class GetProductInfoAPIArgs : IWebAPIArgs
    {
        public string ProductUrl { get; set; }

        public void Validate()
        {
            if(!Uri.IsWellFormedUriString(this.ProductUrl,UriKind.Absolute))
            {
                throw new ArgumentException("Product URI is invalid");
            }
        }

    }

    [DataContract]
    public class GetProductAPIArgs
    {
        [DataMember]
        public string ProductASIN { get; set; }
        [DataMember]
        public AmazonProductAPIContext.Regions ProductRegion { get; set; }

        public void Validate()
        {
            if (this.ProductASIN.Length != 10)
            {
                throw new ArgumentException("Product ASIN is invalid");
            }
        }
    }

    [DataContract]
    public class GetProductAPIContract
    {
        [DataMember]
        public string ProductASIN { get; set; }
        [DataMember]
        public string ProductName { get; set; }
        [DataMember]
        public AmazonProductAPIContext.Regions ProductRegion { get; set; }
        [DataMember]
        public string CurrentPrice { get; set; }
        [DataMember]
        public string CurrencyCode { get; set; }
        [DataMember]
        public string AmazonUrl { get; set; }
        [DataMember]
        public string EmailId { get; set; }
    }

    [DataContract]
    public class ActionSuccessAPIContract
    {
        [DataMember]
        string Action { get; set; }
        [DataMember]
        string Message { get; set; }
    }

    [DataContract]
    public class UpdateProductAPIArgs : IWebAPIArgs
    {
        [DataMember]
        public string EmailId { get; set; }
        [DataMember]
        public string ProductASIN { get; set; }
        [DataMember]
        public AmazonProductAPIContext.Regions ProductRegion { get; set; }
        [DataMember]
        public bool ToEmailEveryWeek { get; set; }
        [DataMember]
        public string DayToEmail { get; set; }
        [DataMember]
        public DateTime EmailEveryWeekDuration { get; set; }
        [DataMember]
        public bool ToEmailOnPrice { get; set; }
        [DataMember]
        public string PriceToEmail { get; set; }
        [DataMember]
        public DateTime EmailOnPriceDuration { get; set; }
        [DataMember]
        public bool ToEmailOnDate { get; set; }
        [DataMember]
        public DateTime DateToEmail { get; set; }
        [DataMember]
        public DateTime EmailOnDateDuration { get; set; }
        [DataMember]
        public string ProductPriceType { get; set; }

        public void Validate()
        {
            if (this.ProductASIN.Length != 10)
            {
                throw new ArgumentException("Product ASIN is invalid");
            }

            // Do other validations

        }

    }

}
