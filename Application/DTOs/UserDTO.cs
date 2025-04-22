namespace Monolito_Modular.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}