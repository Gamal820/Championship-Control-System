using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.ViewModels
{
    public class UserListVM
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class UserCreateVM
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;  
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}

