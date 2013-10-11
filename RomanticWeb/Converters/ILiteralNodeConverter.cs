using System;
using RomanticWeb.Model;

namespace RomanticWeb.Converters
{
	public interface ILiteralNodeConverter
	{
		object Convert(Node objectNode);

	    bool CanConvert(Uri dataType);
	}
}