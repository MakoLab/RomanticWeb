using System;

namespace RomanticWeb
{
	public interface ILiteralNodeConverter
	{
		object Convert(Node objectNode);

	    bool CanConvert(Uri dataType);
	}
}