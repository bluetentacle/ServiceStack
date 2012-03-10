using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Xml.XPath;
using System.Reflection;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	/// <summary>
	///		Represents a documented ServiceStack operation.
	/// </summary>
	public sealed class OperationDoc: DocumentedCodeElementBase
	{
		private const string ResponseSuffix = "Response";
		private static Dictionary<Type, OperationDoc> _cache = new Dictionary<Type, OperationDoc>();

		/// <summary>
		/// 	<para>Gets documentation for the specified request DTO type.</para>
		/// </summary>
		/// <param name="requestType">
		///		The type of the request object for which to locate documentation.
		/// </param>
		/// <returns>
		///		An <see cref="OperationDoc"/> object for the specified request DTO type;
		///		never <see langword="null"/>.
		/// </returns>
		internal static OperationDoc GetForRequestType(Type requestType)
		{
			lock(_cache)
			{
				OperationDoc doc;

				if (!_cache.TryGetValue(requestType, out doc))
				{
					doc = new OperationDoc(requestType);
					_cache.Add(requestType, doc);
				}

				return doc;
			}
		}

		private OperationDoc(Type requestType)
		{
			this.Name = requestType.Name;

			var xmlDocument = FindXmlDocumentationFile(requestType);

			Request = new DtoDoc
			          	{
			          		Name = requestType.Name,
			          		XmlDocumentation = xmlDocument != null ? ExtractMemberElement(xmlDocument, "T", requestType.FullName) : null,
			          	};

			this.XmlDocumentation = Request.XmlDocumentation;

			PopulateProperties(Request, requestType, xmlDocument);

			Responses = new List<DtoDoc>();

			// Support for multiple response types may appear in the future.
			// For now, append suffix to find the single response DTO.
			var responseType = requestType.Assembly.GetType(requestType.FullName + ResponseSuffix, false);

			if (responseType != null)
			{
				var response = new DtoDoc
				               	{
				               		Name = responseType.Name,
				               		XmlDocumentation =
										xmlDocument != null ? ExtractMemberElement(xmlDocument, "T", responseType.FullName) : null,
				               	};

				PopulateProperties(response, responseType, xmlDocument);

				Responses.Add(response);
			}
		}

		private void PopulateProperties(DtoDoc dto, Type requestType, XDocument xmlDocument)
		{
			foreach (var propertyInfo in requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				var propertyDoc = new PropertyDoc
				                  	{
				                  		Name = propertyInfo.Name,
										PropertyType = propertyInfo.PropertyType.Name,
				                  		XmlDocumentation =
				                  			xmlDocument != null ? ExtractMemberElement(xmlDocument, "P", requestType.FullName + "." + propertyInfo.Name) : null
				                  	};

				dto.Properties.Add(propertyDoc);
			}

			dto.Properties.Sort((x, y) => x.Name.CompareTo(y.Name));
		}

		private XDocument FindXmlDocumentationFile(Type requestType)
		{
			foreach (var dllLocation in new [] {requestType.Assembly.Location, new Uri(requestType.Assembly.CodeBase).AbsolutePath})
			{
				if (!File.Exists(dllLocation)) continue;

				var xmlLocation = Path.ChangeExtension(dllLocation, "xml");

				if (!File.Exists(xmlLocation)) continue;

				var doc = XDocument.Load(xmlLocation);

				return doc;
			}

			return null;
		}

		private XElement ExtractMemberElement(XDocument doc, string prefix, string elementName)
		{
			var xpath = string.Format("/doc/members/member[@name=\"{0}:{1}\"]", prefix, elementName);

			var element = doc.XPathSelectElement(xpath);

			if (element != null)
			{
				// Strip out stuff that is not relevant.
				foreach (var child in new List<XElement>(element.Elements()))
				{
					if (child.Name != "summary"
						&& child.Name != "remarks"
						&& child.Name != "value"
						&& child.Name != "returns")
					{
						child.Remove();
					}
				}
			}

			return element;
		}

		/// <summary>
		/// 	<para>Gets the documented request DTO.</para>
		/// </summary>
		/// <value>
		/// 	<para>A <see cref="DtoDoc"/> instance containing documentation
		///		for the request DTO; never <see langword="null"/>.</para>
		/// </value>
		public DtoDoc Request { get; private set; }

		/// <summary>
		///		<para>Gets the documented response DTOs.</para>
		/// </summary>
		/// <value>
		///		A collection of <see cref="DtoDoc"/> instances containing documentation 
		///		for all possible response DTOs; <see langword="null"/> if the response
		///		DTO cannot be identified.
		/// </value>
		public IList<DtoDoc> Responses { get; private set; }
	}
}
