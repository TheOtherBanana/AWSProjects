using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace EmailUtils.Xml
{
	public class CustomXmlSerializer : IXmlSerializer
	{
		private const string DefaultElementName = "object";

		/// <summary>
		/// Converts an object of type <typeparam name="T" /> to an XElement.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns>Returns the object as it's XML representation in an XElement.</returns>
		public XElement ToXElement<T>(T input)
			where T : class
		{
			return ToXElement(input, null);
		}

		/// <summary>
		/// Converts an object of type <typeparam name="T" /> to an XElement.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="element">The element name.</param>
		/// <returns>Returns the object as it's XML representation in an XElement.</returns>
		public XElement ToXElement<T>(T input, string element)
			where T : class
		{
			return ToXElementInternal(input, element);
		}

		/// <summary>
		/// Generates an XML representation of an object of type <typeparam name="T" /> and returns a <see cref="Stream"/> containing the result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input">The input.</param>
		/// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
		/// <returns>A <see cref="Stream"/> with the resulting XML.</returns>
		public void ToXml<T>(T input, Stream stream)
			where T : class
		{
			ToXml(input, stream, null);
		}

		/// <summary>
		/// Generates an XML representation of an object of type <typeparam name="T" /> and returns a <see cref="Stream"/> containing the result.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input">The input.</param>
		/// <param name="stream">The <see cref="Stream"/> used to write the XML document.</param>
		/// <param name="element">The root element name.</param>
		/// <returns>A <see cref="Stream"/> with the resulting XML.</returns>
		public void ToXml<T>(T input, Stream stream, string element)
			where T : class
		{
			if (input == null) throw new ArgumentNullException("input");
			if (stream == null) throw new ArgumentNullException("stream");
			var xmlElement = ToXElement(input, element);

			if (xmlElement != null)
			{
				xmlElement.Save(stream);
			}
			//stream.Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		/// Gets the array element.
		/// </summary>
		/// <param name="input">The input object.</param>
		/// <param name="element">The element name.</param>
		/// <returns>Returns an XElement with the array collection as child elements.</returns>
		private XElement SerializeEnumerable(IEnumerable input, string element)
		{
			var rootElement = new XElement(element);

			var enumerator = input.GetEnumerator();
			while (enumerator.MoveNext())
			{
				var value = enumerator.Current;
				XElement childElement = TypeHelper.IsSimpleOrNullableType(value.GetType()) ? new XElement(element + "Item", value) : ToXElementInternal(value, element + "Item");

				rootElement.Add(childElement);
			}

			return rootElement;
		}

		private XElement ToXElementInternal(object input, string element)
		{
			if (input == null)
			{
				return null;
			}

			if (string.IsNullOrWhiteSpace(element))
			{
				element = DefaultElementName;
			}

			element = XmlConvert.EncodeName(element);

			var enumerable = input as IEnumerable;
			if (enumerable != null)
			{
				return SerializeEnumerable(enumerable, element);
			}

			var xElement = new XElement(element);

			var props = GetProperties(input);
			var elements = props
				.Where(x => x.Value != null)
				.Select(x => TypeHelper.IsSimpleOrNullableType(x.Value.GetType())
								? new XElement(x.Key, x.Value)
								: ToXElementInternal(x.Value, x.Key));

			xElement.Add(elements);

			return xElement;
		}

		private static IEnumerable<KeyValuePair<string, object>> GetProperties(object input)
		{
			var type = input.GetType();

			return type.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
				.ToDictionary(x => x.Name, x => x.GetValue(input, null));
		}
	}
}