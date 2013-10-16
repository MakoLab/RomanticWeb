using System;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Mapping.Attributes
{
    /// <summary>Maps a type to an RDF class.</summary>
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Class|AttributeTargets.Struct)]
    public sealed class ClassAttribute:MappingAttribute
    {
        #region Fields
        private readonly string _className;
        #endregion

        #region Constructors
        /// <summary>Default constructor with namespace prefix and class name passed.</summary>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="className">Name of the class.</param>
        public ClassAttribute(string prefix,string className):base(prefix)
        {
            _className=className;
        }
        #endregion

        #region Properties
        /// <summary>Gets a class name.</summary>
        public string ClassName { get { return _className; } }
        #endregion

        #region Internal methods
        /// <summary>Creates a class mapping using given ontology.</summary>
        /// <param name="ontology">Ontology to be used to resolve the prefix.</param>
        /// <returns>Class mapping or null.</returns>
        internal IClassMapping GetMapping(IOntologyProvider ontology)
        {
            Uri uri=ontology.ResolveUri(Prefix,ClassName);
            if (uri!=null)
            {
                return new ClassMapping(uri);
            }

            throw new MappingException(string.Format("Cannot resolve class {0}:{1}",Prefix,ClassName));
        }
        #endregion
    }
}