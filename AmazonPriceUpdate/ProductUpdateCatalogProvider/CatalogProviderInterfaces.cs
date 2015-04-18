
using ProductUpdateCatalogProvider.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProductUpdateCatalogProvider
{
    [DataContract]
    public class ProductIdArgs
    {
        [DataMember]
        public string EmailId {get; set;}
        [DataMember]
        public string ASIN { get; set; }
    }

    [DataContract]
    public class DoSLimitArgs
    {
        [DataMember]
        public string EmailId { get; set; }
        [DataMember]
        public string UserIP { get; set; }
    }

    public interface IProductCatalogProvider
    {
        ProductCatalogEntity GetProduct(ProductIdArgs args); 
        ProductCatalogEntity[] GetProductsByEmailId(ProductIdArgs args);
        ProductCatalogEntity[] GetProducts();
        void UpdateProduct(ProductCatalogEntity entityToUpdate);
        void DeleteProduct(ProductIdArgs args);
        void DeleteProductForEmailId(ProductIdArgs args);
    }

    public interface IDoSLimitCatalogProvider
    {
        DoSLimitEntity GetLimit(DoSLimitArgs args);
        void UpdateLimit(DoSLimitEntity entityToUpdate);
        bool FlushCatalog();
    }

}
