using EmailUtils.Templating;
using EmailUtils.Xml;
using System;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailUtils.Net
{
	public class TemplateEmailSender : ITemplateEmailSender
	{
		#region Fields

		private readonly ITemplateCompiler _templateCompiler;
        private readonly Regex _replaceRegex = new Regex(Regex.Escape("<!-- Content -->"), RegexOptions.Compiled);
		private readonly IXmlSerializer _xmlSerializer;
		private string _layoutHtml;

		#endregion Fields

		public string LayoutFilePath { get; set; }

		protected string LayoutHtml
		{
			get
			{
				lock (this)
				{
					if (LayoutFilePath == null)
					{
						return null;
					}

					return _layoutHtml ?? (_layoutHtml = GetBaseHtmlContent());
				}
			}
		}

		#region Constructor

		public TemplateEmailSender(ITemplateCompiler templateCompiler, IXmlSerializer xmlSerializer)
		{
			if (templateCompiler == null) throw new ArgumentNullException("templateCompiler");
			if (xmlSerializer == null) throw new ArgumentNullException("xmlSerializer");

			_templateCompiler = templateCompiler;
			_xmlSerializer = xmlSerializer;
		}

		#endregion Constructor

		#region Methods
        
		public MailMessage ConstructMailMessage(string templatePath, object variables)
		{
			if (templatePath == null) throw new ArgumentNullException("templatePath");
			
			if (!File.Exists(templatePath))
			{
				throw new FileNotFoundException("Template file not found.", templatePath);
			}

			var compiler = _templateCompiler;
			string compiled;
			using (var xmlStream = new MemoryStream())
			{
				_xmlSerializer.ToXml(variables, xmlStream);

                xmlStream.Position = 0;
                var sr = new StreamReader(xmlStream);
                string myStr = sr.ReadToEnd();

				using (var output = new MemoryStream())
				{
					compiler.CompileXsltFromFile(xmlStream, templatePath, output);

					output.Seek(0, SeekOrigin.Begin);
					using (var reader = new StreamReader(output))
					{
						compiled = reader.ReadToEnd();
					}
				}
			}

			string layoutHtml = LayoutHtml;
			if (layoutHtml != null)
			{
				compiled = _replaceRegex.Replace(layoutHtml, compiled);
			}

            compiled = compiled.Replace("&lt;", "<").Replace("&gt;", ">");
			var mailMessage = new MailMessage
			{
				IsBodyHtml = true,
				BodyEncoding = System.Text.Encoding.UTF8,
				Body = compiled
			};
			
			return mailMessage;
		}

		private string GetBaseHtmlContent()
		{
			string filePath = LayoutFilePath;
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("Layout file not found.", filePath);
			}

			using (var reader = new StreamReader(filePath))
			{
				return reader.ReadToEnd();
			}
		}

		#endregion Methods
	}
}