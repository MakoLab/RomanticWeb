using System;
using System.Diagnostics;

namespace RomanticWeb.Mapping.Model
{
    [DebuggerDisplay("Class {Uri}")]
    internal class ClassMapping:IClassMapping
    {
        private static readonly AbsoluteUriComparer UriComparer=new AbsoluteUriComparer();

        public ClassMapping(Uri uri,GraphSelectionStrategyBase graphSelector)
        {
            GraphSelector=graphSelector;
            Uri=uri;
        }

        public Uri Uri { get; private set; }

        public GraphSelectionStrategyBase GraphSelector { get; private set; }

        public static bool operator ==(ClassMapping left,ClassMapping right)
        {
            return Equals(left,right);
        }

        public static bool operator !=(ClassMapping left,ClassMapping right)
        {
            return !Equals(left,right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null,obj)) { return false; }
            if (ReferenceEquals(this,obj)) { return true; }
            if (obj.GetType()!=GetType()) { return false; }

            return Equals((ClassMapping)obj);
        }

        public override int GetHashCode()
        {
            return Uri.GetHashCode();
        }

        protected bool Equals(ClassMapping other)
        {
            return UriComparer.Equals(Uri,other.Uri);
        }
    }
}