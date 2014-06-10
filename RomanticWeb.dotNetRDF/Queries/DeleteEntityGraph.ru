DELETE 
{ 
	GRAPH ?g 
	{ 
		?s ?p ?o . 
	} 
} 
WHERE 
{ 
	GRAPH ?g 
	{ 
		?s ?p ?o . 
	} 

	GRAPH @metaGraph 
	{ 
		?g <http://xmlns.com/foaf/0.1/primaryTopic> @entityId . 
	} 
}; 
DELETE 
{ 
	GRAPH @metaGraph 
	{ 
		?g <http://xmlns.com/foaf/0.1/primaryTopic> @entityId . 
	} 
} 
WHERE 
{ 
	GRAPH @metaGraph 
	{ 
		?g <http://xmlns.com/foaf/0.1/primaryTopic> @entityId . 
	} 
}; 