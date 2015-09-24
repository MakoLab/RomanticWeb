using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Inheritance
{
    public interface ISomeEntity : IEntity
    {
        [Property("http://some/domain/property")]
        string Property { get; set; }

        [Property("http://some/domain/other")]
        string Other { get; set; }

        [Collection("http://some/domain/strings")]
        string[] Strings { get; set; }
    }
}