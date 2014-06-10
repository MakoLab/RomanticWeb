INSERT DATA 
{
	GRAPH @graph 
	{
		@subject @predicate @object . 
	}
	GRAPH @metaGraph 
	{
		@graph <http://xmlns.com/foaf/0.1/primaryTopic> @entityId . 
	}
}; 