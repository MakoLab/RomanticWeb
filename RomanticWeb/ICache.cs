namespace RomanticWeb
{
    /// <summary>Serves as a base interface for caching data.</summary>
    public interface ICache
    {
        /// <summary>Checks if a this cache contains a data under given key.</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);

        /// <summary>Retrieves data from given key.</summary>
        /// <typeparam name="T">Type of the resulting data.</typeparam>
        /// <param name="key">Key of data to be retrieved..</param>
        /// <returns>Data cached.</returns>
        T Retrieve<T>(string key);

        /// <summary>Stores data under given key.</summary>
        /// <param name="key">Key to be stored at.</param>
        /// <param name="data">Data to be stored.</param>
        void Store(string key, object data);
    }
}