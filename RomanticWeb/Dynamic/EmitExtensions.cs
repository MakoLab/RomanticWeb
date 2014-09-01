using System;
using System.Reflection.Emit;

namespace RomanticWeb.Dynamic
{
    internal static class EmitExtensions
    {
        public static Type GetOrEmitType(this ModuleBuilder moduleBuilder, string typeName, Func<ModuleBuilder, TypeBuilder> emitType)
        {
            Type mapType;
            if (moduleBuilder.GetType(typeName) != null)
            {
                mapType = moduleBuilder.GetType(typeName, true);
            }
            else
            {
                mapType = emitType(moduleBuilder).CreateType();
            }

            return mapType;
        }
    }
}