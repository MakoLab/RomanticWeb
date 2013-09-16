using System;
using System.Collections.Generic;

namespace RomanticWeb.Ontologies
{
	public class RdfNodeEqualityComparer:IEqualityComparer<RdfNode>
	{
		public static readonly RdfNodeEqualityComparer Default=new RdfNodeEqualityComparer();

		public bool Equals(RdfNode x,RdfNode y)
		{
			return ((Object.Equals(x,null))&&(Object.Equals(y,null)))||((!Object.Equals(x,null))&&(!Object.Equals(y,null))&&(x.GetHashCode()==y.GetHashCode()));
		}

		public int GetHashCode(RdfNode obj)
		{
			return obj.GetHashCode();
		}
	}
}
