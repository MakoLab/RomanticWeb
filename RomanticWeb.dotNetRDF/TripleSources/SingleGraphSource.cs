using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Ontologies;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace RomanticWeb.dotNetRDF.TripleSources
{
	public class SingleGraphSource:ITripleSource
	{
		private readonly IGraph _graph;

		public SingleGraphSource(IGraph graph)
		{
			_graph=graph;
		}

		public virtual IEnumerable<RdfNode> GetObjectsForPredicate(EntityId entityId,Uri predicate)
		{
			INode entityNode=entityId.ToNode(_graph);
			INode predicateNode=_graph.CreateUriNode(predicate);

			return _graph.GetTriplesWithSubjectPredicate(entityNode,predicateNode)
						 .Select(t => t.Object.WrapNode());
		}

		public bool TryGetListElements(RdfNode rdfList,out IEnumerable<RdfNode> listElements)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<Tuple<RdfNode,RdfNode,RdfNode>> GetNodesForQuery(string commandText)
		{
			IEnumerable<Tuple<RdfNode,RdfNode,RdfNode>> result=new Tuple<RdfNode,RdfNode,RdfNode>[0];
			SparqlQuery query=new SparqlQueryParser().ParseFromString(commandText);
			InMemoryDataset dataSet=new InMemoryDataset(_graph);
			ISparqlQueryProcessor processor=new LeviathanQueryProcessor(dataSet);
			object results=processor.ProcessQuery(query);
			if (results is IGraph)
				result=((IGraph)result).Triples.Select(t => new Tuple<RdfNode,RdfNode,RdfNode>(t.Subject.WrapNode(),t.Predicate.WrapNode(),t.Object.WrapNode()));
			return result;
		}
	}
}