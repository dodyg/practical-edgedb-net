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

app.MapGet("/", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template($$"""
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
    static HtmlString ShowErrorMessage(FluentValidation.Results.ValidationResult result, string propertyName)
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

    try
    {
        return Results.Redirect("/");
    }
    catch (AntiforgeryValidationException)
    {
        return TypedResults.BadRequest("Invalid anti-forgery token");
    }
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
        <div class="container">
            <h1>Tagger</h1>
            {{body}}
        </div>
    </body>
    </html>
    """;
}

