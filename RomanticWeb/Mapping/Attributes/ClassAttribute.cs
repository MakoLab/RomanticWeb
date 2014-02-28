using System;
using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a type to an RDF class.</summary>
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct,Inherited = true,AllowMultiple = true)]
    public sealed class ClassAttribute:MappingAttribute
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

        #region Internal methods

        /// <summary>Creates a class mapping using given mappingContext.</summary>
        /// <param name="mappingContext">Ontology to be used to resolve the prefix.</param>
        /// <returns>Class mapping or null.</returns>
        internal IClassMapping GetMapping(MappingContext mappingContext)
        {
            return new ClassMapping(GetTermUri(mappingContext));
        }
        #endregion
    }
}