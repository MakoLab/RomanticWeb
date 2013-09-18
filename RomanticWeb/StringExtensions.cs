using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb
{
	public static class StringExtensions
	{
		public static string Indent(this string text,int indentation)
		{
			string result=null;
			if (text!=null)
			{
				result=text;
				if (indentation>0)
				{
					string indent=new String('\t',indentation);
					result=indent+result.Replace("\r",System.String.Empty).Replace("\n",System.Environment.NewLine+indent);
				}
			}

			return result;
		}
	}
}