using System;
using RomanticWeb.Entities;

namespace RomanticWeb
{
    /// <summary>
    /// Defines method for selecting named graph URI based on <see cref="EntityId"/> 
    /// </summary>
	public interface IGraphSelectionStrategy
	{
        /// <summary>
        /// Gets a named graph URI for a given entity
        /// </summary>
        /// <param name="entityId">Entity's identifier</param>
		Uri SelectGraph(EntityId entityId);
	}
}