using System;
using System.Collections.Generic;
using System.Linq;
using RomanticWeb.Entities;

namespace RomanticWeb.Converters
{
    public abstract class XsdConverterBase:ILiteralNodeConverter
    {
        protected abstract IEnumerable<Uri> SupportedTypes { get; }

        public abstract object Convert(Node objectNode);

        public bool CanConvert(Uri dataType)
        {
            ////IEntity entity=null;
            ////from x in new List<IComposition>()
            ////where x.ValidForType.Any(item => item==((OntologyAccessor)entity["magi"])["DataStructure"])
            ////select x;

            ////entity.rdf.type

            return SupportedTypes.Contains(dataType,new AbsoluteUriComparer());
        }
    }

    ////interface IComposition
    ////{
    ////    IEnumerable<Uri> ValidForType { get; }
    ////}
}