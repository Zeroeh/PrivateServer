using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using GameObjects;
using System.Web;

namespace ConServer.credits
{
    class add : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context) //exploit where editing the gold amount in the url will give that amount of gold, need to change url requesting
        {
            string status;
            using (var db = new Database())
            {
                var query = HttpUtility.ParseQueryString(context.Request.Url.Query);

                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT id FROM accounts WHERE uuid=@uuid";
                cmd.Parameters.AddWithValue("@uuid", query["guid"]);
                object id = cmd.ExecuteScalar();

                if (id != null)
                {
                    int amount = int.Parse(query["jwt"]);
                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE stats SET credits = credits + @amount WHERE accId=@accId";
                    cmd.Parameters.AddWithValue("@accId", (int)id);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    int result = (int)cmd.ExecuteNonQuery();
                    if (result > 0)
                        status = "";
                    else
                        status = "You dun goofed.";
                }
                else
                    status = "Severe server error. Should not be getting this!";
            }

            var res = Encoding.UTF8.GetBytes(
@"<html>
    <head>
        <title>White Lotus - Purchase Complete</title>
    </head>
    <body style='background: #333333'>
        <h1 style='color: #FF00FF; text-align: center'>
        </h1>
        " + status + @"
    <center><p><font color='#FF00FF'>You can donate to my PayPal: gamesforgames4@gmail.com</font></p></center>
<center><p><font color='#00FFFF'>Check out the rest of the website <a href='http://25.92.155.93:8888/website/index'>here</a></font></p></center>
    </body>
</html>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}