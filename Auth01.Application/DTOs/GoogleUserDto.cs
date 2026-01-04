namespace Auth01.Application.DTOs
{
    public class GoogleUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
        public string Subject { get; set; } // Google User ID
    }
}
