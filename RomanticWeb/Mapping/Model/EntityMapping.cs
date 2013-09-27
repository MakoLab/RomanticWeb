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
            return Properties.Single(p => p.Name == propertyName);
        }
	}
}