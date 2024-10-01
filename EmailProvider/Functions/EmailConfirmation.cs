using Azure;
using Azure.Communication.Email;
using EmailProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Newtonsoft.Json;
using System.Diagnostics;

namespace EmailProvider.Functions;

public class EmailConfirmation(EmailClient emailClient)
{
	private readonly EmailClient _emailClient = emailClient;

	[Function(nameof(EmailConfirmation))]
	public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
	{
		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		dynamic data = JsonConvert.DeserializeObject(requestBody)!;
		string email = data!.email;

		if (!string.IsNullOrEmpty(email))
		{
			var subject = "Confirmation";

			var emailRequest = new EmailRequest()
			{
				To = email,
				Subject = subject,
				HtmlBody = @"
				<html>
					<body>
						<h1>Thank you for contacting us!</h1>
						<p>We have recieved your request and will contact you shortly.</p>
						<p>Best regards,<br>Onatrix</p>
						<footer>
							<p>This is a automatic generated message, please do not respond to this message.</p>
						</footer>
					</body>
				</html>",
				PlainText = "Thank you for contacting us"
			};

			bool emailSent = await SendEmailAsync(emailRequest);

			if (emailSent)
			{
				return new OkObjectResult($"Confirmation email sent to {email}");
			}
			else
			{
				return new BadRequestObjectResult("Email could not be sent.");
			}
		}
		return new BadRequestObjectResult("Please pass a valid email.");
	}

	public async Task<bool> SendEmailAsync(EmailRequest emailRequest)
	{
		try
		{
			var result = await _emailClient.SendAsync(
				WaitUntil.Completed,

				senderAddress: Environment.GetEnvironmentVariable("SenderAddress"),
				recipientAddress: emailRequest.To,
				subject: emailRequest.Subject,
				htmlContent: emailRequest.HtmlBody,
				plainTextContent: emailRequest.PlainText);

			if (result.HasCompleted)
				return true;
		}
		catch (Exception ex)
		{
			Debug.WriteLine(ex);
		}

		return false;
	}
	//private async Task SendEmailAsync(string toEmail, string subject, string body)
	//{
	//	var emailMessage = new EmailRequest(
	//		senderAddress: "no-reply@yourdomain.com",
	//		content: new EmailContent(subject)
	//		{
	//			Html = body
	//		}
	//	);
	//	emailMessage.Recipients.Add(new EmailRecipient(toEmail));

	//	await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
	//}
}
