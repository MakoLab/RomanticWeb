namespace RomanticWeb.DotNetRDF
{
    public class Components : ComponentModel.CompositionRootBase
    {
        public Components()
        {
            SharedComponent<ISparqlCommandFactory, DefaultSparqlCommandFactory>();
        }
    }
}