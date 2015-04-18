using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWSProjects.CommonUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProductUpdateCatalogProvider
{
    public class CatalogProviderFactory
    {
        private CatalogProviderFactoryArgs args;

        public CatalogProviderFactory(CatalogProviderFactoryArgs providerArgs)
        {
            this.args = providerArgs;
        }

        public bool InitializeCatalogs(List<string> tablesToInit)
        {
            bool isInitialized = false;
            try
            {
                CreateTableRequest createRequestProductTable = new CreateTableRequest
                {
                    TableName = CatalogConstants.RegisteredProductsTableName,
                    AttributeDefinitions = new List<AttributeDefinition>()
                            {
                                new AttributeDefinition
                                {
                                  AttributeName = "EmailId",
                                  AttributeType = "S"
                                },
                                new AttributeDefinition
                                {
                                  AttributeName = "ASIN",
                                  AttributeType = "S"
                                }
                            },

                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 10,
                        WriteCapacityUnits = 5
                    },
                    KeySchema = new List<KeySchemaElement>()
                            {
                                new KeySchemaElement
                                {AttributeName = "EmailId",KeyType = KeyType.HASH},
                                new KeySchemaElement
                                {AttributeName = "ASIN", KeyType = KeyType.RANGE}
                            }
                };

                this.CreateTable(createRequestProductTable);

                CreateTableRequest createRequestDoSTable = new CreateTableRequest
                {
                    TableName = CatalogConstants.DoSLimitTableName,
                    AttributeDefinitions = new List<AttributeDefinition>()
                            {
                                new AttributeDefinition
                                {
                                  AttributeName = "UserIPAddress",
                                  AttributeType = "S"
                                },
                            },

                    ProvisionedThroughput = new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 5,
                        WriteCapacityUnits = 5
                    },
                    KeySchema = new List<KeySchemaElement>()
                            {
                                new KeySchemaElement
                                {AttributeName = "UserIPAddress",KeyType = KeyType.HASH}
                            }
                };
                this.CreateTable(createRequestDoSTable);

            }
            catch (Exception ex)
            {
                DynamoDBTracer.Tracer.Write(string.Format("Init Catalog failed. Exception: ", ex));
            }

            return isInitialized;
        }

        public IProductCatalogProvider GetProductCatalogProvider()
        {
            IProductCatalogProvider catalogProvider = new ProductCatalogProvider(this.args);
            return catalogProvider;
        }

        public IDoSLimitCatalogProvider GetDoSLimitCatalogProvider()
        {
            IDoSLimitCatalogProvider dosCatalogProvider = new DoSLimitCatalogProvider(this.args);
            return dosCatalogProvider;
        }

        private void CreateTable(CreateTableRequest createRequest)
        {
            var createResponse = this.args.DynamoDBClient.CreateTable(createRequest);
            var response = this.args.DynamoDBClient.ListTables();
            if (response.TableNames.Contains(createRequest.TableName))
            {
                return;
            }

            while (true)
            {
                var describeResponse = this.args.DynamoDBClient.DescribeTable(new DescribeTableRequest { TableName = createRequest.TableName });
                if (describeResponse.HttpStatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception("Creation of table failed. Table:" + createRequest.TableName);
                }
                else if (describeResponse.Table.TableStatus == TableStatus.ACTIVE)
                {
                    break;
                }

                Thread.Sleep(30000);
            }
        }
    }

    public class CatalogProviderFactoryArgs
    {
        public AmazonDynamoDBClient DynamoDBClient;
    }
}
