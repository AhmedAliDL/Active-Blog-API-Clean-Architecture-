namespace App.Application.Tags.Dto
{
    public class TagDetailsDto
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }

    }
}
