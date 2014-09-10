DROP SILENT GRAPH @graph;

DELETE WHERE 
{ 
	GRAPH @metaGraph 
	{ 
		@graph <http://xmlns.com/foaf/0.1/primaryTopic> @entity . 
	} 
}; 