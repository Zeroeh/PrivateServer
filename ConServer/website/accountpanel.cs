using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace ConServer.website
{
    class accountpanel : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            var res = Encoding.UTF8.GetBytes(
@"<p><center>You have successfully logged into your White Lotus Account</center></p>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}