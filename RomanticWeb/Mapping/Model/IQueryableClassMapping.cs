using System;
using System.Collections.Generic;

namespace RomanticWeb.Mapping.Model
{
    public interface IQueryableClassMapping:IClassMapping
    {
        IEnumerable<Uri> Uris { get; } 
    }
}