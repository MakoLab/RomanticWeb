using System.Reflection;

namespace RomanticWeb.Mapping.Fluent
{
    /// <summary>
    /// A mapping definition for collection properties
    /// </summary>
	public class CollectionMap : PropertyMap
	{
        internal CollectionMap(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
		}

        /// <summary>
        /// Returns true
        /// </summary>
		protected internal override bool IsCollection
		{
			get { return true; }
		} 
	}
}