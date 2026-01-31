using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }

        [Required]
        public string UserNameOREmail { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
