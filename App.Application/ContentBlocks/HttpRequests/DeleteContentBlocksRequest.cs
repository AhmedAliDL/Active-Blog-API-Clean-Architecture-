namespace App.Application.ContentBlocks.HttpRequests
{
    public record DeleteContentBlocksRequest(HashSet<Guid> ContentBlocksIds);
}
