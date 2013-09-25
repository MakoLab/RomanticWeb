using System.Diagnostics;

namespace RomanticWeb.Ontologies
{
	/// <summary>
	/// Represents an RDF class
	/// </summary>
	[DebuggerDisplay("Class {Prefix}:{ClassName}")]
	public class Class : Term
	{
		/// <summary>
		/// Creates a new instance of <see cref="Class"/>
		/// </summary>
		/// <param name="className"></param>
		public Class(string className)
			: base(className)
		{
		}

		/// <summary>
		/// Gets the class name
		/// </summary>
		/// <remarks>See remarks under <see cref="Term.TermName"/></remarks>
		public string ClassName { get { return TermName; } }
	}
}