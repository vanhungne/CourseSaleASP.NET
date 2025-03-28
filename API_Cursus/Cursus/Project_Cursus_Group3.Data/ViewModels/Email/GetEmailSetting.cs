using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Cursus_Group3.Data.ViewModels.Email
{
    public class GetEmailSetting
    {
        public string SecretKey { get; set; } = default!;

        public string From { get; set; } = default!;

        public string SmtpServer { get; set; } = default!;

        public int Port { get; set; }

        public bool EnablSSL { get; set; }

    }
}
