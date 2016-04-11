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
using System.Globalization;

namespace ConServer.account
{
    class register : IRequestHandler // Usernames are still used when registering!!!
    {
        //public bool IsValidEmail(string strIn)
        //{
        //    var invalid = false;
        //    if (String.IsNullOrEmpty(strIn))
        //        return false;

        //    MatchEvaluator DomainMapper = match =>
        //    {
        //         IdnMapping class with default property values.
        //        IdnMapping idn = new IdnMapping();

        //        string domainName = match.Groups[2].Value;
        //        try
        //        {
        //            domainName = idn.GetAscii(domainName);
        //        }
        //        catch (ArgumentException)
        //        {
        //            invalid = false; //should be false
        //        }
        //        return match.Groups[1].Value + domainName;
        //    };

        //     Use IdnMapping class to convert Unicode domain names. 
        //    strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
        //    if (invalid)
        //        return false;

        //     Return true if strIn is in valid e-mail format. 
        //    return Regex.IsMatch(strIn,
        //              @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        //              @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
        //              RegexOptions.IgnoreCase);
        //}

        public void HandleRequest(HttpListenerContext context)
        {
            NameValueCollection query;
            using (StreamReader rdr = new StreamReader(context.Request.InputStream))
                query = HttpUtility.ParseQueryString(rdr.ReadToEnd());

            using (var db = new Database())
            {
                byte[] status;
                if (0 != 0 /*!IsValidEmail(query["newGUID"])*/)
                    status = Encoding.UTF8.GetBytes("<Error>Invalid Email</Error>");
                else
                {
                    if (db.HasUuid(query["guid"]) &&
                        db.Verify(query["guid"], "") != null)
                    {
                        if (db.HasUuid(query["newGUID"]))
                            status = Encoding.UTF8.GetBytes("<Error>Email is already in use!</Error>");
                        else
                        {
                            var cmd = db.CreateQuery();
                            cmd.CommandText = "UPDATE accounts SET uuid=@newUuid, name=@newUuid, password=SHA1(@password), guest=FALSE WHERE uuid=@uuid, name=@name;";
                            cmd.Parameters.AddWithValue("@uuid", query["guid"]);
                            cmd.Parameters.AddWithValue("@newUuid", query["newGUID"]);
                            cmd.Parameters.AddWithValue("@password", query["newPassword"]);
                            if (cmd.ExecuteNonQuery() > 0)
                                status = Encoding.UTF8.GetBytes("<Success />");
                            else
                                status = Encoding.UTF8.GetBytes("<Error>Register.cs error</Error>");
                        }
                    }
                    else
                    {
                        if (db.Register(query["newGUID"], query["newPassword"], false) != null)
                            status = Encoding.UTF8.GetBytes("<Success />");
                        else
                            status = Encoding.UTF8.GetBytes("<Error>Register.cs error</Error>");
                    }
                }
                context.Response.OutputStream.Write(status, 0, status.Length);
            }
        }
    }
}
