using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.ComponentModel.Composition
{
    /// <summary>Provides a main source of the container infrastructure for dependency gathering and injection.</summary>
    public class ContainerFactory
    {
        #region Static methods
        /// <summary>Creates an enumeration of instances implementing given interface.</summary>
        /// <typeparam name="T">Interface to be implemented by resulting instances.</typeparam>
        /// <remarks>Method iterates through loaded assemblies and searches for types that contains a default parameterles constructor to create these instances.</remarks>
        /// <returns>Enumeration of instances implementing given interface.</returns>
        public static IEnumerable<T> GetInstancesImplementing<T>()
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentOutOfRangeException("T");
            }

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where (typeof(T).IsAssignableFrom(type))
                    let constructor=type.GetConstructor(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance,null,new Type[0],null)
                    where constructor!=null
                    select (T)constructor.Invoke(null));
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
                    from type in assembly.GetTypes()
                    let attributes=type.GetCustomAttributes(typeof(T),true)
                    where (attributes.Length>0)
                    select type);
        }
        #endregion
    }
}