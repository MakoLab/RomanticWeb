using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Inheritance
{
    [Class("life", "specific")]
    public interface ISpecificContainer : IGenericContainer<IInterface>
    {
    }
}