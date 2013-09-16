using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping
{
	internal class EntityMapping : IMapping
	{
		public EntityMapping()
		{
			Properties = new List<IPropertyMapping>();
		}

		public ITypeMapping Type { get; private set; }

        public IList<IPropertyMapping> Properties { get; private set; }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            return Properties.Single(p => p.Name == propertyName);
        }
	}
}