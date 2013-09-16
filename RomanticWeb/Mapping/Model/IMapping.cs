namespace RomanticWeb.Mapping.Model
{
	public interface IMapping
	{
		ITypeMapping Type { get; }

		IPropertyMapping PropertyFor(string propertyName);
	}
}