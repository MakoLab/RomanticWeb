PREFIX foaf: <http://xmlns.com/foaf/0.1/>

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
		?g foaf:primaryTopic @entityId .
	}
};

DELETE
{
	GRAPH @metaGraph
	{ 
		?g foaf:primaryTopic @entityId . 
	}
}
where
{
	GRAPH @metaGraph
	{ 
		?g foaf:primaryTopic @entityId . 
	}
}