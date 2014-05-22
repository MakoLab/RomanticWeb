using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Model
{
    internal sealed class Index<T>
    {
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1401:FieldsMustBePrivate",Justification="Performance is top priority here.")]
        internal T Key=default(T);

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1401:FieldsMustBePrivate",Justification="Performance is top priority here.")]
        internal int StartAt=0;

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules","SA1401:FieldsMustBePrivate",Justification="Performance is top priority here.")]
        internal int Length=0;

        internal Index(T key,int startAt,int length)
        {
            Key=key;
            StartAt=startAt;
            Length=length;
        }

        public override bool Equals(object obj)
        {
            if (Object.Equals(obj,null)) { return false; }
            if (Object.ReferenceEquals(obj,this)) { return true; }
            if (!(obj is Index<T>)) { return false; }
            Index<T> index=(Index<T>)obj;
            return index.Key.Equals(Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return System.String.Format("{0}@{1} -> {2}",Key,StartAt,Length);
        }

        internal bool Contains(int itemIndex)
        {
            return ((itemIndex!=IndexCollection<T>.FirstPossible)&&(itemIndex>=StartAt)&&(itemIndex<StartAt+Length))||(itemIndex==IndexCollection<T>.FirstPossible);
        }
    }
}