using System.Text;
using EdgeDB;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Markdig;
using FluentValidation;
using Microsoft.AspNetCore.Html;

var builder = WebApplication.CreateBuilder();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("tagger"), config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

builder.Services.AddAntiforgery();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSingleton<DBCommand>();
builder.Services.AddSingleton<DBQuery>();

var app = builder.Build();

HtmlString ShowErrorMessage(FluentValidation.Results.ValidationResult result, string propertyName)
{
    var found = result.Errors.Where(x => x.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
    if (found is null)
        return HtmlString.Empty;
    else
        return new HtmlString($$"""
            <div class="error-feedback">
                {{found.ErrorMessage}}
            </div>
            """);
}


app.MapGet("/", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template($$"""
    <h1>Resources</h1>
    <div class="row">
        <div class="col-md-6">
            <form>
                <div class="mb-3">
                    <label for="title">Title</label>
                    <input type="text" name="Title" id="title" class="form-control"  />
                </div>
                <div class="mb-3">
                    <label for="url">Url</label>
                    <input type="text" name="Url" id="url" class="form-control"  />
                </div>
                <div class="mb-3">
                    <button type="submit" class="btn btn-primary">Save</button>
                </div>
            </form>
        </div>
        <div class="col-md-6">

        </div>
    </div>
    """), "text/html");
});

app.MapPost("/", async ([FromForm] ResourceInput input, EdgeDBClient client, HttpContext context, IAntiforgery antiforgery, IValidator<ResourceInput> validator) =>
{
    try
    {
        return Results.Redirect("/");
    }
    catch (AntiforgeryValidationException)
    {
        return TypedResults.BadRequest("Invalid anti-forgery token");
    }
});

app.MapGet("/tags", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template($$"""
    <h1>Tags</h1>
    <div class="row">
        <div class="col-md-6">
        </div>
        <div class="col-md-6">

        </div>
    </div>
    """), "text/html");
});

app.MapGet("/namespaces", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template($$"""
    <h1>Namespaces</h1>
    <div class="row">
        <div class="col-md-6">
              <form action="post">
                <div class="mb-3">
                    <label for="title">Name</label>
                    <input type="text" name="Name" id="name" class="form-control"  />
                </div>
                <div class="mb-3">
                    <button type="submit" class="btn btn-primary">Save</button>
                </div>
            </form>
        </div>
        <div class="col-md-6">

        </div>
    </div>
    """), "text/html");
});

app.MapPost("/namespaces", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template($$"""
    <h1>Namespaces</h1>
    <div class="row">
        <div class="col-md-6">
              <form>
                <div class="mb-3">
                    <label for="title">Name</label>
                    <input type="text" name="Name" id="name" class="form-control"  />
                </div>
                <div class="mb-3">
                    <button type="submit" class="btn btn-primary">Save</button>
                </div>
            </form>
        </div>
        <div class="col-md-6">

        </div>
    </div>
    """), "text/html");
});

app.Run();

static string Template(string body)
{
    return $$"""
    <html>
    <head>
      <title>Tagger</title>
      <link href="https://fastly.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
      <style>
        .error-feedback {
            color: red;
        }
      </style>
    </head>
    <body>
        <header>
            <div class="container">
            <nav class="navbar navbar-expand-lg bg-body-tertiary">
                <div class="container-fluid">
                    <a class="navbar-brand" href="/">Tagger</a>
                    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                    </button>
                    <div class="collapse navbar-collapse" id="navbarSupportedContent">
                        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                            <li class="nav-item">
                                <a class="nav-link active" aria-current="page" href="/tags">Tags</a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="/namespaces">Namespaces</a>
                            </li>
                        </ul>
                    </div>
                </div>
                </nav>
            </div>
        </header>
        <div class="container">
            {{body}}
        </div>
    </body>
    </html>
    """;
}

