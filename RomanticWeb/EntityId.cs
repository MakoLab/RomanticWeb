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

		public static bool operator==([AllowNull] EntityId operandA,[AllowNull] string operandB)
		{
			EntityId _operandB=null;
			if (operandB!=null)
				_operandB=EntityId.Create(operandB);
			if (((Object.Equals(operandA,null))&&(Object.Equals(_operandB,null)))||
				((!Object.Equals(operandA,null))&&(!Object.Equals(_operandB,null))&&(operandA.Equals(_operandB))))
				return true;
			return false;
		}

		public static bool operator!=([AllowNull] EntityId operandA,[AllowNull] string operandB)
		{
			return !(operandA==operandB);
		}

		public static bool operator==([AllowNull] string operandA,[AllowNull] EntityId operandB)
		{
			EntityId _operandA=null;
			if (operandA!=null)
				_operandA=EntityId.Create(operandA);
			if (((Object.Equals(_operandA,null))&&(Object.Equals(operandB,null)))||
				((!Object.Equals(_operandA,null))&&(!Object.Equals(operandB,null))&&(operandA.Equals(operandB))))
				return true;
			return false;
		}

		public static bool operator!=([AllowNull] string operandA,[AllowNull] EntityId operandB)
		{
			return !(operandA==operandB);
		}

		public static explicit operator EntityId(string entityId)
		{
			EntityId result=null;
			if (entityId!=null)
				result=EntityId.Create(entityId);
			return result;
		}
	}
}