using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RomanticWeb.MetaData;
using RomanticWeb.Ontologies;

namespace RomanticWeb
{
	public sealed class StaticOntologyProvider:IOntologyProvider
	{
		#region Fields
		private static List<Ontology> _ontologies;
		#endregion

		#region Constructors
		internal StaticOntologyProvider()
		{
			if (_ontologies==null)
			{
				List<Tuple<string,string,List<RdfTerm>>> ontologies=new List<Tuple<string,string,List<RdfTerm>>>();
				BuildAssemblyMappings(ref ontologies);
				_ontologies=ontologies.Select(item => new Ontology(new NamespaceSpecification(item.Item2,item.Item1),item.Item3.ToArray())).ToList();
			}
		}
		#endregion

		#region Properties
		public IEnumerable<Ontology> Ontologies { get { return _ontologies; } }
		#endregion

		#region Private methods
		private void BuildAssemblyMappings(ref List<Tuple<string,string,List<RdfTerm>>> ontologies)
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				BuildTypeMappings(ref ontologies,assembly);
			}
		}

		private void BuildTypeMappings(ref List<Tuple<string,string,List<RdfTerm>>> ontologies,Assembly assembly)
		{
			foreach (Type type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(item => item.GetTypes()))
			{
				foreach (RdfTypeAttribute mapping in type.GetCustomAttributes(typeof(RdfTypeAttribute),true).Cast<RdfTypeAttribute>())
				{
					List<RdfTerm> terms=ontologies.Where(item => item.Item1==mapping.BaseUri.AbsoluteUri).Select(item => item.Item3).FirstOrDefault();
					if (terms==null)
					{
						ontologies.Add(new Tuple<string,string,List<RdfTerm>>(mapping.BaseUri.AbsoluteUri,mapping.Prefix,terms=new List<RdfTerm>()));
					}

					terms.Add(new RdfClass(mapping.ClassName));
				}

				BuildPropertyMappings(ref ontologies,type);
			}
		}

		private void BuildPropertyMappings(ref List<Tuple<string,string,List<RdfTerm>>> ontologies,Type type)
		{
			foreach (PropertyInfo property in type.GetProperties())
			{
				foreach (RdfPropertyAttribute mapping in property.GetCustomAttributes(typeof(RdfPropertyAttribute),true).Cast<RdfPropertyAttribute>())
				{
					List<RdfTerm> terms=ontologies.Where(item => item.Item1==mapping.BaseUri.AbsoluteUri).Select(item => item.Item3).FirstOrDefault();
					if (terms==null)
					{
						ontologies.Add(new Tuple<string,string,List<RdfTerm>>(mapping.BaseUri.AbsoluteUri,mapping.Prefix,terms=new List<RdfTerm>()));
					}

					if (mapping is RdfObjectPropertyAttribute)
					{
						terms.Add(new ObjectProperty(mapping.PropertyName));
					}
					else if (mapping is RdfDatatypePropertyAttribute)
					{
						terms.Add(new DatatypeProperty(mapping.PropertyName));
					}
					else
					{
						terms.Add(new Property(mapping.PropertyName));
					}
				}
			}
		}
		#endregion
	}
}