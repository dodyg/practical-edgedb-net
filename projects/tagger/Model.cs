using FluentValidation;

public record Namespace(Guid Id, string Name);

public class NamespaceInput(string name)
{
    public string Name { get; set; } = name;

    public NamespaceInput() : this(string.Empty)
    {

    }
}

public class Tag(Guid Id, string Name, string Description, Namespace Namespace);

public class TagInput(string name, string description, string namespaceId)
{
    public string Name { get; set; } = name;
    public string Description { get; set; } = description;

    public string NamespaceId {get; set; } = namespaceId;
}

public class Resource(Guid Id, string Title, string Url, List<Tag> Tags);

public class ResourceInput(string title, string url, List<string> tagIds)
{
    public string Title { get; set; } = title;

    public string Url { get; set; } = url;

    public List<string> TagIds { get; set; } = tagIds;

    public ResourceInput() : this(string.Empty, string.Empty, new())
    {

    }

    public class Validator : AbstractValidator<ResourceInput>
    {
        public Validator()
        {
            RuleFor(x => x.Url).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Url).NotEmpty().WithMessage("Url is required");
        }
    }
}

