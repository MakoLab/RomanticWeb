using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.Mapping.Providers;

namespace RomanticWeb.Collections
{
    /// <summary>Extension method to implement topologic sorting. Will return a list where items with dependencies will come after items without dependencies.</summary>
    /// <remarks>From http://stackoverflow.com/questions/21189222/topological-sort-with-support-for-cyclic-dependencies .</remarks>
    internal static class TopologicSortExtensions
    {
        /// <summary>Sorts a list of items with dependencies such that items without dependencies come first.</summary>
        /// <typeparam name="T">Type of list, which must implement <see cref="ITopologicSortable"/>.</typeparam>
        /// <param name="graph">List of items to sort.</param>
        /// <returns>A sorted list of item, where items with dependencies after items without dependencies.</returns>
        internal static IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> TopologicSort(this IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> graph)
        {
            int[] state = new int[graph.Count];
            List<KeyValuePair<Type, IList<IEntityMappingProvider>>> list = new List<KeyValuePair<Type, IList<IEntityMappingProvider>>>();

            for (int index = 0; index < graph.Count; index++)
            {
                state[index] = 1;
            }

            for (int index = 0; index < graph.Count; index++)
            {
                Visit(graph, index, list, state);
            }

            return list;
        }

        private static void Visit(IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> graph, int index, IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> list, int[] state)
        {
            if (state[index] == -1)
            {
                return;
            }

            if (state[index] == 0)
            {
                return;
            }

            state[index] = 0;
            foreach (KeyValuePair<Type, IList<IEntityMappingProvider>> neighbour in GetNeighbours(graph, graph[index]))
            {
                Visit(graph, graph.IndexOf(neighbour), list, state);
            }

            state[index] = -1;
            list.Add(graph[index]);
        }

        private static IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> GetNeighbours(IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> graph, KeyValuePair<Type, IList<IEntityMappingProvider>> node)
        {
            List<KeyValuePair<Type, IList<IEntityMappingProvider>>> neighbours = graph.Where(type => type.Key.DependsOn(node.Key)).ToList();

            IList<KeyValuePair<Type, IList<IEntityMappingProvider>>> unique = new List<KeyValuePair<Type, IList<IEntityMappingProvider>>>();
            neighbours.ForEach(type =>
            {
                if (!unique.Any(other => other.Key == type.Key))
                {
                    unique.Add(type);
                }
            });

            if (unique.IndexOf(node) > -1)
            {
                unique.Remove(node);
            }

            return unique;
        }

        private static bool DependsOn(this Type type, Type another)
        {
            if ((another.IsAssignableFrom(type)) || (another.IsAssignableFromSpecificGeneric(type)))
            {
                return true;
            }

            int typeAssemblyHashCode = type.Assembly.GetHashCode();
            int anotherAssemblyHashCode = another.Assembly.GetHashCode();
            if (typeAssemblyHashCode != anotherAssemblyHashCode)
            {
                return (type.Assembly.GetReferencedAssemblies().Contains(another.Assembly.GetName()));
            }

            return false;
        }
    }
}