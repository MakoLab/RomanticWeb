using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface.Dynamic;

namespace RomanticWeb.Model
{
    internal class LooseEntityQuadEqualityComparer : IEqualityComparer<EntityQuad>
    {
        internal static readonly LooseEntityQuadEqualityComparer Instance = new LooseEntityQuadEqualityComparer();

        private LooseEntityQuadEqualityComparer()
        {
        }

        public bool Equals(EntityQuad x, EntityQuad y)
        {
            return GetHashCode(x).Equals(GetHashCode(y));
        }

        public int GetHashCode(EntityQuad obj)
        {
            unchecked
            {
                var hashCode = obj.Object.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Subject.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Predicate.GetHashCode();
                return hashCode;
            }
        }
    }
}