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
    class sendVerifyEmail : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            byte[] status= Encoding.UTF8.GetBytes("<Error>Will be implemented in the future.</Error>");
            context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}
