using SQLite;
using System;

namespace CheckListApp.Model
{
    [Table("Users")]
    public class Users
    {
        [PrimaryKey, AutoIncrement]
        public int UserID { get; set; } = 1;

        [Indexed, NotNull]
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Password {  get; set; } = string.Empty;
        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;  // Changed from byte[] to string for base64 hash storage

        [Indexed]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}