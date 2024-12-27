using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;

namespace papercut_email_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailsController(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail ?? throw new ArgumentNullException(nameof(fluentEmail));
        }


        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail(SendEmail mail)
        {
            var response = await _fluentEmail
                .To(mail.ToEmail)
                .Subject(mail.Subject)
                .Body(mail.Body)
                .SendAsync();

            return response.Successful ? Ok(response) : BadRequest();
        }

        [HttpPost("sendemailattach")]
        public async Task<IActionResult> SendEmailAttach([FromForm] SendEmailAttach mail)
        {
            // Attach the file from a byte array, stream, or path
            var response = await _fluentEmail
                .To(mail.ToEmail)
                .Subject(mail.Subject)
                .Body(mail.Body)
                .Attach(new FluentEmail.Core.Models.Attachment
                {
                    Filename = mail.File.FileName,
                    Data = mail.File.OpenReadStream(),
                    ContentType = mail.File.ContentType
                })
                .SendAsync();

            return response.Successful ? Ok() : BadRequest();
        }
    }

    public record SendEmail(string ToEmail, string Subject, string Body);
    public record SendEmailAttach(string ToEmail, string Subject, string Body, IFormFile File);
}
