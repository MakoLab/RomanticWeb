using System.Reflection;

namespace RomanticWeb.Mapping
{
    internal class MappingFromAttributes : MappingFrom
    {
        public MappingFromAttributes(MappingBuilder mappingBuilder)
            :base(mappingBuilder)
        {
        }

        public override void FromAssembly(Assembly mappingAssembly)
        {
            MappingBuilder.AddMapping(mappingAssembly,new AttributeMappingsSource(mappingAssembly));
        }
    }
}