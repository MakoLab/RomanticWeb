using System;
using System.Collections;
using System.Collections.Generic;
using NullGuard;

namespace RomanticWeb
{
	public abstract class EntityId
	{
		public static EntityId Create(string entityId)
		{
			// TODO: Change fixed UriId creater for some more intelligent probing mechanism
			EntityId result=new UriId(entityId);
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