using Education.Api.Dtos.Contact;
using Education.Api.Models;
using Education.Api.Services.Abstractions.Email;
using Microsoft.Extensions.Options;

namespace Education.Api.Services.Implementations.Email;

public class EmailTemplateBuilder(IOptions<Company> options) : IEmailTemplateBuilder
{
    private readonly Company _company = options.Value;

    public string BuildPasswordResetRequestTemplate(string recipientName, string otp)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Password Reset</title>
    <style>
      body {{
        margin: 0;
        padding: 0;
        background-color: #f7fafc;
        font-family: Helvetica, Arial, sans-serif;
      }}

      .wrapper {{
        padding: 40px 16px;
      }}

      .container {{
        max-width: 600px;
        margin: 0 auto;
        width: 100%;
      }}

      .logo,
      .footer-logo {{
        display: block;
        margin: auto;
      }}

      .spacer-40 {{
        height: 40px;
      }}

      .spacer-24 {{
        height: 24px;
      }}

      .card {{
        background-color: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 6px;
        padding: 40px;
      }}

      h1 {{
        margin-top: 0;
        font-size: 24px;
        font-weight: 700;
      }}

      .otp-box {{
        margin: 32px 0;
        text-align: center;
      }}

      .otp {{
        display: inline-block;
        padding: 12px 24px;
        background-color: #edf2f7;
        border-radius: 6px;
        font-size: 24px;
        font-weight: bold;
        letter-spacing: 4px;
      }}

      .footer-note {{
        font-size: 14px;
        color: #6c757d;
        margin-top: 32px;
      }}

      .footer-info {{
        color: #718096;
        font-size: 14px;
        text-align: center;
      }}
    </style>
  </head>
  <body>
    <table
      role=""presentation""
      border=""0""
      cellpadding=""0""
      cellspacing=""0""
      width=""100%""
      bgcolor=""#f7fafc""
    >
      <tr>
        <td align=""center"" class=""wrapper"">
          <table
            role=""presentation""
            border=""0""
            cellpadding=""0""
            cellspacing=""0""
            class=""container""
          >
            <!-- Logo -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://assets.bootstrapemail.com/logos/light/square.png""
                  alt=""Company Logo""
                  width=""96""
                  class=""logo""
                />
              </td>
            </tr>

            <tr>
              <td class=""spacer-40"">&nbsp;</td>
            </tr>

            <!-- Main Card -->
            <tr>
              <td class=""card"">
                <h1>Password Reset Request</h1>
                <p>Hi {recipientName},</p>
                <p>
                  We received a request to reset your password. Please use the
                  one-time password (OTP) below to continue:
                </p>

                <div class=""otp-box"">
                  <span class=""otp"">{otp}</span>
                </div>

                <p>Thank you,</p>
                <p>The {_company.Name} Team</p>

                <p class=""footer-note"">
                  If you did not request a password reset, you can safely ignore
                  this email.
                </p>
              </td>
            </tr>

            <tr>
              <td class=""spacer-40"">&nbsp;</td>
            </tr>

            <!-- Footer Logo -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://assets.bootstrapemail.com/logos/light/text.png""
                  width=""160""
                  class=""footer-logo""
                />
              </td>
            </tr>

            <tr>
              <td class=""spacer-24"">&nbsp;</td>
            </tr>

            <!-- Footer Info -->
            <tr>
              <td class=""footer-info"">
                {_company.Email}<br />
                {_company.WebsiteUrl}
              </td>
            </tr>

            <tr>
              <td class=""spacer-24"">&nbsp;</td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>

";
    }

    public string BuildEmailVerificationRequestTemplate(string recipientName, string otp)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Email Verification Code</title>
    <style>
      body {{
        margin: 0;
        padding: 0;
        background-color: #f7fafc;
        font-family: Helvetica, Arial, sans-serif;
      }}

      .wrapper {{
        padding: 40px 16px;
      }}

      .container {{
        max-width: 600px;
        margin: 0 auto;
        width: 100%;
      }}

      .logo,
      .footer-logo {{
        display: block;
        margin: auto;
      }}

      .spacer-40 {{
        height: 40px;
      }}

      .spacer-24 {{
        height: 24px;
      }}

      .card {{
        background-color: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 6px;
        padding: 40px;
      }}

      h1 {{
        margin-top: 0;
        font-size: 24px;
        font-weight: 700;
      }}

      .otp-box {{
        margin: 32px 0;
        text-align: center;
      }}

      .otp {{
        display: inline-block;
        padding: 12px 24px;
        background-color: #edf2f7;
        border-radius: 6px;
        font-size: 24px;
        font-weight: bold;
        letter-spacing: 4px;
      }}

      .footer-note {{
        font-size: 14px;
        color: #6c757d;
        margin-top: 32px;
      }}

      .footer-info {{
        color: #718096;
        font-size: 14px;
        text-align: center;
      }}
    </style>
  </head>
  <body>
    <table
      role=""presentation""
      border=""0""
      cellpadding=""0""
      cellspacing=""0""
      width=""100%""
      bgcolor=""#f7fafc""
    >
      <tr>
        <td align=""center"" class=""wrapper"">
          <table
            role=""presentation""
            border=""0""
            cellpadding=""0""
            cellspacing=""0""
            class=""container""
          >
            <!-- Logo -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://assets.bootstrapemail.com/logos/light/square.png""
                  alt=""Company Logo""
                  width=""96""
                  class=""logo""
                />
              </td>
            </tr>

            <tr>
              <td class=""spacer-40"">&nbsp;</td>
            </tr>

            <!-- Main Card -->
            <tr>
              <td class=""card"">
                <h1>Email Verification Code</h1>
                <p>Hi {recipientName},</p>
                <p>
                  We received a request to verify your email address. Please use
                  the one-time password (OTP) below to continue:
                </p>

                <div class=""otp-box"">
                  <span class=""otp"">{otp}</span>
                </div>

                <p>Thank you,</p>
                <p>The {{companyName}} Team</p>

                <p class=""footer-note"">
                  If you did not request a verification code, please ignore this
                  email.
                </p>
              </td>
            </tr>

            <tr>
              <td class=""spacer-40"">&nbsp;</td>
            </tr>

            <!-- Footer Logo -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://assets.bootstrapemail.com/logos/light/text.png""
                  width=""160""
                  class=""footer-logo""
                />
              </td>
            </tr>

            <tr>
              <td class=""spacer-24"">&nbsp;</td>
            </tr>

            <!-- Footer Info -->
            <tr>
              <td class=""footer-info"">
                {_company.Email}<br />
                {_company.WebsiteUrl}
              </td>
            </tr>

            <tr>
              <td class=""spacer-24"">&nbsp;</td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>

";
    }

    public string BuildContactFormMessageTemplate(ContactDto contactDto)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <meta http-equiv=""x-ua-compatible"" content=""ie=edge"" />
    <meta name=""x-apple-disable-message-reformatting"" />
    <meta
      name=""format-detection""
      content=""telephone=no, date=no, address=no, email=no""
    />
    <title>Contact Form Message</title>
    <style>
      body {{
        background-color: #f7fafc;
        margin: 0;
        padding: 0;
        font-family: Helvetica, Arial, sans-serif;
      }}

      .container {{
        max-width: 600px;
        margin: auto;
        padding: 40px 16px;
      }}

      .card {{
        background-color: #ffffff;
        border: 1px solid #e2e8f0;
        border-radius: 6px;
        padding: 40px;
      }}

      .logo,
      .footer-logo {{
        display: block;
        margin: auto;
      }}

      h1 {{
        margin-top: 0;
        font-size: 24px;
        font-weight: 700;
      }}

      p {{
        margin-bottom: 16px;
      }}

      .footer {{
        color: #718096;
        font-size: 14px;
        text-align: center;
      }}

      .note {{
        font-size: 14px;
        color: #6c757d;
        margin-top: 32px;
      }}

      a {{
        color: #0d6efd;
        text-decoration: none;
      }}
    </style>
  </head>
  <body>
    <div class=""container"">
      <table width=""100%"">
        <tr>
          <td align=""center"">
            <img
              class=""logo""
              src=""https://assets.bootstrapemail.com/logos/light/square.png""
              alt=""Company Logo""
              width=""96""
            />
          </td>
        </tr>
        <tr>
          <td height=""40"">&nbsp;</td>
        </tr>
        <tr>
          <td>
            <div class=""card"">
              <h1>New Contact Form Message</h1>
              <p><strong>From:</strong> {contactDto.Name}</p>
              <p>
                <strong>Email:</strong> <a href=""mailto:{contactDto.Email}"">{{{{Email}}}}</a>
              </p>
              <p><strong>Message:</strong><br />{contactDto.Message}</p>
              <p class=""note"">
                This message was sent via the contact form on your website.
              </p>
            </div>
          </td>
        </tr>
        <tr>
          <td height=""40"">&nbsp;</td>
        </tr>
        <tr>
          <td align=""center"">
            <img
              class=""footer-logo""
              src=""https://assets.bootstrapemail.com/logos/light/text.png""
              width=""160""
            />
          </td>
        </tr>
        <tr>
          <td height=""24"">&nbsp;</td>
        </tr>
        <tr>
          <td class=""footer"">
            {_company.Email}<br />
            {_company.WebsiteUrl}
          </td>
        </tr>
        <tr>
          <td height=""24"">&nbsp;</td>
        </tr>
      </table>
    </div>
  </body>
</html>

";
    }
}
