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

var app = builder.Build();

app.MapGet("/", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    return Results.Content(Template("Hello"), "text/html");
});

app.MapPost("/", async ([FromForm] BlogPostInput input, EdgeDBClient client, HttpContext context, IAntiforgery antiforgery, IValidator<BlogPostInput> validator) =>
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
      <title>Simple Blog</title>
      <link href="https://fastly.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-9ndCyUaIbzAi2FUVXJi0CjmCapSmO7SnpJef0486qhLnuZ2cdeRhO02iuK6FUUVM" crossorigin="anonymous">
      <style>
        .error-feedback {
            color: red;
        }
      </style>
    </head>
    <body>
        <div class="container">
            <h1>Simple Blog</h1>
            {{body}}
        </div>
    </body>
    </html>
    """;
}


public class BlogPostInput(string? title, string body)
{
    public string? Title { get; set; } = title;
    public string Body { get; set; } = body;

    public BlogPostInput() : this(null, string.Empty)
    {

    }

    public class Validator : AbstractValidator<BlogPostInput>
    {
        public Validator()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage("Body is required");
        }
    }
}

public enum BlogPostStatus
{
    Active,
    Deleted,
    Pending
}

public record BlogPost(Guid Id, string Title, string Body, BlogPostStatus Status, EdgeDB.DataTypes.DateTime DateCreated);