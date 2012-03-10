using System;
using System.Xml.Linq;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	/// <summary>
	///		The base class for code elements (classes and properties 
	///		in this case) that are XML-documented.
	/// </summary>
	public abstract class DocumentedCodeElementBase
	{
		/// <summary>
		/// 	<para>Gets or sets the name of the code element.
		///		For instance, if this instance represents a class, 
		///		it would be the name of the class.</para>
		/// </summary>
		/// <value>
		/// 	<para>A <see cref="String"/> providing the name of the 
		///		documented code element; never <see langword="null"/>.</para>
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// 	<para>Gets or sets the XML documentation attached to this 
		///		code element.</para>
		/// </summary>
		/// <value>
		/// 	<para>An <see cref="XElement"/> class providing the documentation;
		///		<see langword="null"/> if documentation is not available.</para>
		/// </value>
		public XElement XmlDocumentation { get; set; }
	}
}
