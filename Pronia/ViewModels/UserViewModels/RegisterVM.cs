using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels.UserViewModels
{
    public class RegisterVM
    {
        [Required,MaxLength(32),MinLength(3)]
        public string UserName { get; set; } = string.Empty;
        [Required,MaxLength(32),MinLength(3)]
        public string Firstname { get; set; } = string.Empty;
        [Required,MaxLength(32),MinLength(3)]
        public string Lastname { get; set; } = string.Empty;
        [Required,MaxLength(256),EmailAddress]
        public string EmailAddress { get; set; } = string.Empty;
        [Required,MaxLength(256),MinLength(6),DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required,MaxLength(256),MinLength(6),DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
