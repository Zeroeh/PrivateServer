using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace ConServer.account
{
    class forgotPassword : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            byte[] status = Encoding.UTF8.GetBytes("<Error>Contact Zeroeh if you forgot your password. zerodev@gmail.com</Error>");
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}
