using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using ProductUpdateCatalogProvider.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductUpdateCatalogProvider
{
    public class ProductCatalogProvider : IProductCatalogProvider
    {
        CatalogProviderFactoryArgs catalogArgs;
        DynamoDBContext dbContext;
        private static List<ScanCondition> isDeletedCondition = new List<ScanCondition> { new ScanCondition("IsDeleted", ScanOperator.Equal,false) };
        private static DynamoDBOperationConfig defaultConfig = new DynamoDBOperationConfig
                {
                    IgnoreNullValues = true,
                    QueryFilter = isDeletedCondition
                }; 
        public ProductCatalogProvider(CatalogProviderFactoryArgs _args)
        {
            this.catalogArgs = _args;
            this.dbContext = new DynamoDBContext(this.catalogArgs.DynamoDBClient);
        }

        public ProductCatalogEntity GetProduct(ProductIdArgs args)
        {
            var response = this.dbContext.Load<ProductCatalogEntity>(args.EmailId, args.ASIN);
            return response;
        }

        public ProductCatalogEntity[] GetProductsByEmailId(ProductIdArgs args)
        {
            var response = this.dbContext.Query<ProductCatalogEntity>(args.EmailId);
            return response.ToArray() ?? null;
        }

        public ProductCatalogEntity[] GetProducts()
        {
            var response = this.dbContext.Scan<ProductCatalogEntity>(isDeletedCondition.ToArray());
            return response.ToArray() ?? null;
        }
        public void UpdateProduct(ProductCatalogEntity entityToUpdate)
        {
            //Make version checks
            //Increment version
            this.dbContext.Save<ProductCatalogEntity>(entityToUpdate);
        }
        public void DeleteProduct(ProductIdArgs args)
        {

        }
        public void DeleteProductForEmailId(ProductIdArgs args)
        {

        }
    }

    public class DoSLimitCatalogProvider : IDoSLimitCatalogProvider
    {
        CatalogProviderFactoryArgs catalogArgs;
        DynamoDBContext dbContext;
        public DoSLimitCatalogProvider(CatalogProviderFactoryArgs _args)
        {
            this.catalogArgs = _args;
            this.dbContext = new DynamoDBContext(this.catalogArgs.DynamoDBClient);
        }

        public DoSLimitEntity GetLimit(DoSLimitArgs args)
        {
            //var response = this.dbContext.Query<DoSLimitEntity>(args.UserIP).FirstOrDefault();
            var response = this.dbContext.Load<DoSLimitEntity>(args.UserIP);
            return response;
        }

        public void UpdateLimit(DoSLimitEntity entityToUpdate)
        {
            this.dbContext.Save<DoSLimitEntity>(entityToUpdate);
        }

        public bool FlushCatalog()
        {
            bool isFlushed = false;
            try
            {
                //THINK OF BETTER WAY TO DELETE
                var deleteRequest = new DeleteTableRequest { TableName = CatalogConstants.DoSLimitTableName };
                var response = this.catalogArgs.DynamoDBClient.DeleteTable(deleteRequest);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    isFlushed = true;
            }
            catch
            {

            }
            return isFlushed;
        }
    }
}
