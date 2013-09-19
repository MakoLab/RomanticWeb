using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public class RdfNodeConverter : IRdfNodeConverter
	{
		private readonly IEntityFactory _entityFactory;

		public RdfNodeConverter(IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;
		}

		// todo: refactor this functionality to a specialized class (multiple implementations stored in a lookup dictionary?)
		public IEnumerable<object> Convert(IEnumerable<RdfNode> subjects, ITripleSource tripleSource)
		{
			foreach (var subject in subjects)
			{
				if (subject.IsUri)
				{
					yield return _entityFactory.Create(new UriId(subject.Uri));
				}
				else if (subject.IsBlank)
				{
					IEnumerable<RdfNode> listElements;
					if (tripleSource.TryGetListElements(subject, out listElements))
					{
						yield return Convert(listElements, tripleSource).ToList();
					}
					else
					{
						yield return _entityFactory.Create(new BlankId(subject.BlankNodeId, subject.GraphUri));
					}
				}
				else
				{
					object value;
					if (TryConvert(subject, out value))
					{
						yield return value;
					}
					else
					{
						yield return subject.Literal;
					}
				}
			}
		}

		// todo: refactor this functionality to a specialized class (multiple implementations stored in a lookup dictionary?)
		private bool TryConvert(RdfNode subject, out object value)
		{
			if (subject.DataType != null)
			{
				switch (subject.DataType.ToString())
				{
					case "http://www.w3.org/2001/XMLSchema#int":
					case "http://www.w3.org/2001/XMLSchema#integer":
						int integer;
						if (int.TryParse(subject.Literal, NumberStyles.Any, CultureInfo.InvariantCulture, out integer))
						{
							value = integer;
							return true;
						}

						break;
				}
			}

			value = null;
			return false;
		}
	}
}