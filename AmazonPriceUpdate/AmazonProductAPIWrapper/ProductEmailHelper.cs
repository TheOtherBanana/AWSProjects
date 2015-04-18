using AmazonProductAPIWrapper;
using EmailUtils.Net;
using EmailUtils.Templating;
using EmailUtils.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Web.UI;
using System.Xml.Serialization;
using Amazon.S3;
using Amazon.S3.Model;

namespace AmazonProductAPIWrapper
{
    public class ProductEmailHelper
    {
        const string amazonURLWithTagFormat = "{0}/exec/obidos/redirect-home?tag={1}&placement=home_multi.gif&site=amazon";
        const string imageURL = "http://media.corporate-ir.net/media_files/IROL/97/97664/images/amazon_logo_RGB.jpg";
        public static string GetAmazonLogoAndLink(AmazonProductAPIContext.Regions region)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                string amazonEndpoint = AmazonProductAPIConstants.RegionPublicURLMapping[region];
                string associateTag = AmazonProductAPIContext.RegionAssociateIdMapping[region];
                string amazonURL = string.Format(amazonURLWithTagFormat, amazonEndpoint, associateTag);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, amazonURL);
                writer.RenderBeginTag(HtmlTextWriterTag.A);//Open A tag

                writer.AddAttribute(HtmlTextWriterAttribute.Src, imageURL);
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "200");
                writer.AddAttribute(HtmlTextWriterAttribute.Height, "200");
                writer.RenderBeginTag(HtmlTextWriterTag.Img); //Img open
                writer.RenderEndTag();//Img Close
                writer.RenderEndTag();//A Close

            }
            return stringWriter.ToString();
        }

        private static string GetLinkForProduct(string product, string link)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);//Open Li tag

                writer.AddAttribute(HtmlTextWriterAttribute.Href, link);
                writer.RenderBeginTag(HtmlTextWriterTag.A);//Open A tag
                writer.Write(product);
                
                writer.RenderEndTag();//Close A tag
                writer.RenderEndTag();//Close LI tag

            }

            return stringWriter.ToString();
        }
        public static MailMessage ProductHtmlMailContent(List<ProductEmailDetails> productsToEmail, AmazonProductAPIContext.Regions region)
        {
            //StringWriter stringWriter = new StringWriter();
            //using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            //{
            //    writer.RenderBeginTag(HtmlTextWriterTag.Html);//Html open
            //    writer.RenderBeginTag(HtmlTextWriterTag.Body);//Body open

            //    writer.Write(GetAmazonLogoAndLink(region));//Insert logo and link
                
            //    writer.RenderBeginTag(HtmlTextWriterTag.Ul);//Unordered list open

            //    foreach(var key in productsToMail.Keys)
            //    {
            //        string product = key;
            //        string link = productsToMail[key];
            //        var tempWriter = GetLinkForProduct(product, link);
            //        writer.Write(tempWriter);
            //    }
            //    writer.RenderEndTag();//Unorder List close
            //    writer.RenderEndTag();//Body Close
            //    writer.RenderEndTag();//Html Close
            //}

            //return stringWriter.ToString();
            // We'll dump the resulting HTML body in a file
            string status = string.Empty;;
            MailMessage mailMessage = null;
            try
            {
                status = "Entered try bloc";
                var templateCompiler = new TemplateCompiler();
                var xmlSerializer = new CustomXmlSerializer();

                var layoutFileName = "EmailLayout.html";
                var xslFileName = "EmailTemplate.xslt";

                status = "Getting files from S3;";
                var layoutFile = GetFileFromS3(layoutFileName);
                var xsltFilePath = GetFileFromS3(xslFileName);

                status = "Got files from S3";
                //var layoutFile = Path.Combine(templateDirectory, "EmailLayout.html");
                //var xsltFilePath = Path.Combine(templateDirectory, "EmailTemplate.xslt");

                var variables = new
                {
                    ProductsToEmail = productsToEmail.ToArray()
                };

                var templateEmailSender = new TemplateEmailSender(templateCompiler, xmlSerializer)
                {
                    LayoutFilePath = layoutFile
                };

                status = "Calling construct message";
                mailMessage  = templateEmailSender.ConstructMailMessage(xsltFilePath, variables);

                return mailMessage;
        
                // Close the file
            }
            catch (Exception ex)
            {
                throw new Exception("Send email failed with exception " + status + ex.Message,ex);
            }
       }

        private static string GetFileFromS3(string fileName)
        {
            var s3Client = new AmazonS3Client(AmazonProductAPIContext.Instance.AWS_ACCESS_KEY, AmazonProductAPIContext.Instance.AWS_SECRET_KEY,Amazon.RegionEndpoint.USWest2);
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = "email-template-files";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            request.Key = fileName;
            var response = s3Client.GetObject(request);
            response.WriteResponseStreamToFile(filePath);
            
            return filePath;
        }


    }

    [XmlRoot("productEmailDetails")]
    public class ProductEmailDetails
    {
        [XmlElement("productName")]
        public string ProductName { get; set; }
        [XmlElement("initialPrice")]
        public string InitialPrice { get; set; }
        [XmlElement("currentPrice")]
        public string CurrentPrice { get; set; }
        [XmlElement("productPurchaseLink")]
        public string ProductPurchaseLink { get; set; }
    }
}
