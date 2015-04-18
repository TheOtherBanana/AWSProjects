using AWSProjects.CommonUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AmazonPriceUpdateWebAPI
{
    public class APIConfigManager
    {
        public const string PrmAWSAccessKey = "AWS_ACCESS_KEY_ID";
        public const string PrmAWSSecretKey = "AWS_SECRET_KEY";
        public const string PrmServiceAcctId = "ServiceAccountId";
        public const string PrmServiceAcctPwd = "PARAM1";
        public const string PrmAdminAcctId = "AdminAccountId";
        public const string PrmTablesToCreate = "TablesToCreate";
        public const string PrmBaseRequestUri = "BaseRequestUri";
        public const string PrmEncryptionCertificateThumbprint = "EncryptionCertificateThumbprint";
        public const string PrmAlchemyApiKey = "AlchemyApiPrivateKey";
        public const string PrmImgurClientId = "ImgurClientId";
        public static string AWSAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAWSAccessKey];        
            }
        }

        public static string AWSSecretKey
        {
            get
            {
                string encryptedKey = ConfigurationManager.AppSettings[PrmAWSSecretKey];
                string thumbprint = EncryptionCertificateThumbprint;
                return ConfigDecryptionHelper.DecryptConfiguration(encryptedKey,thumbprint);
            }
        }

        public static string ServiceAccountId
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmServiceAcctId];
            }
        }

        public static string ServiceAccountPassword
        {
            get
            {
                string encryptedPwd = ConfigurationManager.AppSettings[PrmServiceAcctPwd];
                string thumbprint = EncryptionCertificateThumbprint;
                return ConfigDecryptionHelper.DecryptConfiguration(encryptedPwd, thumbprint);   
            }
        }

        public static string AdminAccountId
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAdminAcctId];
            }
        }

        public static List<string> TablesToCreate
        {
            get
            {
                string tablesString = ConfigurationManager.AppSettings[PrmTablesToCreate];
                var tables = tablesString.Split(',').ToList();
                return tables;
            }
        }

        public static string BaseRequestUri
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmBaseRequestUri];
            }
        }

        public static string EncryptionCertificateThumbprint
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmEncryptionCertificateThumbprint];
            }
        }

        public static string AlchemyApiPricateKey
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmAlchemyApiKey];
            }
        }

        public static string ImgurClientId
        {
            get
            {
                return ConfigurationManager.AppSettings[PrmImgurClientId];
            }
        }
    }

    public static class ConfigDecryptionHelper
    {
        /// <summary>
        /// Decrypts message if the cert is present else returns raw string
        /// </summary>
        /// <param name="msgToDecrypt"></param>
        /// <returns></returns>
        public static string DecryptConfiguration(string msgToDecrypt, string thumbprint)
        {
            EncryptionProvider encryptionProvider = new EncryptionProvider(null); 
            string decryptedMsg = msgToDecrypt;
            try
            {
                decryptedMsg = encryptionProvider.Decrypt(msgToDecrypt);
            }
            catch
            {

            }
            return decryptedMsg;
        }
    }
}