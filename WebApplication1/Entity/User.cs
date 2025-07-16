namespace WebApplication1.Entity
{
    public class User
    {
        public int Id { get; set; }
        public  string? Username { get; set; } // Correct placement of 'required'

        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}