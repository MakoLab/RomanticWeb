using System;
using System.Diagnostics;
using NullGuard;

namespace RomanticWeb.Mapping.Model
{
    [NullGuard(ValidationFlags.All)]
    [DebuggerDisplay("Class {Uri}")]
    internal class ClassMapping:IClassMapping
    {
        private static readonly AbsoluteUriComparer UriComparer=new AbsoluteUriComparer();

        public ClassMapping(Uri uri)
        {
            Uri=uri;
        }

        public Uri Uri { get; private set; }

        public static bool operator ==([AllowNull]ClassMapping left,[AllowNull]ClassMapping right)
        {
            return Equals(left,right);
        }

        public static bool operator !=([AllowNull]ClassMapping left,[AllowNull]ClassMapping right)
        {
            return !Equals(left,right);
        }

        public override bool Equals([AllowNull]object obj)
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

        protected bool Equals([AllowNull]ClassMapping other)
        {
            return UriComparer.Equals(Uri,other.Uri);
        }
    }
}