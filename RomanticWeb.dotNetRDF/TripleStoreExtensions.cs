using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Vocabularies;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Builder;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>Provides useful extension methods for <see cref="ITripleStore" />.</summary>
    public static class TripleStoreExtensions
    {
        /// <summary>Loads data from file with optional automated graph generation.</summary>
        /// <param name="store">Target store to be loaded with data.</param>
        /// <param name="file">Source file with data.</param>
        /// <param name="metaGraphUri">When provided, store will have automatically created graphs for all resources that are mentioned in the meta graph provided.</param>
        public static void LoadFromFile(this ITripleStore store,string file,Uri metaGraphUri)
        {
            ITripleStore targetStore=(metaGraphUri!=null?new TripleStore():store);
            targetStore.LoadFromFile(file);
            if (metaGraphUri!=null)
            {
                store.ExpandGraphs((TripleStore)targetStore,metaGraphUri);
            }
        }

        /// <summary>Loads data from file with optional automated graph generation.</summary>
        /// <param name="store">Target store to be loaded with data.</param>
        /// <param name="resourceName">Source file with data.</param>
        /// <param name="metaGraphUri">When provided, store will have automatically created graphs for all resources that are mentioned in the meta graph provided.</param>
        public static void LoadFromEmbeddedResource(this ITripleStore store,string resourceName,Uri metaGraphUri)
        {
            ITripleStore targetStore=(metaGraphUri!=null?new TripleStore():store);
            targetStore.LoadFromEmbeddedResource(resourceName);
            if (metaGraphUri!=null)
            {
                store.ExpandGraphs((TripleStore)targetStore,metaGraphUri);
            }
        }

        private static void ExpandGraphs(this ITripleStore store,TripleStore targetStore,Uri metaGraphUri)
        {
            IGraph metaGraph=store.AddGraph(metaGraphUri);
            foreach (Triple triple in targetStore.Triples)
            {
                IUriNode subject=(triple.Subject is IBlankNode?targetStore.FindOwningSubject((IBlankNode)triple.Subject):(IUriNode)triple.Subject);
                if (subject!=null)
                {
                    IGraph graph=store.GetGraph(metaGraphUri,subject.Uri);
                    graph.Assert(graph.Import(triple));
                }
            }
        }

        private static IUriNode FindOwningSubject(this TripleStore store,IBlankNode blankNode)
        {
            INode result=blankNode;
            do
            {
                Triple triple=store.Triples.Where(item => item.Object.Equals(result)).FirstOrDefault();
                if (triple!=null)
                {
                    result=triple.Subject;
                }
            }
            while ((result!=null)&&(result is IBlankNode));

            return (IUriNode)result;
        }

        private static Triple Import(this IGraph graph,Triple triple)
        {
            INode subject=null;
            if (triple.Subject is IUriNode)
            {
                subject=graph.CreateUriNode(((IUriNode)triple.Subject).Uri);
            }
            else
            {
                subject=graph.CreateBlankNode(((IBlankNode)triple.Subject).InternalID);
            }

            IUriNode predicate=graph.CreateUriNode(((IUriNode)triple.Predicate).Uri);
            INode @object=null;
            if (triple.Object is IUriNode)
            {
                @object=graph.CreateUriNode(((IUriNode)triple.Object).Uri);
            }
            else if (triple.Object is IBlankNode)
            {
                @object=graph.CreateBlankNode(((IBlankNode)triple.Object).InternalID);
            }
            else
            {
                ILiteralNode value=(ILiteralNode)triple.Object;
                if (value.DataType!=null)
                {
                    @object=graph.CreateLiteralNode(value.Value,value.DataType);
                }
                else
                {
                    @object=graph.CreateLiteralNode(value.Value,value.Language);
                }
            }

            return new Triple(subject,predicate,@object);
        }

        private static IGraph GetGraph(this ITripleStore store,Uri metaGraphUri,Uri graphBaseUri)
        {
            IGraph graph=store.Graphs.Where(item => item.BaseUri.AbsoluteUri==graphBaseUri.AbsoluteUri).FirstOrDefault();
            if (graph==null)
            {
                graph=store.AddGraph(graphBaseUri);
                IGraph metaGraph=store.Graphs.Where(item => item.BaseUri.AbsoluteUri==metaGraphUri.AbsoluteUri).FirstOrDefault();
                if (metaGraph==null)
                {
                    metaGraph=store.AddGraph(metaGraphUri);
                }

                metaGraph.Assert(graph.CreateUriNode(graphBaseUri),graph.CreateUriNode(Foaf.primaryTopic),graph.CreateUriNode(graphBaseUri));
            }

            return graph;
        }

        private static IGraph AddGraph(this ITripleStore store,Uri graphBaseUri)
        {
            IGraph result=new Graph() { BaseUri=graphBaseUri };
            if (store.Add(result))
            {
                return result;
            }

            return null;
        }
    }
}