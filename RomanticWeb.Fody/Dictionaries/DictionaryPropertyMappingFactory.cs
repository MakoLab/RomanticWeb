using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class DictionaryPropertyMappingFactory
    {
        private readonly ModuleDefinition _module;
        private readonly WeaverImports _imports;

        public DictionaryPropertyMappingFactory(ModuleDefinition module,WeaverImports imports)
        {
            _module=module;
            _imports=imports;
        }

        public DictionaryPropertyMapping CreateFromAttributeMapping(PropertyDefinition property)
        {
            var dictionaryAttribute = property.CustomAttributes.Single(at => at.AttributeType.FullName == _imports.DictionaryAttributeTypeRef.FullName);

            Action<ILProcessor> injectDictionaryEntriesCode;
            if (dictionaryAttribute.IsQNameDictionaryAttributeConstructor())
            {
                string prefix=dictionaryAttribute.ConstructorArguments[0].Value.ToString();
                string term=dictionaryAttribute.ConstructorArguments[1].Value.ToString();
                injectDictionaryEntriesCode=processor => InjectQNameTerm(processor,prefix,term);
            }
            else if (dictionaryAttribute.IsUriStringDictionaryAttributeConstructor())
            {
                string uriString=dictionaryAttribute.ConstructorArguments[0].Value.ToString();
                injectDictionaryEntriesCode=processor => InjectUriStringTerm(processor,uriString);
            }
            else
            {
                throw new InvalidOperationException("Unrecognized DictionaryAttribute constructor");
            }

            return new DictionaryPropertyMapping(property,injectDictionaryEntriesCode);
        }

        private void InjectQNameTerm(ILProcessor il, string prefix, string termName)
        {
            il.Append(Instruction.Create(OpCodes.Ldstr, prefix));
            il.Append(Instruction.Create(OpCodes.Ldstr, termName));
            il.Append(Instruction.Create(OpCodes.Callvirt, _module.Import(_imports.TermPartIsQNameMethodRef.Resolve().MakeHostInstanceGeneric(_imports.CollectionMapTypeRef))));
        }

        private void InjectUriStringTerm(ILProcessor il, string uriString)
        {
            il.Append(Instruction.Create(OpCodes.Ldstr,uriString));
            il.Append(Instruction.Create(OpCodes.Newobj,_imports.UriStringConstructorRef));
            il.Append(Instruction.Create(OpCodes.Callvirt,_module.Import(_imports.TermPartIsUriMethodRef.Resolve().MakeHostInstanceGeneric(_imports.CollectionMapTypeRef))));
        }
    }
}