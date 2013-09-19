using System.Reflection;

namespace RomanticWeb.Mapping.Fluent
{
	public class CollectionMap : PropertyMap
	{
		public CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

		protected internal override bool IsCollection
		{
			get { return true; }
		} 
	}
}