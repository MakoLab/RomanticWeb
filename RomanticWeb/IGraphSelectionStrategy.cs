using System;
using RomanticWeb.Entities;

namespace RomanticWeb
{
	public interface IGraphSelectionStrategy
	{
		Uri SelectGraph(EntityId entityId);
	}
}