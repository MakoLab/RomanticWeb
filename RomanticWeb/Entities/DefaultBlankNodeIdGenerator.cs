namespace RomanticWeb.Entities
{
    /// <summary>
    /// Genreates ids in a sequence magi1, magi2, magi3, etc.
    /// </summary>
    internal class DefaultBlankNodeIdGenerator : IBlankNodeIdGenerator
    {
        private const string BlankIdBase = "magi";
        private static readonly object Locker = new object();
        private int _currentBlankId = 1;

        public string Generate()
        {
            lock (Locker)
            {
                return string.Format("{0}{1}", BlankIdBase, _currentBlankId++);
            }
        }
    }
}