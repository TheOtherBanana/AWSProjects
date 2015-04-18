using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace EmailUtils.Templating
{
	public class TemplateCompiler : ITemplateCompiler
	{
		private readonly Dictionary<string, XslCompiledTransform> _cache;

		public TemplateCompiler()
		{
			_cache = new Dictionary<string, XslCompiledTransform>(StringComparer.CurrentCultureIgnoreCase);
		}

		public void CompileXsltFromFile(Stream inputXmlStream, string xsltFilePath, Stream outputStream)
		{
			if (inputXmlStream == null) throw new ArgumentNullException("inputXmlStream");
			if (xsltFilePath == null) throw new ArgumentNullException("xsltFilePath");
			if (outputStream == null) throw new ArgumentNullException("outputStream");

			var transform = GetTransform(xsltFilePath);

			CompileXslt(inputXmlStream, transform, outputStream);
		}

		public void CompileXsltFromStream(Stream inputXmlStream, Stream xsltStream, Stream outputStream)
		{
			if (inputXmlStream == null) throw new ArgumentNullException("inputXmlStream");
			if (xsltStream == null) throw new ArgumentNullException("xsltStream");
			if (outputStream == null) throw new ArgumentNullException("outputStream");

			var transform = GetTransform(xsltStream);

			CompileXslt(inputXmlStream, transform, outputStream);
		}

		private static void CompileXslt(Stream inputXmlStream, XslCompiledTransform transform, Stream outputStream)
		{
			inputXmlStream.Seek(0, SeekOrigin.Begin);
			using (XmlReader reader = XmlReader.Create(inputXmlStream))
			{
				transform.Transform(reader, null, outputStream);
			}
		}

		private static XslCompiledTransform GetTransform(Stream stream)
		{
			using (var streamReader = new StreamReader(stream))
			{
				return GetTransform(streamReader);
			}
		}

		private static XslCompiledTransform GetTransform(TextReader reader)
		{
			var transform = new XslCompiledTransform();
			using (var xmlReader = XmlReader.Create(reader))
			{
				transform.Load(xmlReader);
			}
			return transform;
		}

		private XslCompiledTransform GetTransform(string xsltFilePath)
		{
			lock (_cache)
			{
				if (_cache.ContainsKey(xsltFilePath))
				{
					return _cache[xsltFilePath];
				}

				if (!File.Exists(xsltFilePath))
				{
					throw new FileNotFoundException("XSLT file not found.", xsltFilePath);
				}

				XslCompiledTransform transform;
				using (var streamReader = new StreamReader(xsltFilePath))
				{
					transform = GetTransform(streamReader);
				}

				_cache[xsltFilePath] = transform;
				return transform;
			}
		}
	}
}