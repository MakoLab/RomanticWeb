using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Model
{
    internal class EntityMapping : IEntityMapping
	{
		public EntityMapping()
		{
			Properties = new List<IPropertyMapping>();
		}

		public IClassMapping Class { get; internal set; }

        internal IList<IPropertyMapping> Properties { get; set; }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            var propertyMapping=Properties.SingleOrDefault(p => p.Name==propertyName);

            if (propertyMapping==null)
            {
                throw new MappingException(string.Format("No mapping found for property {0}",propertyName));
            }

            return propertyMapping;
        }
	}
}