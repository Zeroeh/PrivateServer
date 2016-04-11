using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using GameObjects;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;

namespace ConServer.account
{
    class purchaseCharSlot : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context) //fame = fame II gold = credits
        {
            NameValueCollection query;
            using (StreamReader rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            using (var db = new Database())
            {
                var acc = db.Verify(query["guid"], query["password"]);
                byte[] status;
                if (acc == null)
                {
                    status = Encoding.UTF8.GetBytes("<Error>Bad login</Error>");
                }
                else
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "SELECT credits FROM stats WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                    if ((int)cmd.ExecuteScalar() < 0)
                        status = Encoding.UTF8.GetBytes("<Error>Not enough gold</Error>");
                    else
                    {
                        cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE stats SET credits = credits - 1000 WHERE accId=@accId"; //gold=credits fame=fame NOTE: the "- 1000" takes away 1000 of whatever currency, but to be able to get it to show up as 1000 in the client, edit Database.cs
                        cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                        if ((int)cmd.ExecuteNonQuery() > 0)
                        {
                            cmd = db.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET maxCharSlot = maxCharSlot + 1 WHERE id=@accId";
                            cmd.Parameters.AddWithValue("@accId", acc.AccountId);
                            if ((int)cmd.ExecuteNonQuery() > 0)
                                status = Encoding.UTF8.GetBytes("<Success/>");
                            else
                                status = Encoding.UTF8.GetBytes("<Error>CharSlot.cs error</Error>");
                        }
                        else
                            status = Encoding.UTF8.GetBytes("<Error>CharSlot.cs Error</Error>");
                    }
                }
                context.Response.OutputStream.Write(status, 0, status.Length);
            }
        }
    }
}
