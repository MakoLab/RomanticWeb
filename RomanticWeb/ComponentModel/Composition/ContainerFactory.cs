using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RomanticWeb.ComponentModel.Composition
{
    /// <summary>Provides a main source of the container infrastructure for dependency gathering and injection.</summary>
    public class ContainerFactory
    {
        #region Static methods
        /// <summary>Creates an enumeration of types implementing given interface.</summary>
        /// <typeparam name="T">Interface to be implemented by resulting types.</typeparam>
        /// <remarks>Method iterates through loaded assemblies and searches for types that implement given interface.</remarks>
        /// <returns>Enumeration of types implementing given interface.</returns>
        public static IEnumerable<Type> GetTypesImplementing<T>()
        {
            return GetTypesImplementing(typeof(T));
        }

        /// <summary>Creates an enumeration of types implementing given interface.</summary>
        /// <param name="interface">Interface to be implemented by resulting types.</param>
        /// <remarks>Method iterates through loaded assemblies and searches for types that implement given interface.</remarks>
        /// <returns>Enumeration of types implementing given interface.</returns>
        public static IEnumerable<Type> GetTypesImplementing(Type @interface)
        {
            if (!@interface.IsInterface)
            {
                throw new ArgumentOutOfRangeException("T");
            }

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !assembly.IsDynamic
                    from type in assembly.GetTypes()
                    where (type!=@interface)&&(@interface.IsAssignableFrom(type))&&(type.IsInterface)
                    select type);
        }

        /// <summary>Creates an enumeration of instances implementing given interface.</summary>
        /// <typeparam name="T">Interface to be implemented by resulting instances.</typeparam>
        /// <param name="arguments">Optional arguments to be passed to constructor.</param>
        /// <remarks>Method iterates through loaded assemblies and searches for types that contains constructor matching given arguments to create these instances.</remarks>
        /// <returns>Enumeration of instances implementing given interface.</returns>
        public static IEnumerable<T> GetInstancesImplementing<T>(params object[] arguments)
        {
            return GetTypesImplementing<T>(arguments).Select(constructor => (T)constructor.Invoke(arguments));
        }

        /// <summary>Creates an enumeration of types implementing given interface.</summary>
        /// <typeparam name="T">Interface to be implemented by resulting types.</typeparam>
        /// <param name="arguments">Optional arguments to be passed to constructor.</param>
        /// <remarks>Method iterates through loaded assemblies and searches for types that contains constructor matching given arguments.</remarks>
        /// <returns>Enumeration of constructors of types implementing given interface.</returns>
        public static IEnumerable<ConstructorInfo> GetTypesImplementing<T>(params object[] arguments)
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentOutOfRangeException("T");
            }

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !assembly.IsDynamic
                    from type in assembly.GetTypes()
                    where (type!=typeof(T))&&(!type.IsGenericTypeDefinition)&&(typeof(T).IsAssignableFrom(type))&&(!type.IsAbstract)
                    from constructor in type.GetConstructors(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance)
                    let parameters=constructor.GetParameters()
                    where ((arguments==null)||((parameters.Length==arguments.Length)&&(parameters.Where(
                        (parameter,index) => (arguments[index]==null)||(parameter.ParameterType.IsAssignableFrom(arguments[index].GetType()))).Count()==parameters.Length)))
                    select constructor);
        }

        /// <summary>Creates an enumeration of types that are decorated with given attribute.</summary>
        /// <typeparam name="T">Attributes to be filtered by.</typeparam>
        /// <remarks>Method iterates through loaded assemblies.</remarks>
        /// <returns>Enumeration of types decorated with given attribute.</returns>
        public static IEnumerable<Type> GetTypesWithAttribute<T>()
        {
            if (!typeof(Attribute).IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentOutOfRangeException("T");
            }

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    where !assembly.IsDynamic
                    from type in assembly.GetTypes()
                    let attributes=type.GetCustomAttributes(typeof(T),true)
                    where (attributes.Length>0)
                    select type);
        }
        #endregion
    }
}