using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Mapping.Fluent;
using RomanticWeb.TestEntities.Inheritance;

namespace RomanticWeb.TestEntities.FluentMappings
{
    public class GenericContainerMap : EntityMap<IGenericContainer>
    {
        public GenericContainerMap()
        {
            Class.Is("life", "generic");
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Generic nad non-generic class")]
    public class GenericContainerMap<T> : EntityMap<IGenericContainer<T>>
    {
        public GenericContainerMap()
        {
            Collection(instance => instance.Items).Term.Is("life", "items");
        }
    }
}