using System.IO;

namespace EmailUtils.Xml
{
	public interface IXmlSerializer
	{
		/// <summary>
		/// Generates an XML representation of an object of type <typeparam name="T" /> and returns a <see cref="Stream"/> containing the result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input">The input.</param>
		/// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
		/// <returns>A <see cref="Stream"/> with the resulting XML.</returns>
		void ToXml<T>(T input, Stream stream)
			where T : class;

		/// <summary>
		/// Generates an XML representation of an object of type <typeparam name="T" /> and returns a <see cref="Stream"/> containing the result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input">The input.</param>
		/// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
		/// <param name="element">The root element name.</param>
		/// <returns>A <see cref="Stream"/> with the resulting XML.</returns>
		void ToXml<T>(T input, Stream stream, string element)
			where T : class;
	}
}