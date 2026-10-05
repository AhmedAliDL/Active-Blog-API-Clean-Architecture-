namespace App.Application.Tags.HttpRequests
{
    public record UpdateTagRequest(Guid CategoryId, string? TagName);
}
