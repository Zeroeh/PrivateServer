using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace ConServer.website
{
    class login : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            var res = Encoding.UTF8.GetBytes(
@"<div class='login'>
      <h1>Login to your White Lotus Account</h1>
      <form method='post' action='accountpanel'>
        <p><input type='text' name='login' value='' placeholder='Username or Email'></p>
        <p><input type='password' name='password' value='' placeholder='Password'></p>
        <p class='submit'><input type='submit' name='commit' value='Login'></p>
      </form>
    </div>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}