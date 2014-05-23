using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Linq.Model;
using RomanticWeb.Linq.Sparql;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>Parses given query into a SPARQL 1.1 query.</summary>
    [Obsolete("This class is only to address issue with donNetRDF SPARQL algebra that generates instable result sets. Please use the GenericSparqlQueryVisitor from the RomanticWeb assembly.")]
    public class SparqlQueryVisitor:GenericSparqlQueryVisitor
    {
        /// <inheritdoc />
        protected override void VisitQueryResultModifiers(IDictionary<IExpression,bool> orderByExpressions,int offset,int limit)
        {
            if (orderByExpressions.Count==0)
            {
                CommandTextBuilder.Append("ORDER BY ");
                VisitComponent(CurrentEntityAccessor.About);
                CommandTextBuilder.Append(" ");
            }

            base.VisitQueryResultModifiers(orderByExpressions,offset,limit);
        }
    }
}