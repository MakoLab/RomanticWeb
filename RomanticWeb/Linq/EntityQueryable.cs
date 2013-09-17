using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using RomanticWeb.Mapping;
using RomanticWeb.Ontologies;

namespace RomanticWeb.Linq
{
	public class EntityQueryable<T>:QueryableBase<T> where T:class,IEntity
	{
		#region Constructors
		protected internal EntityQueryable(IEntityFactory entityFactory,IMappingsRepository mappingsRepository,IOntologyProvider ontologyProvider):
			base(new EntityQueryProvider<T>(entityFactory,mappingsRepository,ontologyProvider))
		{
			if (entityFactory==null)
			{
				throw new ArgumentNullException("entityFactory");
			}
		}

		protected internal EntityQueryable(IEntityFactory entityFactory,IQueryProvider provider):base(provider)
		{
			if (entityFactory==null)
			{
				throw new ArgumentNullException("entityFactory");
			}

			if (provider==null)
			{
				throw new ArgumentNullException("provider");
			}

			if (!typeof(EntityQueryable<T>).IsAssignableFrom(provider.GetType()))
			{
				throw new ArgumentOutOfRangeException("provider");
			}
		}

		protected internal EntityQueryable(IEntityFactory entityFactory,IQueryProvider provider,Expression expression):base(provider,expression)
		{
			if (entityFactory==null)
			{
				throw new ArgumentNullException("entityFactory");
			}

			if (provider==null)
			{
				throw new ArgumentNullException("provider");
			}

			if (!typeof(EntityQueryProvider<T>).IsAssignableFrom(provider.GetType()))
			{
				throw new ArgumentOutOfRangeException("provider");
			}

			if (expression==null)
			{
				throw new ArgumentNullException("expression");
			}

			if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
			{
				throw new ArgumentOutOfRangeException("expression");
			}
		}
		#endregion
	}
}