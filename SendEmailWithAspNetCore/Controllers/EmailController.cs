using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SendEmailWithAspNetCore.Configurations;
using SendEmailWithAspNetCore.DTO;
using System.Net;
using System.Net.Mail;

namespace SendEmailWithAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly SMTP _smtpSettings;

        public EmailController(IOptions<SMTP> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        [HttpPost(Name = "SendEmail")]
        public IActionResult SendEmail([FromForm] EmailDto model)
        {
            try
            {
                using (MailMessage mm = new MailMessage(_smtpSettings.Email, model.To))
                {
                    mm.Subject = model.Subject;
                    mm.Body = model.Body;

                    if (!string.IsNullOrEmpty(model.CC))
                        mm.CC.Add(model.CC);

                    if (model.Attachment != null)
                    {
                        string fileName = Path.GetFileName(model.Attachment.FileName);
                        mm.Attachments.Add(new Attachment(model.Attachment.OpenReadStream(), fileName));
                    }
                    mm.IsBodyHtml = false;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = _smtpSettings.Host;
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = _smtpSettings.Port;
                        smtp.Send(mm);

                        return Ok("Email Sent Succesfully!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                return BadRequest($"Error Occurred while sending Email: {ex.Message}!");
            }
        }
    }
}
