using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Entities;
using RomanticWeb.Mapping.Attributes;

namespace RomanticWeb.TestEntities.Inheritance
{
    [Class("life", "generic")]
    public interface IGenericContainer : IEntity
    {
    }

    public interface IGenericContainer<T> : IGenericContainer
    {
        [Collection("life", "items")]
        ICollection<T> Items { get; }
    }
}