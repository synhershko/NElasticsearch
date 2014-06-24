NElasticsearch
==============

An alternative Elasticsearch client for .NET, built on-the-go while working on real-world projects

## Why?

Yeah, there is an officially released (or, to be released) Elasticsearch client for .NET, and also several others out there, but I found them all not satisfactory for my requirements.

This is a client library which is a result of multiple spikes I made while working on real-world project using Elasticsearch. I believe a consumer library written while dogfooding can shape up to be much better to the developer using it.

You can think of this as an experiment in consumer library deisn. I may either succeed in producing a highly-usable, high-performance client library for the popular Elasticsearch fame, or I may learn a lesson or two along the way. Either way, fun.

And the main goals behind this implementation are more or less the following:

### Because of easier query syntax

This:

```csharp
var filter = new {missing = new {field = "redirect"}};
var match_phrase = new {query = q, analyzer = "hebrew_query"};

var query = new
{
filtered = new
          {
              query = new
                      {
                          @bool = new
                                  {
                                      should = new object[]
                                               {
                                                   new {match_phrase = new{title = match_phrase}},
                                                   new {match_phrase = new{text = match_phrase}},
                                               },
                                      minimum_should_match = 1,
                                  }
                      },
              filter = filter,
          }
};

var results = client.Search<ContentPage>(new
          {
              query = query,

              highlight = new
                          {
                              fields = new
                                       {
                                           title = new {number_of_fragments = 0 },
                                           text = new {},
                                       },
                              pre_tags = new[] {"<b>"}, post_tags = new[]{"</b>"},
                              
                          },

              fields = new[] { "title", "categories", "author" },

              aggregations = new {categories = new
                                  {
                                      terms = new
                                              {
                                                  field = "categories",
                                                  size = 50,
                                                  //filter = filter,
                                              }
                                  }},

              from = pageSize * (page - 1),
              size = pageSize,
          });
```

Is much better than this:

```csharp
var results = await elasticClient.SearchAsync<ContentPage>(
   search => search.Type("contentpage").Index("hebrew-wikipedia-20140208").Query(
       mainQuery => mainQuery.Filtered(filtered => filtered.Query(
        query => query.Bool(b => b.Should(
            bc => bc.MatchPhrase(_ => _.OnField("title").QueryString(q)),
            bc => bc.MatchPhrase(_ => _.OnField("text").QueryString(q))
            ) // .Analyzer("hebrew_query")
          )
         ).Filter(
            filter => filter.And(_ => _.Missing("redirect"))
         )
       ))
       .Highlight(h => h.PreTags("<b>").PostTags("</b>").OnFields(_ => _.OnField("title").NumberOfFragments(0), _ => _.OnField("text")))
       .Fields("title", "categories", "author")
       .FacetTerm("categories", f => f.OnField("categories").FacetFilter(filter => filter.And(_ => _.Missing("redirect"))).Size(50))
       .Size(pageSize).Skip(pageSize * (page - 1))
   );
```

In so many ways.

And we can now also do much better since we can easily introduce extension methods and helpers (upcoming).

### Because query syntax should match closer to the docs

Continuing on the previous point, either you are cloning the JSONs to be used from C#, or you use easy C# syntax that generates it. No real sense in mimicing JSON structure with method calls.

By providing JSON-like syntax in code (anonymous objects) you can now copy-paste from the excellent [Elasticsearch docs](http://www.elasticsearch.org/guide/en/elasticsearch/reference/current/index.html) and not scratch your head too much trying to map JSONs to nested method calls.

Anonymous objects can also be easily reused (like shown above), because they are just objects.

### Because dog-fooding

There's no better way of creating a consumer product than being the consumer yourself.

### Because dependency on JSON.NET is a disaster waiting to happen

Unfortunately the current state of affairs with strong named libraries makes it very probably that in one point or another you will find yourself fighting to make your project run again due to version conflicts

### Because we can leverage RestSharp

RestSharp is an awesome library that can take care of all the REST chatter over HTTP for us reliably, no need to reinvent the wheel.

### Because extension points

Well, that isn't implemented yet, but you'll see.

# More to come...
