using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Enums;
using MediatR;


namespace App.Application.ContentBlocks.Commands.EditContentBlocks
{
    public class EditContentBlocksHandler(IUnitOfWork unitOfWork) : IRequestHandler<EditContentBlocksCommand, int>
    {
        public async Task<int> Handle(EditContentBlocksCommand request, CancellationToken cancellationToken)
        {
            var contentBlocks = await unitOfWork.ContentBlocks.FindAllAsync(i => i.BlogId == request.BlogId, cancellationToken);
            int order = 1;
            foreach (var block in request.Data)
            {
                var existingBlock = contentBlocks.FirstOrDefault(b => b.Id == block.BlockId);
                if (existingBlock != null)
                {
                    if (!(existingBlock.Type == ContentBlockType.Image || existingBlock.Type == ContentBlockType.Video) && BeValidUrl(block.Content))
                    {
                        throw new ArgumentException($"Content for block type {existingBlock.Type} cannot be a URL.");
                    }
                    else
                    {
                        existingBlock.Order = order++; // if user change order
                        existingBlock.Data = block.Content;
                        unitOfWork.ContentBlocks.Update(existingBlock);
                    }
                }
            }
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            return result;
        }
        private static bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp ||
                       uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
