using System;

namespace ServiceStack.WebHost.Endpoints.Metadata
{
	/// <summary>
	///		Represents a documented object property on a DTO.
	/// </summary>
	public sealed class PropertyDoc: DocumentedCodeElementBase
	{
		/// <summary>
		/// 	<para>Gets or sets the type of this property.</para>
		/// </summary>
		/// <value>
		/// 	<para>A <see cref="String"/> providing the type of this property;
		///		never <see langword="null"/>.</para>
		/// </value>
		public string PropertyType { get; set; }
	}
}
