using AWSProjects.CommonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmazonProductAPIWrapper
{
    public class AmazonProductHelper
    {
        public static ItemLookupResponse GetItemDetails(string itemId, string region)
        {
            ItemLookupResponse itemResponse = null;

            string response = AmazonProductAPIContext.Instance.GetDetailsForItemId(itemId, region.ToString());
            itemResponse = GetObjectFromResponse<ItemLookupResponse>(response);

            return itemResponse;
        }

        public static ItemLookupResponse GetOffers(string itemId, string region)
        {
            ItemLookupResponse offersResponse = null;

            string response = AmazonProductAPIContext.Instance.GetOffersForItemId(itemId, region.ToString());


            offersResponse = GetObjectFromResponse<ItemLookupResponse>(response);

            return offersResponse;
        }

        public static CartCreateResponse GetCartDetails(string itemId, string region)
        {
            CartCreateResponse cartResponse = null;
            
            string response = AmazonProductAPIContext.Instance.GetCartDetails(itemId, region.ToString());

            cartResponse = GetObjectFromResponse<CartCreateResponse>(response);

            return cartResponse;
        }

        public static T GetObjectFromResponse<T>(string response)
        {
            response = response.Replace("xmlns=\"http://webservices.amazon.com/AWSECommerceService/2011-08-01\"", "xmlns=\"\"");
            T obj = XmlUtils.DeserializeStringToObject<T>(response);
            return obj;
        }
    }
}
