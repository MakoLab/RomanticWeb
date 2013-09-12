﻿using System;

namespace RomanticWeb.Tests.Stubs
{
    public class TestPropertyMapping : IPropertyMapping
    {
        public Uri Uri { get; set; }
        public IGraphSelectionStrategy GraphSelector { get; set; }
        public bool UsesUnionGraph { get; set; }
    }
}