using Amazon.TraceListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSProjects.CommonUtils
{
    public static class DynamoDBTracer
    {
        private static DynamoDBTraceListener tracer;
        public static DynamoDBTraceListener Tracer
        {
            get
            {
                if (tracer == null)
                {
                    throw new ArgumentNullException("Tracer not initialized");
                }

                return tracer;
            }
        }

        public static void InitializeListener(string awsAccessKey, string awsSecretKey)
        {
            tracer = new DynamoDBTraceListener
            {
                Configuration = new DynamoDBTraceListener.Configs
                {
                    AWSAccessKey = awsAccessKey,
                    AWSSecretKey = awsSecretKey,
                    HashKeyFormat = "%ComputerName%-{EventType}",
                    CreateTableIfNotExist = true,
                    TableName = "ProductUpdateAPITracer"
                }

            };
            try
            {
                tracer.Write(DateTime.UtcNow, " Tracer Initialized Time");
            }
            catch
            {
                throw new Exception("Something went wrong." + awsAccessKey + awsSecretKey);
            }
        }
    }
}
