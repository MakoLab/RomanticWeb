using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Inheritance;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class SpecificContainerMap : EntityMap<ISpecificContainer>
    {
        public SpecificContainerMap()
        {
            Class.Is("life", "specific");
            Collection(instance => instance.Items).Term.Is("life", "items");
        }
    }
}