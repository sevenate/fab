using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fab.Metro.DataModel
{
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets WCF service address.
        /// </summary>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        public string Password { get; set; }
    }
}
