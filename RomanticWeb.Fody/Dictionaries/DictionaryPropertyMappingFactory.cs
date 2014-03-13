using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace RomanticWeb.Fody.Dictionaries
{
    internal class DictionaryPropertyMappingFactory
    {
        private readonly WeaverImports _imports;

        public DictionaryPropertyMappingFactory(WeaverImports imports)
        {
            _imports=imports;
        }

        public DictionaryMappingMeta CreateFromAttributeMapping(PropertyDefinition property)
        {
            var dictionaryAttribute=property.CustomAttributes.Single(at => at.AttributeType.FullName==_imports.DictionaryAttributeTypeRef.FullName);
            var keyAttribute=property.CustomAttributes.SingleOrDefault(at => at.AttributeType.FullName==_imports.KeyAttributeTypeRef.FullName);
            var valueAttribute=property.CustomAttributes.SingleOrDefault(at => at.AttributeType.FullName==_imports.ValueAttributeTypeRef.FullName);

            var injectDictionaryEntriesCode=GetTermUriInjectMethod(dictionaryAttribute);
            var injectKeyMappingCode=GetPropertyMappingCode(keyAttribute)??DefaultKeyMappingCode;
            var injectValueMappingCode=GetPropertyMappingCode(valueAttribute)??DefaultValueMappingCode;

            return new DictionaryMappingMeta(property,injectDictionaryEntriesCode,injectKeyMappingCode,injectValueMappingCode);
        }

        private Action<ILProcessor> GetPropertyMappingCode(CustomAttribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }

            if (attribute.IsQNameConstructor())
            {
                string prefix=attribute.ConstructorArguments[0].Value.ToString();
                string term=attribute.ConstructorArguments[1].Value.ToString();
                return processor => InjectPropertyAsQName(processor,prefix,term);
            }

            if (attribute.IsUriStringConstructor())
            {
                string uriString=attribute.ConstructorArguments[0].Value.ToString();
                return processor => InjectPropertyAsUri(processor,uriString);
            }

            throw new InvalidOperationException(string.Format("Unrecognized '{0}' constructor",attribute.AttributeType.FullName));
        }

        private void DefaultKeyMappingCode(ILProcessor ilProcessor)
        {
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld, _imports.PredicateFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt, _imports.PropertyMapIsUriRef));
        }

        private void DefaultValueMappingCode(ILProcessor ilProcessor)
        {
            ilProcessor.Append(Instruction.Create(OpCodes.Ldsfld,_imports.ObjectFieldRef));
            ilProcessor.Append(Instruction.Create(OpCodes.Callvirt,_imports.PropertyMapIsUriRef));
        }

        private Action<ILProcessor> GetTermUriInjectMethod(CustomAttribute dictionaryAttribute)
        {
            if (dictionaryAttribute.IsQNameConstructor())
            {
                string prefix = dictionaryAttribute.ConstructorArguments[0].Value.ToString();
                string term = dictionaryAttribute.ConstructorArguments[1].Value.ToString();
                return processor => InjectCollectionAsQName(processor,prefix,term);
            }
            
            if (dictionaryAttribute.IsUriStringConstructor())
            {
                string uriString = dictionaryAttribute.ConstructorArguments[0].Value.ToString();
                return processor => InjectCollectionAsUri(processor,uriString);
            }
            
            throw new InvalidOperationException("Unrecognized DictionaryAttribute constructor");
        }

        private void InjectCollectionAsQName(ILProcessor il,string prefix,string termName)
        {
            InjectQNameTerm(il,prefix,termName,_imports.CollectionMapIsQNameRef);
        }

        private void InjectCollectionAsUri(ILProcessor il,string uriString)
        {
            InjectUriStringTerm(il,uriString,_imports.CollectionMapIsUriRef);
        }

        private void InjectPropertyAsQName(ILProcessor il,string prefix,string termName)
        {
            InjectQNameTerm(il,prefix,termName,_imports.PropertyMapIsQNameRef);
        }

        private void InjectPropertyAsUri(ILProcessor il,string uriString)
        {
            InjectUriStringTerm(il,uriString,_imports.PropertyMapIsUriRef);
        }

        private void InjectQNameTerm(ILProcessor il,string prefix,string termName,MethodReference method)
        {
            il.Append(Instruction.Create(OpCodes.Ldstr,prefix));
            il.Append(Instruction.Create(OpCodes.Ldstr,termName));
            il.Append(Instruction.Create(OpCodes.Callvirt,method));
        }

        private void InjectUriStringTerm(ILProcessor il,string uriString,MethodReference method)
        {
            il.Append(Instruction.Create(OpCodes.Ldstr,uriString));
            il.Append(Instruction.Create(OpCodes.Newobj,_imports.UriStringConstructorRef));
            il.Append(Instruction.Create(OpCodes.Callvirt,method));
        }
    }
}