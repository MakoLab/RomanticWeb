using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NullGuard;
using RomanticWeb.Converters;
using RomanticWeb.Entities;
using RomanticWeb.NamedGraphs;

namespace RomanticWeb.Collections
{
    [DebuggerDisplay("Count = {Count}")]
    [NullGuard(ValidationFlags.All)]
    internal class RdfListAdapter<TOwner,TConverter,T>:IList<T>,IRdfListAdapter<TOwner,TConverter,T>
        where TConverter:INodeConverter
    {
        private readonly IEntityContext _context;
        private readonly ISourceGraphSelectionOverride _namedGraphOverride;
        private readonly IEntity _owner;
        private IRdfListNode<TOwner,TConverter,T> _head;
        private IRdfListNode<TOwner,TConverter,T> _tail;

        public RdfListAdapter(IEntityContext context,IEntity owner,IRdfListNode<TOwner,TConverter,T> head,ISourceGraphSelectionOverride namedGraphOverride)
        {
            Count=0;
            _context=context;
            _owner=owner;
            _namedGraphOverride=namedGraphOverride;
            _head=_tail=head;

            Initialize();
        }

        public RdfListAdapter(IEntityContext context,IEntity owner,ISourceGraphSelectionOverride namedGraphOverride)
            :this(context,owner,context.Load<IRdfListNode<TOwner,TConverter,T>>(Vocabularies.Rdf.nil),namedGraphOverride)
        {
        }

        IRdfListNode<TOwner,TConverter,T> IRdfListAdapter<TOwner,TConverter,T>.Head { get { return _head; } }

        public int Count { get; private set; }

        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get { return (T)GetNodeAt(index).First; }
            set { Insert(index,value); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(_head);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IRdfListAdapter<TOwner,TConverter,T>.Add(T item)
        {
            Add((T)item);
        }

        public void Add(T item)
        {
            if (item!=null)
            {
                if (_head.Id==Vocabularies.Rdf.nil)
                {
                    InsertToEmptyList(item);
                }
                else
                {
                    var newTail=InsertNodeAfter(_tail,item);
                    _tail=newTail;
                }
            }
        }

        public void Clear()
        {
            var currentNode=_head;
            while (currentNode.Id!=Vocabularies.Rdf.nil)
            {
                _context.Delete(currentNode.Id);
                currentNode=currentNode.Rest;
            }

            _head=_tail=_context.Load<IRdfListNode<TOwner,TConverter,T>>(Vocabularies.Rdf.nil);
        }

        public bool Contains(T item)
        {
            return GetNodeForItemWithPredecessor(item).Item1.Id!=Vocabularies.Rdf.nil;
        }

        public void CopyTo(T[] array,int arrayIndex)
        {
            if (array==null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex<0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (Count>array.Length-arrayIndex)
            {
                throw new ArgumentException("The number of elements in the source collection is greater than the available space from 'arrayIndex' to the end of the destination array.");
            }

            IRdfListNode<TOwner,TConverter,T> currentNode=_head;
            int currentIndex=0;

            while (currentNode.Id!=Vocabularies.Rdf.nil)
            {
                array[arrayIndex+currentIndex]=currentNode.First;
                currentNode=currentNode.Rest;
                currentIndex++;
            }
        }

        public bool Remove(T item)
        {
            var nodeWithPredecessor=GetNodeForItemWithPredecessor(item);

            if (nodeWithPredecessor.Item1.Id==Vocabularies.Rdf.nil)
            {
                return false;
            }

            DeleteNode(nodeWithPredecessor.Item1,nodeWithPredecessor.Item2);

            return true;
        }

        public int IndexOf(T item)
        {
            throw new System.NotImplementedException();
        }

        public void Insert(int index,T item)
        {
            if ((index<0)||(index>Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (index==0)
            {
                InsertFirstNode(item);
            }
            else if (index==Count)
            {
                Add(item);
            }
            else
            {
                var nodeToReplace=GetNodeAt(index-1);
                InsertNodeAfter(nodeToReplace,item);
            }
        }

        public void RemoveAt(int index)
        {
            if (index==0)
            {
                DeleteNode(_head,null);
            }
            else
            {
                IRdfListNode<TOwner,TConverter,T> previousNode=GetNodeAt(index-1);
                DeleteNode(previousNode.Rest,previousNode);
            }
        }

        private void DeleteNode(IRdfListNode<TOwner,TConverter,T> nodeToDelete,[AllowNull] IRdfListNode<TOwner,TConverter,T> previousNode)
        {
            if (previousNode!=null)
            {
                previousNode.Rest=nodeToDelete.Rest;
                if (nodeToDelete.Rest.Id==Vocabularies.Rdf.nil)
                {
                    _tail=previousNode;
                }
            }
            else
            {
                _head=nodeToDelete.Rest;
            }

            _context.Delete(nodeToDelete.Id);
        }

        private Tuple<IRdfListNode<TOwner,TConverter,T>,IRdfListNode<TOwner,TConverter,T>> GetNodeForItemWithPredecessor(T item)
        {
            IRdfListNode<TOwner,TConverter,T> previousNode=null;
            var currentNode=_head;
            while (currentNode.Id!=Vocabularies.Rdf.nil)
            {
                if (Equals(currentNode.First,item))
                {
                    break;
                }

                previousNode=currentNode;
                currentNode=currentNode.Rest;
            }

            return Tuple.Create(currentNode,previousNode);
        }

        private IRdfListNode<TOwner,TConverter,T> GetNodeAt(int index)
        {
            if ((index<0)||(index>=Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            IRdfListNode<TOwner,TConverter,T> currentNode=_head;
            int currentIndex=0;

            while ((currentIndex<index)&&(currentNode.Id!=Vocabularies.Rdf.nil))
            {
                currentNode=currentNode.Rest;
                currentIndex++;
            }

            return currentNode;
        }

        private IRdfListNode<TOwner,TConverter,T> InsertNodeAfter(IRdfListNode<TOwner,TConverter,T> existingNode,T item)
        {
            var newNode=CreateNode();
            newNode.First=item;
            newNode.Rest=existingNode.Rest;
            existingNode.Rest=newNode;
            Count++;

            return newNode;
        }

        private void InsertToEmptyList(T item)
        {
            var newNode=InsertFirstNode(item);
            _tail=newNode;
        }

        private IRdfListNode<TOwner,TConverter,T> InsertFirstNode(T item)
        {
            var newNode=CreateNode();
            newNode.First=item;
            newNode.Rest=_head;
            _head = newNode;
            Count++;

            return newNode;
        }

        private IRdfListNode<TOwner,TConverter,T> CreateNode()
        {
            var nodeId=_owner.CreateBlankId();
            var newNode=_context.Create<IRdfListNode<TOwner,TConverter,T>>(nodeId);
            var proxy=newNode.UnwrapProxy() as IEntityProxy;

            if (proxy!=null)
            {
                proxy.OverrideGraphSelection(_namedGraphOverride);
            }

            return newNode;
        }

        private void Initialize()
        {
            foreach (var element in this)
            {
                Count++;
            }
        }

        private class Enumerator:IEnumerator<T>
        {
            private readonly IRdfListNode<TOwner,TConverter,T> _firstNode;
            private IRdfListNode<TOwner,TConverter,T> _currentNode;

            public Enumerator(IRdfListNode<TOwner,TConverter,T> actualEntity)
            {
                _firstNode=actualEntity;
            }

            public T Current
            {
                get
                {
                    if ((_currentNode==null)||(_currentNode.First==null))
                    {
                        return default(T);
                    }

                    return (T)_currentNode.First;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_currentNode==null)
                {
                    _currentNode=_firstNode;
                }
                else
                {
                    _currentNode=_currentNode.Rest;
                }

                if (_currentNode==null||_currentNode.Id==Vocabularies.Rdf.nil)
                {
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _currentNode=null;
            }
        }
    }
}