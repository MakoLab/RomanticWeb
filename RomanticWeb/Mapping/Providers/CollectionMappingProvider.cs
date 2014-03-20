using System;
using System.Reflection;
using RomanticWeb.Entities.ResultAggregations;
using RomanticWeb.Mapping.Model;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Providers
{
    public class CollectionMappingProvider:PropertyMappingProvider,ICollectionMappingProvider
    {
        private StoreAs _storeAs;
        private Aggregation? _aggregation;

        public CollectionMappingProvider(Uri termUri,StoreAs storeAs,PropertyInfo property)
            :base(termUri,property)
        {
            ((ICollectionMappingProvider)this).StoreAs = storeAs;
        }

        public CollectionMappingProvider(string namespacePrefix,string termName,StoreAs storeAs,PropertyInfo property)
            :base(namespacePrefix,termName,property)
        {
            ((ICollectionMappingProvider)this).StoreAs=storeAs;
        }

        /// <summary>
        /// Gets or sets the storage strategy
        /// </summary>
        StoreAs ICollectionMappingProvider.StoreAs
        {
            get
            {
                return _storeAs;
            }

            set
            {
                switch (value)
                {
                    case StoreAs.SimpleCollection:
                        _aggregation=Entities.ResultAggregations.Aggregation.Original;
                        break;
                    case StoreAs.RdfList:
                        _aggregation=Entities.ResultAggregations.Aggregation.SingleOrDefault;
                        break;
                    default:
                        _aggregation=null;
                        break;
                }

                _storeAs=value;
            }
        }

        public override Aggregation? Aggregation
        {
            get
            {
                return _aggregation;
            }
        }

        public override void Accept(IMappingProviderVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}