DROP SILENT GRAPH @graph ;
	
INSERT DATA
{{ 
	GRAPH @graph 
	{{
		{0} 
	}}
	GRAPH @metaGraph
	{{
		@graph <http://xmlns.com/foaf/0.1/primaryTopic> @entity . 
	}}
}};