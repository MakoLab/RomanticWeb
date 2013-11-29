using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Model
{
    internal class EntityMapping : IEntityMapping
	{
        private readonly IList<IClassMapping> _classes;

        private readonly IList<IPropertyMapping> _properties;

        public EntityMapping(IEnumerable<IClassMapping> classes, IEnumerable<IPropertyMapping> properties)
        {
            _properties=properties.ToList();
            _classes=classes.ToList();
        }

        internal EntityMapping():this(new IClassMapping[0],new IPropertyMapping[0])
        {
        }

        public IEnumerable<IClassMapping> Classes
		{
		    get
		    {
		        return _classes;
		    }
		}

        internal IEnumerable<IPropertyMapping> Properties
        {
            get
            {
                return _properties;
            }
        }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            var propertyMapping=Properties.SingleOrDefault(p => p.Name==propertyName);

            if (propertyMapping==null)
            {
                throw new MappingException(string.Format("No mapping found for property {0}",propertyName));
            }

            return propertyMapping;
        }

        internal void AddPropertyMapping(IPropertyMapping propertyMapping)
        {
            _properties.Add(propertyMapping);
        }

        internal void AddClassMapping(IClassMapping classMapping)
        {
            _classes.Add(classMapping);
        }
	}
}