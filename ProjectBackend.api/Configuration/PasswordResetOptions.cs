namespace ProjectBackend.api.Configuration
{
    public class PasswordResetOptions
    {
        public const string SectionName = "PasswordReset";

        public int TokenLifetimeMinutes { get; set; } = 60;

        public string FrontendBaseUrl { get; set; } = "http://localhost:5173";

        public string ResetPath { get; set; } = "/reset-password";
    }
}
