using RomanticWeb.Mapping.Model;

namespace RomanticWeb.Mapping.Visitors
{
    public interface IMappingModelVisitor
    {
        void Visit(IEntityMapping entityMapping);

        void Visit(ICollectionMapping entityMapping);
        
        void Visit(IDictionaryMapping entityMapping);

        void Visit(IPropertyMapping entityMapping);

        void Visit(IClassMapping entityMapping);
    }
}