using System.Reflection;

namespace RomanticWeb.Mapping
{
    internal class MappingFromFluent:MappingFrom
    {
        public MappingFromFluent(MappingBuilder mappingBuilder)
            :base(mappingBuilder)
        {
        }

        public override void FromAssembly(Assembly mappingAssembly)
        {
            MappingBuilder.AddMapping(mappingAssembly,new FluentMappingsRepository(mappingAssembly));
        }
    }
}