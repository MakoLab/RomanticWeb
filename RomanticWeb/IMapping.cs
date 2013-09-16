namespace RomanticWeb
{
	public interface IMapping
	{
		ITypeMapping Type { get; }

		IPropertyMapping PropertyFor(string propertyName);
	}
}