using System.Collections.Generic;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public interface IRdfNodeConverter
	{
		IEnumerable<object> Convert(IEnumerable<RdfNode> subjects,IEntityStore tripleSource);
	}
}