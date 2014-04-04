using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Visitors
{
    /// <summary>
    /// Visits mapping attributes and produces mapping providers
    /// </summary>
    public interface IMappingModelVisitor
    {
        /// <summary>
        /// Visits the specified entity mapping.
        /// </summary>
        /// <param name="entityMapping">The entity mapping.</param>
        void Visit(IEntityMapping entityMapping);

        /// <summary>
        /// Visits the specified collection mapping.
        /// </summary>
        /// <param name="collectionMapping">The collection mapping.</param>
        void Visit(ICollectionMapping collectionMapping);

        /// <summary>
        /// Visits the specified dictionary mapping.
        /// </summary>
        /// <param name="dictionaryMapping">The dictionary mapping.</param>
        void Visit(IDictionaryMapping dictionaryMapping);

        /// <summary>
        /// Visits the specified property mapping.
        /// </summary>
        /// <param name="propertyMapping">The property mapping.</param>
        void Visit(IPropertyMapping propertyMapping);

        /// <summary>
        /// Visits the specified class mapping.
        /// </summary>
        /// <param name="classMapping">The class mapping.</param>
        void Visit(IClassMapping classMapping);
    }
}