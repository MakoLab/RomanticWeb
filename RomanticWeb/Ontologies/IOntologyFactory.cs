using System;
using System.IO;
using RomanticWeb.Ontologies;

namespace Magi.Data
{
	public interface IOntologyFactory
	{
		string[] Accepts { get; }

		Ontology Create(Stream fileStream);
	}
}