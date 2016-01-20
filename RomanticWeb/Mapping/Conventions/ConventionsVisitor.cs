using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RomanticWeb.Mapping.Providers;
using RomanticWeb.Mapping.Visitors;

namespace RomanticWeb.Mapping.Conventions
{
    /// <summary>Visits mapping providers and applies conventions.</summary>
    public class ConventionsVisitor : IMappingProviderVisitor
    {
        private const int MaxChainIterations = 6;

        private readonly IEnumerable<IConvention> _conventions;

        /// <summary>Initializes a new instance of the <see cref="ConventionsVisitor"/> class.</summary>
        public ConventionsVisitor(MappingContext mappingContext)
        {
            lock (this)
            {
                _conventions = InitializeConventions(mappingContext.Conventions);
            }
        }

        /// <summary>Applies property and collection conventions to <paramref name="collectionMappingProvider"/>.</summary>
        public void Visit(ICollectionMappingProvider collectionMappingProvider)
        {
            SelectAndApplyConventions<ICollectionConvention, ICollectionMappingProvider>(collectionMappingProvider);
            Visit(collectionMappingProvider as IPropertyMappingProvider);
        }

        /// <summary>Applies property conventions to <paramref name="propertyMappingProvider"/>.</summary>
        public void Visit(IPropertyMappingProvider propertyMappingProvider)
        {
            SelectAndApplyConventions<IPropertyConvention, IPropertyMappingProvider>(propertyMappingProvider);
        }

        /// <summary>Applies property and dictionary conventions to <paramref name="dictionaryMappingProvider"/>.</summary>
        public void Visit(IDictionaryMappingProvider dictionaryMappingProvider)
        {
            SelectAndApplyConventions<IDictionaryConvention, IDictionaryMappingProvider>(dictionaryMappingProvider);
            Visit(dictionaryMappingProvider as IPropertyMappingProvider);
        }

        /// <summary>Does nothing for now.</summary>
        public void Visit(IClassMappingProvider classMappingProvider)
        {
        }

        /// <summary>Does nothing for now.</summary>
        public void Visit(IEntityMappingProvider entityMappingProvider)
        {
        }

        private void SelectAndApplyConventions<TConvention, TProvider>(TProvider provider) where TConvention : IConvention<TProvider>
        {
            foreach (var convention in _conventions.OfType<TConvention>().Where(convention => convention.ShouldApply(provider)))
            {
                convention.Apply(provider);
            }
        }

        private IEnumerable<IConvention> InitializeConventions(IEnumerable<IConvention> conventions)
        {
            var iterations = 0;
            var currentConventions = conventions.ToList();
            var validRequirements = new Dictionary<IConvention, IList<Type>>();
            var result = new List<IConvention>();
            var waiting = new Stack<IConvention>();
            do
            {
                var added = new List<IConvention>();
                foreach (var convention in currentConventions)
                {
                    IList<Type> validRequired;
                    if (!validRequirements.TryGetValue(convention, out validRequired))
                    {
                        validRequirements[convention] = validRequired =
                            convention.Requires.Join(conventions, outer => outer, inner => inner.GetType(), (outer, inner) => outer).ToList();
                    }

                    if (validRequired.Join(result, outer => outer, inner => inner.GetType(), (outer, inner) => inner).Count() == validRequired.Count)
                    {
                        result.Add(convention);
                        added.Add(convention);
                        if ((waiting.Count > 0) && (waiting.Peek() == convention))
                        {
                            waiting.Pop();
                        }
                    }
                    else
                    {
                        waiting.Push(convention);
                    }
                }

                added.ForEach(item => currentConventions.Remove(item));
                iterations++;
            }
            while ((waiting.Count > 0) && (currentConventions.Count > 0) && (iterations < MaxChainIterations));
            return result;
        }
    }
}