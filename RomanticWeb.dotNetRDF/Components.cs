namespace RomanticWeb.DotNetRDF
{
    internal class Components : ComponentModel.CompositionRootBase
    {
        public Components()
        {
            SharedComponent<ISparqlCommandFactory, DefaultSparqlCommandFactory>();
        }
    }
}