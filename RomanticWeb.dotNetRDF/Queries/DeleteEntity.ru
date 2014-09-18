DELETE WHERE
{
	GRAPH ?g
	{
		?s ?p ?o
	}
	GRAPH @metaGraph 
	{ 
		?g <http://xmlns.com/foaf/0.1/primaryTopic> @entity . 
	}
};

DELETE WHERE 
{ 
	GRAPH @metaGraph 
	{ 
		?g <http://xmlns.com/foaf/0.1/primaryTopic> @entity . 
	} 
}; 