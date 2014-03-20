using System;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a type to an RDF class.</summary>
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct,Inherited = true,AllowMultiple = true)]
    public sealed class ClassAttribute : TermMappingAttribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAttribute"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="className">Name of the class.</param>
        public ClassAttribute(string prefix,string className):base(prefix,className)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAttribute"/> class.
        /// </summary>
        /// <param name="classUri">The URI.</param>
        public ClassAttribute(string classUri):base(classUri)
        {
        }
        #endregion

        #region Public methods
        public IClassMappingProvider Accept(IMappingAttributesVisitor visitor)
        {
            return visitor.Visit(this);
        }
        #endregion
    }
}