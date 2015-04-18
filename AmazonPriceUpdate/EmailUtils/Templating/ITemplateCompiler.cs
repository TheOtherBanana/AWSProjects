using System.IO;

namespace EmailUtils.Templating
{
	public interface ITemplateCompiler
	{
		void CompileXsltFromFile(Stream inputXmlStream, string xsltFilePath, Stream outputStream);

		void CompileXsltFromStream(Stream inputXmlStream, Stream xsltStream, Stream outputStream);
	}
}