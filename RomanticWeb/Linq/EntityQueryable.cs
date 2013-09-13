using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;

namespace RomanticWeb.Linq
{
	public class EntityQueryable<T>:QueryableBase<T> where T:class,IEntity
	{
		internal protected EntityQueryable(IEntityFactory entityFactory):base(new EntityQueryProvider<T>(entityFactory))
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
		}

		internal protected EntityQueryable(IEntityFactory entityFactory,IQueryProvider provider):base(provider)
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
			if (provider==null)
				throw new ArgumentNullException("provider");
		}

		internal protected EntityQueryable(IEntityFactory entityFactory,IQueryProvider provider,Expression expression):base(provider,expression)
		{
			if (entityFactory==null)
				throw new ArgumentNullException("entityFactory");
			if (provider==null)
				throw new ArgumentNullException("provider");
			if (expression==null)
				throw new ArgumentNullException("expression");
			if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
				throw new ArgumentOutOfRangeException("expression");
		}
	}
}