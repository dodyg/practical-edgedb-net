using EdgeDB;
public class DBQuery(EdgeDBClient client)
{
    public async Task<Namespace?> GetNamespaceAsync(Guid id)
    {
        return await client.QuerySingleAsync<Namespace>($$"""
            SELECT Namespace {
                id,
                name
            } FILTER .id = <uuid>$id
        """, new Dictionary<string, object?>()
        {
            ["id"] = id
        });
    }

    public async Task<IReadOnlyCollection<Namespace>> GetNamespacesAsync()
    {
        return await client.QueryAsync<Namespace>($$"""
            SELECT Namespace {
                id,
                name
            }
        """);
    }
}

public class DBCommand(EdgeDBClient client)
{
    public async Task InsertNamespaceAsync(NamespaceInput input)
    {
        await client.ExecuteAsync($$"""
            INSERT Namespace {
                name := <str>$Name
            }
        """, new Dictionary<string, object?>()
        {
            [nameof(input.Name)] = input.Name
        });
    }

    public async Task InsertTagAsync(TagInput input)
    {
        await client.ExecuteAsync($$"""
            INSERT Tag {
                name := <str>$Name,
                description := <str>$Description,
                namespace := (SELECT Namespace FILTER .name = <uuid>$NamespaceId)
            }
        """, new Dictionary<string, object?>()
        {
            [nameof(input.Name)] = input.Name,
            [nameof(input.Description)] = input.Description,
            [nameof(input.NamespaceId)] = input.NamespaceId
        });
    }

    public async Task InsertResourceAsync(ResourceInput input)
    {
        await client.ExecuteAsync($$"""
                INSERT Resource {
                    title := <str>$Title,
                    url := <str>$Url,
                    tags := (SELECT Tag FILTER .id IN <array<uuid>>$TagIds)
                }
            """, new Dictionary<string, object?>()
        {
            [nameof(input.Title)] = input.Title,
            [nameof(input.Url)] = input.Url,
            [nameof(input.TagIds)] = input.TagIds
        });
    }
}