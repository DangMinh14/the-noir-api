using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TheNoir.Api.Services;

public class EmailService(HttpClient http, IConfiguration config, ILogger<EmailService> logger) : IEmailService
{
    private const string ResendUrl = "https://api.resend.com/emails";

    // Never throws: a failed email send should not block the request that
    // triggered it (password reset, order confirmation). Failures are only
    // visible in the server log; the caller's own data (reset token, order)
    // is already saved regardless.
    public async Task SendAsync(string to, string subject, string html)
    {
        var apiKey = config["Resend:ApiKey"];
        var from = config["Resend:FromAddress"] ?? "Thé Noir <onboarding@resend.dev>";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("Resend:ApiKey is not configured; skipping email to {To} ({Subject}).", to, subject);
            return;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, ResendUrl)
            {
                Content = JsonContent.Create(new { from, to, subject, html }),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                logger.LogWarning(
                    "Resend rejected email to {To}: {Status} {Body}", to, response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send email to {To} ({Subject}).", to, subject);
        }
    }
}
