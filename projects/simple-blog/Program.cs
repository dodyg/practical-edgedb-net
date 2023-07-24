using System.Text;
using EdgeDB;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Markdig;
using FluentValidation;
using Microsoft.AspNetCore.Html;

var builder = WebApplication.CreateBuilder();
builder.Services.AddEdgeDB(EdgeDBConnection.FromInstanceName("simple-blog"), config =>
{
    config.SchemaNamingStrategy = INamingStrategy.SnakeCaseNamingStrategy;
});

builder.Services.AddAntiforgery();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.MapGet("/", async (EdgeDBClient client, HttpContext context, IAntiforgery antiforgery) =>
{
    var blogs = await GetBlogPostAsync(client);

    var token = antiforgery.GetAndStoreTokens(context);
    return Results.Content(Template($$"""
            <div class="row">
                <div class="col-md-6">
                    <form method="POST" action="/">
                        <input name="{{token.FormFieldName}}" type="hidden" value="{{token.RequestToken}}" />
                        <div class="mb-3">
                            <label for="Title" class="form-label">Title</label>
                            <input type="text" class="form-control" id="Title" name="Title">
                        </div>
                        <div class="mb-3">
                            <label for="Body" class="form-label">Body</label>
                            <textarea class="form-control" id="Body" name="Body" rows="3"></textarea>
                        </div>
                        <button type="submit" class="btn btn-primary">Submit</button>
                    </form>
                </div>
                <div class="col-md-6">
                    {{RenderBlogList(blogs)}}
                </div>
            </div>
    """), "text/html");
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
        await antiforgery.ValidateRequestAsync(context);

        FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(input);

        if (validationResult.IsValid == false)
        {
            var blogs = await GetBlogPostAsync(client);
            var token = antiforgery.GetAndStoreTokens(context);
            return Results.Content(Template($$"""
            <div class="row">
                <div class="col-md-6">
                    <form method="POST" action="/">
                        <input name="{{token.FormFieldName}}" type="hidden" value="{{token.RequestToken}}" />
                        <div class="mb-3">
                            <label for="Title" class="form-label">Title</label>
                            <input type="text" class="form-control" id="Title" name="Title" value="{{input.Title}}">
                            {{ShowErrorMessage(validationResult, nameof(input.Title))}}
                        </div>
                        <div class="mb-3">
                            <label for="Body" class="form-label">Body</label>
                            <textarea class="form-control" id="Body" name="Body" rows="3">{{input.Body}}</textarea>
                            {{ShowErrorMessage(validationResult, nameof(input.Body))}}
                        </div>
                        <button type="submit" class="btn btn-primary">Submit</button>
                    </form>
                </div>
                <div class="col-md-6">
                    {{RenderBlogList(blogs)}}
            </div>
    """), "text/html");
        }

        await client.ExecuteAsync($$"""
            INSERT BlogPost {
                title := <str>$Title,
                body := <str>$Body,
                status := BlogPostStatus.Pending
            }
        """, 
        new Dictionary<string, object?>
        {
            { nameof(input.Title), input.Title },
            { nameof(input.Body), input.Body }
        });

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

string RenderBlogList(IReadOnlyCollection<BlogPost?> posts)
{
    MarkdownPipeline config = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    var str = new StringBuilder();

    foreach (var p in posts)
    {
        var title = string.IsNullOrWhiteSpace(p!.Title) ? "" : """<h2 class="card-title">""" + p.Title + "</h2>";
        var x = $$"""
        <div class="card mb-3">
            <div class="card-body">
                {{title}}
                <div class="mt-3">
                    {{Markdown.ToHtml(p.Body, config)}}
                </div>
                <p>
                    created on {{p.DateCreated.SystemDateTime}} with status {{ p.Status }}
                </p>
            </div>
        </div>
        """;

        str.AppendLine(x);
    }

    return str.ToString();
}

static async Task<IReadOnlyCollection<BlogPost?>> GetBlogPostAsync(EdgeDBClient client)
{
    return await client.QueryAsync<BlogPost>($$"""
        SELECT BlogPost {
            id,
            title,
            body,
            status,
            date_created
        }
        order by .date_created desc
    """);
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