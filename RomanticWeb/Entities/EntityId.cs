using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text.RegularExpressions;
using NullGuard;

namespace RomanticWeb
{
	public abstract class EntityId
	{
		[ImportMany(typeof(EntityId))]
		private static IList<EntityId> EntityIdTypes;

		static EntityId()
		{
			DirectoryCatalog catalog=new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory,"*.dll");
			CompositionContainer container=new CompositionContainer(catalog);
			CompositionBatch batch=new CompositionBatch();
			EntityIdTypes=(IList<EntityId>)container.GetExportedValues<EntityId>();
		}

		/// <summary>Gets a regular expression matching given scheme of the IRI adress handled by given identifier implementation.</summary>
		public abstract Regex MatchingScheme { get; }

		public static EntityId Create(string entityId)
		{
			EntityId entityIdType=EntityIdTypes.Where(item => item.MatchingScheme.IsMatch(entityId)).FirstOrDefault();
			if (entityIdType==null)
			{
				throw new NotSupportedException(System.String.Format("Identifiers of type '{0}' are not supported.",entityId));
			}

			EntityId result=(EntityId)entityIdType.GetType().GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { entityId });
			return result;
		}

		/// <summary>
		/// Checks if two Entity identifiers are equal
		/// </summary>
		public static bool operator==([AllowNull] EntityId left,[AllowNull] EntityId right)
		{
			return Equals(left,right);
		}

		/// <summary>
		/// Checks if two Entity identifiers are not equal
		/// </summary>
		public static bool operator!=([AllowNull] EntityId left,[AllowNull] EntityId right)
		{
			return !(left==right);
		}

		public static implicit operator EntityId(string entityId)
		{
			EntityId result=null;
			if (entityId!=null)
			{
			    result=EntityId.Create(entityId);
			}

			return result;
		}
	}
}