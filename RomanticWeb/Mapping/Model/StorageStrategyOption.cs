namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Represents different way to persist collections to triples
    /// </summary>
    public enum StorageStrategyOption
    {
        /// <summary>
        /// Not applicable, ie. not a collection
        /// </summary>
        None, 
        
        /// <summary>
        /// Persist a collection as multiple triples for subject/predicate pair
        /// </summary>
        Simple, 
        
        /// <summary>
        /// Presist a colelction as an rdf:List
        /// </summary>
        RdfList
    }
}