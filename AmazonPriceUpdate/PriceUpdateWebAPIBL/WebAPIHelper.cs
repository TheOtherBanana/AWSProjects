using ProductUpdateCatalogProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace PriceUpdateWebAPIBL
{
    public class WebAPIHelper
    {
        public static string GetConfirmationMail(string confirmationLink)
        {
            string clickToConfirm = "Click here to confirm";
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Html);//Html open
                writer.RenderBeginTag(HtmlTextWriterTag.Body);//Body open

                writer.AddAttribute(HtmlTextWriterAttribute.Href, confirmationLink);
                writer.RenderBeginTag(HtmlTextWriterTag.A);//Open A tag
                writer.Write(clickToConfirm);

                writer.RenderEndTag();//Close A tag
                writer.RenderEndTag();//Body Close
                writer.RenderEndTag();//Html Close
            }
            return stringWriter.ToString();
        }
    }
}
