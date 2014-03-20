namespace RomanticWeb.Mapping.Model
{
    /// <summary>
    /// Represents different way to persist collections to triples
    /// </summary>
    public enum StoreAs
    {
        /// <summary>
        /// Not applicable, ie. not a collection
        /// </summary>
        Undefined, 
        
        /// <summary>
        /// Persist a collection as multiple triples for subject/predicate pair
        /// </summary>
        SimpleCollection, 
        
        /// <summary>
        /// Presist a colelction as an rdf:List
        /// </summary>
        RdfList
    }
}