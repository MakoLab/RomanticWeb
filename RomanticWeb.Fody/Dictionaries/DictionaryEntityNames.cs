namespace RomanticWeb.Dynamic
{
    /// <summary>Contains the type names of dictionary IEntities, which are used to access dictionary triples.</summary>
    internal abstract class DictionaryEntityNames
    {
        private readonly string _ns;
        private readonly string _entityTypeName;
        private readonly string _propertyName;
        private readonly string _assemblyName;

        protected DictionaryEntityNames(string @namespace,string entityTypeName,string propertyName,string assemblyName)
        {
            _ns=@namespace;
            _entityTypeName=entityTypeName;
            _propertyName=propertyName;
            _assemblyName=assemblyName;
        }

        /// <summary>Gets the fully qualified name of the dictionary owner type .</summary>
        public string OwnerTypeFullyQualifiedName { get { return string.Format("{0}.{1}, {2}",Namespace,OwnerTypeName,_assemblyName); } }

        /// <summary>Gets the fully qualified name of the dictionary entry type.</summary>
        public string EntryTypeFullyQualifiedName { get { return string.Format("{0}.{1}, {2}",Namespace,EntryTypeName,_assemblyName); } }

        /// <summary>Gets the name of the dictionary owner type.</summary>
        public string OwnerTypeName { get { return string.Format("{0}_{1}_Owner",_entityTypeName,_propertyName); } }

        /// <summary>Gets the name of the dictionary entry type.</summary>
        public string EntryTypeName { get { return string.Format("{0}_{1}_Entry",_entityTypeName,_propertyName); } }

        /// <summary>Gets the dictionary types' namespace.</summary>
        public string Namespace { get { return _ns; } }
    }
}