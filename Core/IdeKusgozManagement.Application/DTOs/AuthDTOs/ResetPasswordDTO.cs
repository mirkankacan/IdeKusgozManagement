namespace IdeKusgozManagement.Application.DTOs.AuthDTOs
{
    public class ResetPasswordDTO
    {
        public string TCNo { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public string VerificationCode { get; set; }
    }
}