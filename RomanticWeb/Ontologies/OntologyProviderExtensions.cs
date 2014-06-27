using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanticWeb.Ontologies
{
    /// <summary>Provides usefull <see cref="IOntologyProvider" /> extension methods.</summary>
    public static class OntologyProviderExtensions
    {
        /// <summary>Tries to resolve a prefix for given Uri.</summary>
        /// <param name="ontologies">Instance of the <see cref="IOntologyProvider"/>.</param>
        /// <param name="uriString">Uri to be resolved.</param>
        /// <returns><see cref="String" /> beeing a prefix of the given Uri or <b>null</b>.</returns>
        public static string ResolveUri(this IOntologyProvider ontologies,string uriString)
        {
            return ontologies.ResolveUri(new Uri(uriString));
        }

        /// <summary>Tries to resolve a prefix for given <see cref="Uri"/>.</summary>
        /// <param name="ontologies">Instance of the <see cref="IOntologyProvider"/>.</param>
        /// <param name="uriString">Uri to be resolved.</param>
        /// <returns><see cref="String" /> beeing a prefix of the given <see cref="Uri"/> or <b>null</b>.</returns>
        public static string ResolveUri(this IOntologyProvider ontologies,Uri uri)
        {
            return ontologies.Ontologies.Where(item => item.BaseUri.AbsoluteUri==uri.AbsoluteUri).Select(item => item.Prefix).FirstOrDefault();
        }
    }
}
