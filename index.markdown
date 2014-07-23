---
layout: home
title: Home
---

## Write c&#35;

{% highlight java %}
IEntityContext ctx = ...

Uri id = new Uri("Tim-Berners-Lee");
IPerson tim = ctx.Create<IPerson>(id);
tim.Name = "Tim";
tim.LastName = "Berners-Lee";
{% endhighlight %}

## Map your contract

{% highlight java %}
[Class("foaf:Person")]
interface IPerson : IEntity
{
    [Property("foaf:givenName")]
    public Name { get; set; }

    [Property("foaf:familyName")]
    public LastName { get; set; }
}
{% endhighlight %}

## Manipulate RDF data

{% highlight css %}
<Tim-Berners-Lee> a foaf:Person ;
    foaf:givenName "Tim"^^xsd:string ;
    foaf:familyName "Berners-Lee"^^xsd:string .
{% endhighlight %}