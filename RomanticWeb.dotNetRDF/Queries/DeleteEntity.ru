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
	GRAPH <meta://www.renault.co.uk/>
	{ 
		?g foaf:primaryTopic @entityId . 
	}
}
where
{
	GRAPH <meta://www.renault.co.uk/>
	{ 
		?g foaf:primaryTopic @entityId . 
	}
}