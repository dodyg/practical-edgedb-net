using EdgeDB;

var builder = WebApplication.CreateBuilder();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("hello-world"), config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

var app = builder.Build();

app.MapGet("/", async (EdgeDBClient client) => {

    var message = await client.QueryAsync<Message>("SELECT Message { content }");

    return Results.Content($$"""
    <html>
    <head>
      <link href="https://fastly.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
    </head>
    <body>
        <div class="container">
            <blockquote class="blockquote">
                SELECT Message { content }
            </blockquote>
            {{message.First().Content}}
        </div>
    </body>
    </html>
    """, "text/html");
});

app.Run();

public record Message(string Content);