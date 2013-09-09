namespace RomanticWeb.Ontologies
{
    public class RdfClass
    {
        public RdfClass(string className)
        {
            ClassName = className;
        }

        public string ClassName { get; private set; }
    }
}