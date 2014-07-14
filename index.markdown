---
layout: home
title: Home
---

## Write c&#35;

``` java
IEntityContext ctx = ...

Uri id = new Uri("Tim-Berners-Lee");
IPerson tim = ctx.Create<IPerson>(id);
tim.Name = "Tim";
tim.LastName = "Berners-Lee";
```

## Map your contract

``` java
[Class("foaf:Person")]
interface IPerson : IEntity
{
    [Property("foaf:givenName")]
    public Name { get; set; }

    [Property("foaf:familyName")]
    public LastName { get; set; }
}
```

## Manipulate RDF data

``` css
<Tim-Berners-Lee> a foaf:Person ;
    foaf:givenName "Tim"^^xsd:string ;
    foaf:familyName "Berners-Lee"^^xsd:string .
```