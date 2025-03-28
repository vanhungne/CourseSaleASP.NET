using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Project_Cursus_Group3.Data.Data;
using Project_Cursus_Group3.Data.Interfaces;
using Project_Cursus_Group3.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Project_Cursus_Group3.Service.Services
{
    public class RatePriceService : IRatePriceService
    {
        private readonly IConfiguration _configuration;
        private readonly string _configPath;
        private readonly CursusDbContext _context;
        private readonly IEmailSender _emailSender;

        public RatePriceService(IConfiguration configuration, CursusDbContext dbcontext, IEmailSender emailSender)
        {
            _configuration = configuration;
            _context = dbcontext;
            _emailSender = emailSender;
            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        }
        public RatePriceService(IConfiguration configuration)
        {
            _configuration = configuration;

            _configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        }

        public double GetCurrentRatePrice()
        {
            var rateString = _configuration["RatePrice:rate"];
            if (double.TryParse(rateString, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
            {
                return rate;
            }
            return 1.0;
        }

        public async Task<bool> UpdateRatePriceAsync(double newRate)
        {
            try
            {
                var json = await File.ReadAllTextAsync(_configPath);
                var jsonObj = JObject.Parse(json);

                jsonObj["RatePrice"]["rate"] = newRate.ToString(CultureInfo.InvariantCulture);

                await File.WriteAllTextAsync(_configPath, jsonObj.ToString());

                ((IConfigurationRoot)_configuration).Reload();

                var instructors = await _context.User.Where(u => u.RoleId == 2).ToListAsync();

                var emailSubject = "Rate Price Update Notification";
                var emailBody = DesignRateUpdateEmailBody(newRate * 100, DateTime.Now);
                foreach (var instructor in instructors)
                {
                    await _emailSender.EmailSendAsync(instructor.Email, emailSubject, emailBody);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public string DesignRateUpdateEmailBody(double newRate, DateTime effectiveDate)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; text-align: center; background-color: #f8f8f8; padding: 20px;'>
        <div style='max-width: 600px; background-color: #ffffff; margin: 0 auto; padding: 30px; border-radius: 10px; box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);'>
            <h1 style='color: #333333; font-size: 24px;'>Invoice for Rate Price Update</h1>
            <p style='font-size: 18px; color: #666666;'>Dear Instructor,</p>
            <p style='font-size: 16px; color: #555555;'>We would like to inform you that a new fee of <strong>{newRate}%</strong> will be applied to the courses you sell on our platform starting from <strong>{effectiveDate:MMMM dd, yyyy}</strong>.</p>
            <div style='background-color: #f2f2f2; padding: 15px; margin: 20px 0; border-radius: 8px; text-align: left;'>
                <h3 style='color: #333333;'>Fee Details</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr>
                        <th style='text-align: left; padding: 8px 0; color: #666666;'>New Rate Fee:</th>
                        <td style='text-align: right; padding: 8px 0; color: #333333;'>{newRate}%</td>
                    </tr>
                    <tr>
                        <th style='text-align: left; padding: 8px 0; color: #666666;'>Effective Date:</th>
                        <td style='text-align: right; padding: 8px 0; color: #333333;'>{effectiveDate:MMMM dd, yyyy}</td>
                    </tr>
                </table>
                            </div>
            <p style='font-size: 16px; color: #555555;'>Please feel free to reach out to us if you have any questions or need further assistance.</p>
            <p style='font-size: 16px; color: #555555;'>Thank you for your continued support.</p>
                            <div style='margin-top: 30px;'>
                                <a href='#' style='background-color: #008CBA; color: #ffffff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Contact Support</a>
                            </div>
                        </div>
                        <p style='color: #888888; font-size: 12px; margin-top: 20px;'>© 2024 Cursus. All rights reserved.</p>
                    </div>";
        }




    }
}
