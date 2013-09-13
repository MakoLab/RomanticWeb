using System;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public interface IMapping
	{
		ITypeMapping Type { get; }
		IPropertyMapping PropertyFor(string propertyName);
	}

	public interface IMapping<TEntity>:IMapping
	{
	}

	public interface IPropertyMapping
	{
		Uri Uri { get; }
		IGraphSelectionStrategy GraphSelector { get; }
		bool UsesUnionGraph { get; }
	}

	public interface ITypeMapping
	{
		Uri Uri { get; }
	}

	public interface IGraphSelectionStrategy
	{
		Uri SelectGraph(EntityId entityId);
	}
}