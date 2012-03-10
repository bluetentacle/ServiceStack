using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ServiceStack.WebHost.Endpoints.Metadata;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ServiceStack.WebHost.Endpoints.Support.Metadata.Controls
{
	/// <summary>
	///		Provides extension methods to render XML documentation into HTML.
	/// </summary>
	internal static class XmlDocumentationRenderer
	{
		/// <summary>
		/// 	<para>Renders the XML documentation to HTML in a <see cref="String"/>.</para>
		/// </summary>
		/// <param name="codeElement">
		/// 	<para>The code element object whose XML documentation is to be rendered.  Required.</para>
		/// </param>
		/// <param name="elementName">
		/// 	<para>If specified, only the first-level XML documentation element by this name will be rendered, 
		///		without an enclosing tag (the caller would provide that).  Optional.  If unspecified, all
		///		XML documentation will be rendered.</para>
		/// </param>
		/// <param name="sectionTitleTag">
		/// 	<para>The tag to use for rendering the header of each section in the XML documentation.  
		///		Optional.  If unspecified, "h1" is used.  Not applicable if <paramref name="elementName"/>
		///		is <see langword="true"/> since the header is not rendered in that case.</para>
		/// </param>
		/// <param name="sectionTitleCssClass">
		/// 	<para>The CSS class to use for rendering the header of each section in the XML documentation.  
		///		Optional.  Not applicable if <paramref name="elementName"/>
		///		is <see langword="true"/> since the header is not rendered in that case.</para>
		/// </param>
		/// <param name="noIndentation">
		///		<para><see langword="true"/> if the output string has no indentation; otherwise, <see langword="false"/>.
		///		The default is <see langword="false"/>.</para>
		/// </param>
		/// <param name="suppressSummaryHeader">
		///		<see langword="true"/> to suppress the rendering of the header for the "summary" first-level element,
		///		<see langword="false"/> to render the header like all other first-level elements.  The default
		///		value is <see langword="false"/>.
		/// </param>
		/// <returns>
		///		A <see cref="String"/> containing the rendered HTML; never <see langword="null"/>.
		/// </returns>
		public static string GetRenderedDocumentation(this DocumentedCodeElementBase codeElement, 
			string elementName = null, 
			string sectionTitleTag = null, 
			string sectionTitleCssClass = null, 
			bool noIndentation = false, 
			bool suppressSummaryHeader = false)
		{
			if (codeElement.XmlDocumentation == null) return String.Empty;

			var builder = new StringBuilder();
			var stringWriter = new StringWriter(builder);
			var writer = new HtmlTextWriter(stringWriter, noIndentation ? String.Empty : "\t");

			writer.Indent = 0;

			if (noIndentation)
			{
				writer.NewLine = String.Empty;
			}

			codeElement.RenderDocumentation(writer, elementName, sectionTitleTag, sectionTitleCssClass, suppressSummaryHeader);

			return builder.ToString();
		}

		/// <summary>
		/// 	<para>Renders the XML documentation to HTML with thespecified <see cref="HtmlTextWriter"/>.</para>
		/// </summary>
		/// <param name="codeElement">
		/// 	<para>The code element object whose XML documentation is to be rendered.  Required.</para>
		/// </param>
		/// <param name="writer">
		/// 	<para>The <see cref="HtmlTextWriter"/> to which the HTML is to be rendered.  Required.</para>
		/// </param>
		/// <param name="elementName">
		/// 	<para>If specified, only the first-level XML documentation element by this name will be rendered, 
		///		without an enclosing tag (the caller would provide that).  Optional.  If unspecified, all
		///		XML documentation will be rendered.</para>
		/// </param>
		/// <param name="sectionTitleTag">
		/// 	<para>The tag to use for rendering the header of each section in the XML documentation.  
		///		Optional.  If unspecified, "h1" is used.  Not applicable if <paramref name="elementName"/>
		///		is <see langword="true"/> since the header is not rendered in that case.</para>
		/// </param>
		/// <param name="sectionTitleCssClass">
		/// 	<para>The CSS class to use for rendering the header of each section in the XML documentation.  
		///		Optional.  Not applicable if <paramref name="elementName"/>
		///		is <see langword="true"/> since the header is not rendered in that case.</para>
		/// </param>
		/// <param name="suppressSummaryHeader">
		///		<see langword="true"/> to suppress the rendering of the header for the "summary" first-level element,
		///		<see langword="false"/> to render the header like all other first-level elements.  The default
		///		value is <see langword="false"/>.
		/// </param>
		public static void RenderDocumentation(this DocumentedCodeElementBase codeElement, 
			HtmlTextWriter writer, 
			string elementName = null, 
			string sectionTitleTag = null, 
			string sectionTitleCssClass = null,
			bool suppressSummaryHeader = false)
		{
			if (codeElement.XmlDocumentation == null) return;

			foreach (var firstLevelElement in codeElement.XmlDocumentation.Elements())
			{
				if (elementName != null)
				{
					if (firstLevelElement.Name == elementName)
					{
						Render(writer, firstLevelElement);
						break;
					}
				}
				else
				{
					if (!suppressSummaryHeader || firstLevelElement.Name.LocalName != "summary")
					{
						if (sectionTitleCssClass != null)
						{
							writer.AddAttribute("class", sectionTitleCssClass);
						}

						writer.RenderBeginTag(sectionTitleTag ?? "h1");
						writer.WriteEncodedText(Capitalize(firstLevelElement.Name.LocalName));
						writer.RenderEndTag();
					}
				
					Render(writer, firstLevelElement);
				}
			}
		}

		private static void Render(HtmlTextWriter writer, XNode node, bool isInTable = false)
		{
			var element = node as XElement;
			var text = node as XText;

			if (text != null)
			{
				writer.WriteEncodedText(text.Value); // Finally--text!
			}
			else if (element != null)
			{
				if (GetListType(element.Parent) == ListType.Table)
				{
					isInTable = true;
				}

				var htmlTags = GetEnclosingTagsForElement(element, isInTable);

				foreach (var tag in htmlTags)
				{
					writer.RenderBeginTag(tag);
				}

				if (element.IsEmpty)
				{
					if ((element.Name == "see"))
					{
						var cref = element.Attribute("cref");
						var langword = element.Attribute("langword");
						if (cref != null && !String.IsNullOrEmpty(cref.Value))
						{
							var translatedCref = TranslateCref(cref.Value);
							writer.WriteEncodedText(translatedCref);
						}
						else if (langword != null && !String.IsNullOrEmpty(langword.Value))
						{
							writer.WriteEncodedText(langword.Value);
						}
					}
					else if ((element.Name == "paramref" 
						|| element.Name == "typeparamref"))
					{
						var word = element.Attribute("name");
						if (word != null)
						{
							writer.WriteEncodedText(word.Value);
						}
					}
				}
				else
				{
					foreach (var child in element.Nodes())
					{
						Render(writer, child, isInTable);
					}
				}

				if (element.Name == "term" && !isInTable)
				{
					writer.WriteEncodedText(": ");
				}

				foreach (var tag in htmlTags)
				{
					writer.RenderEndTag();
				}
			}
		}

		private static string TranslateCref(string cref)
		{
			// Strip to the last segment.
			return _crefLastSegment.Match(cref).Value;
		}

		private static Regex _crefLastSegment = new Regex(@"\w+$");

		private static List<string> GetEnclosingTagsForElement(XElement element, bool isInTable)
		{
			List<string> htmlTags = new List<string>(2);

			switch (element.Name.LocalName)
			{
				case "para":
					htmlTags.Add("p");
					break;
				case "list":
					switch (GetListType(element))
					{
						case ListType.Number:
							htmlTags.Add("ol");
							break;
						case ListType.Table:
							htmlTags.Add("table");
							break;
						default:
							htmlTags.Add("ul");
							break;
					}
					break;
				case "listheader":
				case "item":
					if (isInTable)
					{
						htmlTags.Add("tr");

						if (element.XPathSelectElement("term") == null
						    && element.XPathSelectElement("description") == null)
						{
							// Only one column.
							if (element.Name == "listheader")
							{
								htmlTags.Add("th");
							}
							else
							{
								htmlTags.Add("td");
							}
						}
					}
					else
					{
						htmlTags.Add("li");
					}
					break;
				case "term":
					if (isInTable && element.Parent != null && element.Parent.Name == "listheader")
					{
						htmlTags.Add("th");
					}
					else if (isInTable && element.Parent != null && element.Parent.Name == "item")
					{
						htmlTags.Add("td");
					}
					else
					{
						htmlTags.Add("strong");
					}
					break;
				case "description":
					if (isInTable && element.Parent != null && element.Parent.Name == "listheader")
					{
						htmlTags.Add("th");
					}
					else if (isInTable && element.Parent != null && element.Parent.Name == "item")
					{
						htmlTags.Add("td");
					}
					break;
				case "see":
				case "paramref":
				case "typeparamref":
					htmlTags.Add("strong");
					break;
				case "c":
					htmlTags.Add("code");
					break;
				case "code":
					htmlTags.Add("pre");
					break;
				default:
					// By default, the XML tag is stripped and only the contents are rendered.
					break;
			}

			return htmlTags;
		}

		private static ListType? GetListType(XElement listElement)
		{
			if (listElement == null) return null;

			var typeAttribute = listElement.Attribute("type");

			if (typeAttribute != null)
			{
				switch (typeAttribute.Value)
				{
					case "number":
						return ListType.Number;
					case "table":
						return ListType.Table;
					default:
						break;
				}
			}

			return ListType.Bullet;
		}

		private static string Capitalize(string str)
		{
			var builder = new StringBuilder(str.Length);

			if (str.Length > 0)
			{
				builder.Append(str.Substring(0, 1).ToUpper());
			}

			if (str.Length > 1)
			{
				builder.Append(str.Substring(1, str.Length - 1));
			}

			return builder.ToString();
		}

		private enum ListType
		{
			Bullet,
			Number,
			Table
		}
	}
}
