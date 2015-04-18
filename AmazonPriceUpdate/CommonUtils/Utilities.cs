using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AWSProjects.CommonUtils
{
    public class XmlUtils
    {
        public static string SerializeObjectToString<T>(T obj)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            string objString = string.Empty;

            using (StringWriter stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, obj);
                objString = stringWriter.ToString();
            }

            return objString;
        }

        public static T DeserializeStringToObject<T>(string objectString)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T obj;

            using(StringReader stringReader = new StringReader(objectString))
            {
                //XmlReader xmlReader = XmlReader.Create(stringReader);
                obj = (T)xmlSerializer.Deserialize(stringReader);
            }

            return obj;
        }
    }

    //TODO: Make it functional. 
    public class RestApiUtils
    {
        public static string ExecuteRestApi(string requestUri)
        {
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string responseBody = null;

            //get the response status 
            if (response != null)
            {
                //get the response body
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseBody = reader.ReadToEnd();
                }
            }

            return responseBody;

        }

        public static HttpWebResponse GetHttpResponse(string requestUri)
        {
            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            return response;
        }
    }

    public class EncryptionUtilities
    {
        /// <summary> 
        /// Converts a hex string to byte array 
        /// </summary> 
        /// <param name="input">The string</param> 
        /// <returns>The byte array</returns> 
        public static byte[] ConvertHexStringToByteArray(string strInput)
        {
            // i variable used to hold position in string
            int i = 0;

            // x variable used to hold byte array element position
            int x = 0;

            // allocate byte array based on half of string length    
            byte[] bytes = new byte[(strInput.Length) / 2];

            // Loop through the string - 2 bytes at a time converting
            // it to equivalent decimal value and store in byte array
            while (strInput.Length > i + 1)
            {
                long lngDecimal = Convert.ToInt32(strInput.Substring(i, 2), 16);
                bytes[x] = Convert.ToByte(lngDecimal);
                i = i + 2; ++x;
            }

            // Return the byte array
            return bytes;
        }

        /// <summary> 
        /// Converts a byte array to a hex string 
        /// </summary> 
        /// <param name="bytes">the byte array</param> 
        /// <returns>The string</returns> 
        public static string ConvertToHexString(byte[] bytes)
        {
            // convert the byte array to a true string
            string strTemp = "";
            for (int x = 0; x <= bytes.GetUpperBound(0); x++)
            {
                int number = int.Parse(bytes[x].ToString());
                strTemp += number.ToString("X").PadLeft(2, '0');
            }

            // return the finished string of hex values
            return strTemp;
        }

        public static X509Certificate2 GetEncryptionCertificateFromThumbprint(string thumbprint)
        {
            X509Certificate2 encCertificate = null;
            X509Store certStoreLocal = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            X509Store certStoreUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStoreLocal.Open(OpenFlags.ReadOnly);
            certStoreUser.Open(OpenFlags.ReadOnly);

            var certCollection = certStoreLocal.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            var certCollectionUser = certStoreUser.Certificates.Find(X509FindType.FindByThumbprint, thumbprint,false );
            if(certCollection.Count > 0)
            {
                encCertificate = certCollection[0];
            }
            else if(certCollectionUser.Count > 0)
            {
                encCertificate = certCollectionUser[0];
            }
            else 
            {
                throw new Exception("Can't find Encryption Certificate" + thumbprint);
            }
            return encCertificate;
        }
    }
}
