using System;
using System.Collections.Generic;

namespace RomanticWeb
{
    /// <summary>
    /// Based on idea from http://www.make-awesome.com/2010/06/easier-complex-icomparable-implementations/
    /// </summary>
    internal class FluentCompare<T>
        where T : class
    {
        private readonly T _left;

        private readonly T _right;

        private int _value;

        private FluentCompare(T left, object right)
        {
            _left = left;
            _right = right as T;

            if (_right == null)
            {
                _value = 1;
            }
        }

        public static FluentCompare<T> Arguments(T left, object right)
        {
            return new FluentCompare<T>(left, right);
        }

        public FluentCompare<T> By<TProp>(Func<T, TProp> getter)
        {
            return By(getter, true, Comparer<TProp>.Default);
        }

        public FluentCompare<T> By<TProp>(Func<T, TProp> getter, bool ascending)
        {
            return By(getter, ascending, Comparer<TProp>.Default);
        }

        public FluentCompare<T> By<TProp>(Func<T, TProp> getter, IComparer<TProp> comparer)
        {
            return By(getter, true, comparer);
        }

        public FluentCompare<T> By<TProp>(Func<T, TProp> getter, bool ascending, IComparer<TProp> comparer)
        {
            if (_value == 0)
            {
                _value = DoCompare(getter(_left), getter(_right), ascending, comparer);
            }

            return this;
        }

        public int End()
        {
            return _value;
        }

        private static int DoCompare<TProp>(TProp a, TProp b, bool ascending, IComparer<TProp> comparer)
        {
            if (ascending)
            {
                return comparer.Compare(a, b);
            }

            return comparer.Compare(b, a);
        }
    }
}