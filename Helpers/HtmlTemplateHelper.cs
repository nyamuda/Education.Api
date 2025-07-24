namespace Education.Api.Helpers;

public class HtmlTemplateHelper
{
    public string AppName { get; }
    public string AppEmail { get; }
    public string AppWebsiteUrl { get; }
    public string AppPhone { get;}

    public HtmlTemplateHelper(IConfiguration config)
    {
        AppName =
            config.GetValue<string>("AppInfo:Name")
            ?? throw new InvalidOperationException(
                "Missing configuration: 'AppInfo:Name' is not set."
            );

        AppEmail =
            config.GetValue<string>("AppInfo:Email")
            ?? throw new InvalidOperationException(
                "Missing configuration: 'AppInfo:Email' is not set."
            );
        AppWebsiteUrl =
            config.GetValue<string>("AppInfo:WebsiteUrl")
            ?? throw new InvalidOperationException(
                "Missing configuration: 'AppInfo:WebsiteUrl' is not set."
            );

       AppPhone =
            config.GetValue<string>("AppInfo:Phone")
            ?? throw new InvalidOperationException(
                "Missing configuration: 'AppInfo:Phone' is not set."
            );
    }

    //Template for resetting password
    public static string PasswordResetRequestTemplate(
       
        string recipientName,
        string otp
       
    )
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
                <p>The {_appName} Team</p>

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
                {{companyEmail}}<br />
                {{companyAddress}}
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

    public static string EmailVerificationRequestTemplate(
        string verificationUrl,
        string recipientName,
        string companyName,
        string companyEmail,
        string companyAddress
    )
    {
        return $@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html>
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
    <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
    <meta name=""x-apple-disable-message-reformatting"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <meta name=""format-detection"" content=""telephone=no, date=no, address=no, email=no"">
    <style type=""text/css"">
      body,table,td{{font-family:Helvetica,Arial,sans-serif !important}}.ExternalClass{{width:100%}}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{{line-height:150%}}a{{text-decoration:none}}*{{color:inherit}}a[x-apple-data-detectors],u+#body a,#MessageViewBody a{{color:inherit;text-decoration:none;font-size:inherit;font-family:inherit;font-weight:inherit;line-height:inherit}}img{{-ms-interpolation-mode:bicubic}}table:not([class^=s-]){{font-family:Helvetica,Arial,sans-serif;mso-table-lspace:0pt;mso-table-rspace:0pt;border-spacing:0px;border-collapse:collapse}}table:not([class^=s-]) td{{border-spacing:0px;border-collapse:collapse}}@media screen and (max-width: 600px){{.w-full,.w-full>tbody>tr>td{{width:100% !important}}.w-24,.w-24>tbody>tr>td{{width:96px !important}}.w-40,.w-40>tbody>tr>td{{width:160px !important}}.p-lg-10:not(table),.p-lg-10:not(.btn)>tbody>tr>td,.p-lg-10.btn td a{{padding:0 !important}}.p-3:not(table),.p-3:not(.btn)>tbody>tr>td,.p-3.btn td a{{padding:12px !important}}.p-6:not(table),.p-6:not(.btn)>tbody>tr>td,.p-6.btn td a{{padding:24px !important}}*[class*=s-lg-]>tbody>tr>td{{font-size:0 !important;line-height:0 !important;height:0 !important}}.s-4>tbody>tr>td{{font-size:16px !important;line-height:16px !important;height:16px !important}}.s-6>tbody>tr>td{{font-size:24px !important;line-height:24px !important;height:24px !important}}.s-10>tbody>tr>td{{font-size:40px !important;line-height:40px !important;height:40px !important}}}}
    </style>
  </head>
  <body bgcolor=""#f7fafc"" style=""margin: 0; padding: 0;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" bgcolor=""#f7fafc"">
      <tr>
        <td align=""center"" style=""padding: 40px 16px;"">
          <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
            <tr>
              <td align=""center"">
                <img src=""https://assets.bootstrapemail.com/logos/light/square.png"" alt=""Company Logo"" width=""96"" style=""display: block; margin: auto;"">
              </td>
            </tr>
            <tr>
              <td height=""40"">&#160;</td>
            </tr>
            <tr>
              <td bgcolor=""#ffffff"" style=""border: 1px solid #e2e8f0; border-radius: 6px; padding: 40px;"">
                <h1 style=""margin-top: 0; font-size: 24px; font-weight: 700;"">Password Reset Request</h1>
                <p>Hi {recipientName},</p>
                <p>To ensure continued access to your account, please verify your email address using the button below.</p>
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""margin: 24px 0;"">
                  <tr>
                    <td align=""center"" bgcolor=""#0d6efd"" style=""border-radius: 6px;"">
                      <a href=""{verificationUrl}"" style=""display: inline-block; padding: 12px 24px; color: #ffffff; background-color: #0d6efd; font-weight: 700; text-decoration: none; border-radius: 6px;"">Confirm Email</a>
                    </td>
                  </tr>
                </table>
                <p>Thank you,</p>
                <p>The {companyName} Team</p>
                <p style=""font-size: 14px; color: #6c757d; margin-top: 32px;"">
                  If this wasn’t you, please disregard this message.
                </p>
              </td>
            </tr>
            <tr>
              <td height=""40"">&#160;</td>
            </tr>
            <tr>
              <td align=""center"">
                <img src=""https://assets.bootstrapemail.com/logos/light/text.png"" width=""160"" style=""display: block; margin: auto;"">
              </td>
            </tr>
            <tr>
              <td height=""24"">&#160;</td>
            </tr>
            <tr>
              <td align=""center"" style=""color: #718096; font-size: 14px;"">
                {companyEmail}<br>
                {companyAddress}
              </td>
            </tr>
            <tr>
              <td height=""24"">&#160;</td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
";
    }

    //Template for contact us
    public static string ContactFormMessageTemplate(
        ContactDto contactDto,
        string companyEmail,
        string companyAddress
    )
    {
        return $@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html>
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
    <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
    <meta name=""x-apple-disable-message-reformatting"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <meta name=""format-detection"" content=""telephone=no, date=no, address=no, email=no"">
    <style type=""text/css"">
      body,table,td{{font-family:Helvetica,Arial,sans-serif !important}}.ExternalClass{{width:100%}}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{{line-height:150%}}a{{text-decoration:none}}*{{color:inherit}}a[x-apple-data-detectors],u+#body a,#MessageViewBody a{{color:inherit;text-decoration:none;font-size:inherit;font-family:inherit;font-weight:inherit;line-height:inherit}}img{{-ms-interpolation-mode:bicubic}}table:not([class^=s-]){{font-family:Helvetica,Arial,sans-serif;mso-table-lspace:0pt;mso-table-rspace:0pt;border-spacing:0px;border-collapse:collapse}}table:not([class^=s-]) td{{border-spacing:0px;border-collapse:collapse}}@media screen and (max-width: 600px){{.w-full,.w-full>tbody>tr>td{{width:100% !important}}.w-24,.w-24>tbody>tr>td{{width:96px !important}}.w-40,.w-40>tbody>tr>td{{width:160px !important}}.p-lg-10:not(table),.p-lg-10:not(.btn)>tbody>tr>td,.p-lg-10.btn td a{{padding:0 !important}}.p-3:not(table),.p-3:not(.btn)>tbody>tr>td,.p-3.btn td a{{padding:12px !important}}.p-6:not(table),.p-6:not(.btn)>tbody>tr>td,.p-6.btn td a{{padding:24px !important}}*[class*=s-lg-]>tbody>tr>td{{font-size:0 !important;line-height:0 !important;height:0 !important}}.s-4>tbody>tr>td{{font-size:16px !important;line-height:16px !important;height:16px !important}}.s-6>tbody>tr>td{{font-size:24px !important;line-height:24px !important;height:24px !important}}.s-10>tbody>tr>td{{font-size:40px !important;line-height:40px !important;height:40px !important}}}}
    </style>
  </head>
  <body bgcolor=""#f7fafc"" style=""margin: 0; padding: 0;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" bgcolor=""#f7fafc"">
      <tr>
        <td align=""center"" style=""padding: 40px 16px;"">
          <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
            <tr>
              <td align=""center"">
                <img src=""https://assets.bootstrapemail.com/logos/light/square.png"" alt=""Company Logo"" width=""96"" style=""display: block; margin: auto;"">
              </td>
            </tr>
            <tr>
              <td height=""40"">&#160;</td>
            </tr>
            <tr>
              <td bgcolor=""#ffffff"" style=""border: 1px solid #e2e8f0; border-radius: 6px; padding: 40px;"">
                <h1 style=""margin-top: 0; font-size: 24px; font-weight: 700;"">New Contact Form Message</h1>
                <p><strong>From:</strong> {contactDto.Name}</p>
                <p><strong>Email:</strong> <a href=""mailto:{contactDto.Email}"" style=""color: #0d6efd;"">{contactDto.Email}</a></p>
                <p><strong>Message:</strong><br>{contactDto.Message}</p>
                <p style=""font-size: 14px; color: #6c757d; margin-top: 32px;"">
                   This message was sent via the contact form on your website.
                </p>
              </td>
            </tr>
            <tr>
              <td height=""40"">&#160;</td>
            </tr>
            <tr>
              <td align=""center"">
                <img src=""https://assets.bootstrapemail.com/logos/light/text.png"" width=""160"" style=""display: block; margin: auto;"">
              </td>
            </tr>
            <tr>
              <td height=""24"">&#160;</td>
            </tr>
            <tr>
              <td align=""center"" style=""color: #718096; font-size: 14px;"">
                {companyEmail}<br>
                {companyAddress}
              </td>
            </tr>
            <tr>
              <td height=""24"">&#160;</td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
";
    }

    // Template for booking-related notifications
    public static string BookingNotificationTemplate(BookingNotificationData data)
    {
        //get the name of the course whose package was booked
        var courseName = data.Booking.CoursePackage.Course is not null
            ? data.Booking.CoursePackage.Course.Title
            : "";
        //get the name, email and phone of the creator of the booking
        var (bookingCreatorName, bookingCreatorEmail, bookingCreatorPhone) =
            data.Booking.GetBookerIdentity();
        //convert the scheduled time to local time and format it
        var scheduledAt = DateHelper
            .ConvertUtcToLocalTime(data.Booking.ScheduledAt)
            .ToString($@"ddd, MMMM dd, yyyy ""at"" h:mm tt"); //E.g. Tue, April 22, 2025 at 7:03 AM

        //include action button HTML if the `actionUrl` was provided
        var actionButtonHtml = string.IsNullOrEmpty(data.ActionUrl)
            ? ""
            : @$" <table class=""btn btn-red-500 rounded-full px-6 w-full w-lg-48"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-radius: 9999px; border-collapse: separate !important; width: 192px;"" width=""192"">
                                <tbody>
                                  <tr>
                                    <td style=""line-height: 24px; font-size: 16px; border-radius: 9999px; width: 192px; margin: 0;"" align=""center"" bgcolor=""#dc3545"" width=""192"">
                                      <a href=""{data.ActionUrl}"" style=""color: #ffffff; font-size: 16px; font-family: Helvetica, Arial, sans-serif; text-decoration: none; border-radius: 9999px; line-height: 20px; display: block; font-weight: normal; white-space: nowrap; background-color: #dc3545; padding: 8px 24px; border: 1px solid #dc3545;"">
                                        {data.ActionText}
                                      </a>
                                    </td>
                                  </tr>
                                </tbody>
                              </table>";

        return $@"

<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html>
  <head>
    <!-- Compiled with Bootstrap Email version: 1.3.1 -->
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
    <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
    <meta name=""x-apple-disable-message-reformatting"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <meta name=""format-detection"" content=""telephone=no, date=no, address=no, email=no"">
    <style type=""text/css"">
      body,table,td{{font-family:Helvetica,Arial,sans-serif !important}}.ExternalClass{{width:100%}}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div{{line-height:150%}}a{{text-decoration:none}}*{{color:inherit}}a[x-apple-data-detectors],u+#body a,#MessageViewBody a{{color:inherit;text-decoration:none;font-size:inherit;font-family:inherit;font-weight:inherit;line-height:inherit}}img{{-ms-interpolation-mode:bicubic}}table:not([class^=s-]){{font-family:Helvetica,Arial,sans-serif;mso-table-lspace:0pt;mso-table-rspace:0pt;border-spacing:0px;border-collapse:collapse}}table:not([class^=s-]) td{{border-spacing:0px;border-collapse:collapse}}@media screen and (max-width: 600px){{.w-lg-48,.w-lg-48>tbody>tr>td{{width:auto !important}}.w-full,.w-full>tbody>tr>td{{width:100% !important}}.w-16,.w-16>tbody>tr>td{{width:64px !important}}.p-lg-10:not(table),.p-lg-10:not(.btn)>tbody>tr>td,.p-lg-10.btn td a{{padding:0 !important}}.p-2:not(table),.p-2:not(.btn)>tbody>tr>td,.p-2.btn td a{{padding:8px !important}}.pr-4:not(table),.pr-4:not(.btn)>tbody>tr>td,.pr-4.btn td a,.px-4:not(table),.px-4:not(.btn)>tbody>tr>td,.px-4.btn td a{{padding-right:16px !important}}.pl-4:not(table),.pl-4:not(.btn)>tbody>tr>td,.pl-4.btn td a,.px-4:not(table),.px-4:not(.btn)>tbody>tr>td,.px-4.btn td a{{padding-left:16px !important}}.pr-6:not(table),.pr-6:not(.btn)>tbody>tr>td,.pr-6.btn td a,.px-6:not(table),.px-6:not(.btn)>tbody>tr>td,.px-6.btn td a{{padding-right:24px !important}}.pl-6:not(table),.pl-6:not(.btn)>tbody>tr>td,.pl-6.btn td a,.px-6:not(table),.px-6:not(.btn)>tbody>tr>td,.px-6.btn td a{{padding-left:24px !important}}.pt-8:not(table),.pt-8:not(.btn)>tbody>tr>td,.pt-8.btn td a,.py-8:not(table),.py-8:not(.btn)>tbody>tr>td,.py-8.btn td a{{padding-top:32px !important}}.pb-8:not(table),.pb-8:not(.btn)>tbody>tr>td,.pb-8.btn td a,.py-8:not(table),.py-8:not(.btn)>tbody>tr>td,.py-8.btn td a{{padding-bottom:32px !important}}*[class*=s-lg-]>tbody>tr>td{{font-size:0 !important;line-height:0 !important;height:0 !important}}.s-4>tbody>tr>td{{font-size:16px !important;line-height:16px !important;height:16px !important}}.s-6>tbody>tr>td{{font-size:24px !important;line-height:24px !important;height:24px !important}}}}
    </style>
  </head>
  <body class=""bg-red-100"" style=""outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border-width: 0;"" bgcolor=""#f8d7da"">
    <table class=""bg-red-100 body"" valign=""top"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""outline: 0; width: 100%; min-width: 100%; height: 100%; -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; font-family: Helvetica, Arial, sans-serif; line-height: 24px; font-weight: normal; font-size: 16px; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; box-sizing: border-box; color: #000000; margin: 0; padding: 0; border-width: 0;"" bgcolor=""#f8d7da"">
      <tbody>
        <tr>
          <td valign=""top"" style=""line-height: 24px; font-size: 16px; margin: 0;"" align=""left"" bgcolor=""#f8d7da"">
            <table class=""container"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"">
              <tbody>
                <tr>
                  <td align=""center"" style=""line-height: 24px; font-size: 16px; margin: 0; padding: 0 16px;"">
                    <!--[if (gte mso 9)|(IE)]>
                      <table align=""center"" role=""presentation"">
                        <tbody>
                          <tr>
                            <td width=""600"">
                    <![endif]-->
                    <table align=""center"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%; max-width: 600px; margin: 0 auto;"">
                      <tbody>
                        <tr>
                          <td style=""line-height: 24px; font-size: 16px; margin: 0;"" align=""left"">

                            <table class=""s-6 w-full"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" width=""100%"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 24px; width: 100%; height: 24px; margin: 0;"" align=""left"" width=""100%"" height=""24"">
                                    &#160;
                                  </td>
                                </tr>
                              </tbody>
                            </table>

                            <img class=""w-16"" src=""https://assets.bootstrapemail.com/logos/light/square.png"" style=""height: auto; line-height: 100%; outline: none; text-decoration: none; display: block; width: 64px; border-style: none; border-width: 0;"" width=""64"" alt=""Company Logo"">

                            <table class=""s-6 w-full"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" width=""100%"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 24px; width: 100%; height: 24px; margin: 0;"" align=""left"" width=""100%"" height=""24"">
                                    &#160;
                                  </td>
                                </tr>
                              </tbody>
                            </table>

                            <div class=""space-y-4"">
                              <h1 class=""text-4xl fw-800"" style=""padding-top: 0; padding-bottom: 0; font-weight: 800 !important; font-size: 36px; line-height: 43.2px; margin: 0;"" align=""left"">
                               {data.Title}
                              </h1>
<p>Hi {data.RecipientName},</p>
<p>{data.MessageBody}</p>

                              

                              <!-- Button -->
                             {actionButtonHtml}

                            </div>

                            <table class=""s-6 w-full"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" width=""100%"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 24px; width: 100%; height: 24px; margin: 0;"" align=""left"" width=""100%"" height=""24"">
                                    &#160;
                                  </td>
                                </tr>
                              </tbody>
                            </table>

                            <!-- Booking Details card with white background -->
                            <table class=""card rounded-3xl px-4 py-8 p-lg-10"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""border-radius: 24px; border-collapse: separate !important; width: 100%; overflow: hidden; border: 1px solid #e2e8f0;"" bgcolor=""#ffffff"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 16px; width: 100%; border-radius: 24px; margin: 0; padding: 40px;"" align=""left"" bgcolor=""#ffffff"">

                                    <h3 style=""padding-top: 0; padding-bottom: 0; font-weight: 500; font-size: 28px; line-height: 33.6px; margin: 0;"" align=""center"">
                                      Booking Details
                                    </h3>

                                    <table class=""p-2 w-full"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" widtha=""100%"">
                                      <tbody>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Name</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{bookingCreatorName}</td>
                                        </tr>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Email</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{bookingCreatorEmail}</td>
                                        </tr>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Phone</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{bookingCreatorPhone}</td>
                                        </tr>
                                        
                                        <tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Course</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{courseName}</td>
                                        </tr>
                                        <tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Course Package</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{data.Booking .CoursePackage .Name}</td>
                                        </tr>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Transmission</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{data.Booking .CoursePackage .Transmission}</td>
                                        </tr>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Price</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{data.Booking .CoursePackage .Price}</td>
                                        </tr>
<tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Scheduled At</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{scheduledAt}</td>
                                        </tr>
                                        <tr>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""left"">Pickup Address</td>
                                          <td style=""line-height: 24px; font-size: 16px; padding: 8px;"" align=""right"">{data.Booking.PickupAddress}</td>
                                        </tr>
                                        
                                      </tbody>
                                    </table>

                                  </td>
                                </tr>
                              </tbody>
                            </table>

                            <table class=""s-6 w-full"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" width=""100%"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 24px; width: 100%; height: 24px; margin: 0;"" align=""left"" width=""100%"" height=""24"">&#160;</td>
                                </tr>
                              </tbody>
                            </table>

                            <p style=""line-height: 24px; font-size: 16px; margin: 0;"" align=""left"">
                              {data.SendOff},
                            </p>
<p>The {data.CompanyName} Team</p>

<p style=""font-size: 14px; color: #6c757d; margin-top: 32px;"">
                 If you have any questions, contact us at <a href=""mailto:{data.CompanyEmail}"" style=""color: #0d6efd;"">{data.CompanyEmail}</a>
                </p>

                            <table class=""s-6 w-full"" role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width: 100%;"" width=""100%"">
                              <tbody>
                                <tr>
                                  <td style=""line-height: 24px; font-size: 24px; width: 100%; height: 24px; margin: 0;"" align=""left"" width=""100%"" height=""24"">&#160;</td>
                                </tr>
                              </tbody>
                            </table>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                    <!--[if (gte mso 9)|(IE)]>
                    </td>
                  </tr>
                </tbody>
              </table>
                    <![endif]-->
                  </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table>
  <script data-cfasync=""false"" src=""/cdn-cgi/scripts/5c5dd728/cloudflare-static/email-decode.min.js""></script></body>
</html>


";
    }
}
