using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RomanticWeb.Entities;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public class RdfNodeConverter : IRdfNodeConverter
	{
		private readonly IEntityContext _entityContext;

	    private readonly Lazy<IEntity> _listNil;

		public RdfNodeConverter(IEntityContext entityContext)
		{
			_entityContext = entityContext;
		}

		// todo: refactor this functionality to a specialized class (multiple implementations stored in a lookup dictionary?)
		public IEnumerable<object> Convert(IEnumerable<Node> subjects,IEntityStore tripleSource)
		{
			foreach (var subject in subjects)
			{
				if (subject.IsUri)
				{
                    yield return _entityContext.Create(subject.ToEntityId());
				}
				else if (subject.IsBlank)
				{
				    dynamic potentialList=_entityContext.Create(subject.ToEntityId()).AsDynamic();

                    var blankNodeListConverter = new BlankNodeListConverter(tripleSource);

				    object actualList;
				    if (blankNodeListConverter.TryConvert(potentialList,out actualList))
				    {
				        yield return actualList;
				    }
                    else
                    {
                        yield return potentialList;
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
		private bool TryConvert(Node subject, out object value)
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