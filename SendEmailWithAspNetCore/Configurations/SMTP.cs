namespace SendEmailWithAspNetCore.Configurations
{
    public class SMTP
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
