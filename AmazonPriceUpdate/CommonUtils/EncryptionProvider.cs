using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AWSProjects.CommonUtils
{
    public interface IEncryptionProvider
    {
        /// <summary>
        /// Encrypt the message
        /// </summary>
        /// <param name="msgToEncrypt"> Message to be encrypted</param>
        /// <returns></returns>
        string Encrypt(string msgToEncrypt);

        /// <summary>
        /// Decrypt message
        /// </summary>
        /// <param name="msgToDecrypt">Message to be decrypted</param>
        /// <returns></returns>
        string Decrypt(string msgToDecrypt);

    }
    public class EncryptionProvider : IEncryptionProvider
    {
        /// <summary>
        /// Certificate to be used for encryption. This can be null for decryption as EnvelopedCmsEncryption 
        /// will look in the My certificate store to read the correct certificate for decryption
        /// </summary>
        public X509Certificate2 RecipientCert { get; private set; }
        public EncryptionProvider(X509Certificate2 encryptionCertificate)
        {
            this.RecipientCert = encryptionCertificate;
        }

        #region IEncryptionProvider impl
        /// <summary>
        /// Encrypt to enveloped cms encryptes txt using x509 certificate
        /// </summary>
        /// <param name="msgToEncrypt"></param>
        /// <returns></returns>
        public string Encrypt(string msgToEncrypt)
        {
            // Validate the msg to be encrypted
            if (string.IsNullOrEmpty(msgToEncrypt))
            {
                DynamoDBTracer.Tracer.Write("Message to encrypt is null or empty");
                throw new ArgumentException(string.Format("Message to encrypt is null or empty"));
            }

            // Validate the certificate
            if (this.RecipientCert == null)
            {
                DynamoDBTracer.Tracer.Write("Recipient certificate for encrytion is null");
                throw new ArgumentException(string.Format(
                    "Recipient certificate for encrytion"));
            }

            // Convert message to an array of Unicode bytes for signing.
            UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] msgBytes = unicode.GetBytes(msgToEncrypt);

            // Encrypt using the certificate
            byte[] encodedEnvelopedCms = EncryptMsg(msgBytes, this.RecipientCert);
            string encodedMessage = EncryptionUtilities.ConvertToHexString(encodedEnvelopedCms);

            return encodedMessage;
        }

        /// <summary>
        /// Decrypts the enveloped encrypted message by selecting the correct certificate from My store
        /// </summary>
        /// <param name="msgToEncrypt"></param>
        /// <returns></returns>
        public string Decrypt(string msgToDecrypt)
        {
            if (string.IsNullOrEmpty(msgToDecrypt))
            {
                DynamoDBTracer.Tracer.Write("Message to decrypt is null or empty");
                throw new ArgumentException(string.Format(
                    "Message to decrypt={0} is null or empty", msgToDecrypt));
            }

            // Convert message to an array of bytes.
            byte[] encodedEnvelopedCms = EncryptionUtilities.ConvertHexStringToByteArray(msgToDecrypt);

            // Decrypt by reading certificate from My certificate store
            byte[] decryptedMsgBytes = DecryptMsg(encodedEnvelopedCms, false);

            // Convert the byte array to string
            UnicodeEncoding unicode = new UnicodeEncoding();
            string decryptedMsg = unicode.GetString(decryptedMsgBytes);
            return decryptedMsg;
        }
        #endregion

        #region Helper methods
        //  Decrypt the encoded EnvelopedCms message.
        static public byte[] DecryptMsg(byte[] encodedEnvelopedCms, bool useLocalCert)
        {
            //  Prepare object in which to decode and decrypt.
            EnvelopedCms envelopedCms = new EnvelopedCms();

            //  Decode the message.
            envelopedCms.Decode(encodedEnvelopedCms);

            //  Decrypt the message for the single recipient.
            envelopedCms.Decrypt(envelopedCms.RecipientInfos[0]);

            //  The decrypted message occupies the ContentInfo property
            //  after the Decrypt method is invoked.
            return envelopedCms.ContentInfo.Content;
        }

        //  Encrypt the message with the public key of
        //  the recipient. This is done by enveloping the message by
        //  using an EnvelopedCms object.
        static public byte[] EncryptMsg(
            Byte[] msg,
            X509Certificate2 recipientCert)
        {
            //  Place the message in a ContentInfo object.This is required to build an EnvelopedCms object.
            ContentInfo contentInfo = new ContentInfo(msg);

            //  Instantiate an EnvelopedCms object with the ContentInfo
            //  above.
            //  Has default SubjectIdentifierType IssuerAndSerialNumber property values    
            AlgorithmIdentifier algo = GetEncryptionAlgo();
            EnvelopedCms envelopedCms = new EnvelopedCms(contentInfo, algo);

            //  Create a CmsRecipient object that
            //  represents information about the recipient to encrypt the message for.
            CmsRecipient recip1 = new CmsRecipient(
                SubjectIdentifierType.IssuerAndSerialNumber,
                recipientCert);

            //  Encrypt the message for the recipient.
            envelopedCms.Encrypt(recip1);

            //  The encoded EnvelopedCms message contains the message
            //  ciphertext and the information about each recipient 
            //  that the message was enveloped for.
            return envelopedCms.Encode();
        }

        private static AlgorithmIdentifier GetEncryptionAlgo()
        {
            // List of OIDs = http://msdn.microsoft.com/en-us/library/windows/desktop/aa379070%28v=vs.85%29.aspx
            // AES256 OID string.(XCN_OID_NIST_AES256_CBC) 
            const string oidString = "2.16.840.1.101.3.4.1.42";
            AlgorithmIdentifier algo = new AlgorithmIdentifier(new Oid(oidString));
            return algo;
        }
        #endregion
    }
}
