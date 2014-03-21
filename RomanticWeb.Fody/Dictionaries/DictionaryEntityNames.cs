namespace RomanticWeb.Dynamic
{
    public abstract class DictionaryEntityNames
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

        public string OwnerTypeFullyQualifiedName
        {
            get
            {
                return string.Format("{0}.{1}, {2}",Namespace,OwnerTypeName,_assemblyName);
            }
        }

        public string EntryTypeFullyQualifiedName
        {
            get
            {
                return string.Format("{0}.{1}, {2}",Namespace,EntryTypeName,_assemblyName);
            }
        }

        public string OwnerTypeName
        {
            get
            {
                return string.Format("{0}_{1}_Owner",_entityTypeName,_propertyName);
            }
        }

        public string EntryTypeName
        {
            get
            {
                return string.Format("{0}_{1}_Entry",_entityTypeName,_propertyName);
            }
        }

        public string Namespace
        {
            get
            {
                return _ns;
            }
        }
    }
}