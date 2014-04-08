using System;
using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Model
{
    internal class MultiMapping:EntityMapping,IEntityMapping
    {
        public MultiMapping(params IEntityMapping[] mappings)
            :base(
            mappings.SelectMany(m=>m.Classes),
            mappings.SelectMany(m=>m.Properties).Distinct(new PropertyMappingComparer()))
        {
        }

        IPropertyMapping IEntityMapping.PropertyFor(string propertyName)
        {
            try
            {
                return PropertyFor(propertyName);
            }
            catch (InvalidOperationException)
            {
                throw new AmbiguousPropertyException(propertyName);
            }
        }

        private sealed class PropertyMappingComparer:IEqualityComparer<IPropertyMapping>
        {
            private readonly AbsoluteUriComparer _uriComparer=AbsoluteUriComparer.Default;

            public bool Equals(IPropertyMapping x,IPropertyMapping y)
            {
                if (ReferenceEquals(x,y)) { return true; }
                if (ReferenceEquals(x,null)) { return false; }
                if (ReferenceEquals(y,null)) { return false; }

                return _uriComparer.Equals(x.Uri,y.Uri)&&string.Equals(x.Name,y.Name)&&x.ReturnType==y.ReturnType;
            }

            public int GetHashCode(IPropertyMapping obj)
            {
                unchecked
                {
                    var hashCode=obj.Uri.GetHashCode();
                    hashCode=(hashCode * 397)^obj.Name.GetHashCode();
                    hashCode=(hashCode * 397)^obj.ReturnType.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}