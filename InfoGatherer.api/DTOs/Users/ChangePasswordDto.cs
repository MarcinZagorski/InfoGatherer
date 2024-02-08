using System.ComponentModel.DataAnnotations;

namespace InfoGatherer.api.DTOs.Users
{
    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }       
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
