using System.Collections.Generic;
using System.Linq;

namespace RomanticWeb.Mapping.Model
{
    internal class EntityMapping : IMapping
	{
		public EntityMapping()
		{
			this.Properties = new List<IPropertyMapping>();
		}

		public ITypeMapping Type { get; internal set; }

        internal IList<IPropertyMapping> Properties { get; set; }

        public IPropertyMapping PropertyFor(string propertyName)
        {
            return this.Properties.Single(p => p.Name == propertyName);
        }
	}
}