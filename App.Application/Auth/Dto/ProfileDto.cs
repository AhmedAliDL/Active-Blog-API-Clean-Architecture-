namespace App.Application.Auth.Dto
{
    public record ProfileDto
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
    }
}
