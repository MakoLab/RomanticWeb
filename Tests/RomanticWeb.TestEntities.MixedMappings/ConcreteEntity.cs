using System;
using System.Collections.Generic;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.MixedMappings
{
    public class ConcreteEntity :IGenericParent<string>
    {
        public EntityId Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEntityContext Context
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string MappedProperty1
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string MappedProperty2
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [Property("urn:concrete:class")]
        public string UnMappedProperty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<string> GenericProperty
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string NonGenericProperty
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}