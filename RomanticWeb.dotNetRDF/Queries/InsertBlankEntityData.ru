DELETE WHERE
{{
	GRAPH @graph 
	{{ 
		{0}
	}}
}};

INSERT DATA
{{ 
	GRAPH @graph 
	{{
		{1} 
	}}
}};