using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;
using RomanticWeb.Mapping.Fluent;

namespace RomanticWeb.TestEntities.Inheritance
{
    public interface ISomeSpecializedEntity : ISomeEntity
    {
        [Property("http://other/domain/property")]
        new string Property { get; set; }
    }

    public class SomeSpecializedEntityMap : EntityMap<ISomeSpecializedEntity>
    {
        public SomeSpecializedEntityMap()
        {
            Property(instance => instance.Other).Term.Is(new Uri("http://other/domain/other"));
        }
    }
}