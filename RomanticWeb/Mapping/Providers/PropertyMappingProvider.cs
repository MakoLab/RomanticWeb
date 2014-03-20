using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public class PropertyMappingProvider:TermMappingProviderBase,IPropertyMappingProvider
    {
        private readonly PropertyInfo _property;

        public PropertyMappingProvider(Uri termUri,PropertyInfo property)
            :base(termUri)
        {
            _property=property;
        }

        public PropertyMappingProvider(string namespacePrefix,string termName,PropertyInfo property)
            :base(namespacePrefix,termName)
        {
            _property=property;
        }

        public PropertyInfo PropertyInfo
        {
            get
            {
                return _property;
            }
        }

        public virtual Aggregation? Aggregation
        {
            get
            {
                return Entities.ResultAggregations.Aggregation.SingleOrDefault;
            }
        }

        public override void Accept(IMappingProviderVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}