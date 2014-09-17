DELETE 
{{
	GRAPH @graph 
	{{ 
		{0}
	}}
}} 
INSERT 
{{ 
	GRAPH @graph 
	{{
		{1} 
	}}
}}
WHERE {{ }};

INSERT 
{{ 
	GRAPH @metaGraph
	{{
		@graph <http://xmlns.com/foaf/0.1/primaryTopic> @entity .
	}}
}}
WHERE {{ }};