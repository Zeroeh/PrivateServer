using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using GameObjects;
using System.Web;

namespace ConServer.website
{
    class index : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
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
                    int amount = int.Parse(query["links"]);
                    cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE stats SET totalCredits = totalCredits + @amount WHERE accId=@accId";
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
@"<!DOCTYPE html>
    <html>
	<head>
	<link href='http://fonts.googleapis.com/css?family=Press+Start+2P' rel='stylesheet' type='text/css'>
	<title>The White Lotus</title>
	</head>
	<body>
	<style>
    *{
	background-color: black;
    }
    #Logo {
	position: relative;
	margin-top: 50px;
	margin-left: 39.8%;
    }
    ::-webkit-input-placeholder { /* WebKit browsers */
    color:    #AC1D00;
    }
    :-moz-placeholder { /* Mozilla Firefox 4 to 18 */
    color:    #AC1D00;
    opacity:  1;
    }
    ::-moz-placeholder { /* Mozilla Firefox 19+ */
    color:    #AC1D00;
    opacity:  1;
    }
    :-ms-input-placeholder { /* Internet Explorer 10+ */
    color:    #AC1D00;
    }
    ::-webkit-input-submit :hover{
    background-color: #FF8000;
    }
    :-moz-submit :hover{
    background-color: #FF8000;
    opacity:  1;
    }
    ::-moz-submit :hover{
    background-color: #FF8000;
    opacity:  1;
    }
    :-ms-input-submit :hover{
    background-color: #FF8000;
    }
    #Username {
	font-size: 8pt;
	font-family: 'Press Start 2P', cursive;
	height: 30px;
	color: #820000;
	border:4px dotted #D54A00;
	border-radius: 5px;
	background-color: #D56B00;
	text-align: center;
	margin-left: 43%;
    }
    #Password {
	font-size: 8pt;
	font-family: 'Press Start 2P', cursive;
	height: 30px;
	color: #820000;
	border:4px dotted #D54A00;
	border-radius: 5px;
	background-color: #D56B00;
	text-align: center;
	margin-left: 43%;
    }
    #pUsername {
	font-size: 10pt;
	font-family: 'Press Start 2P', cursive;
	color: #AC1D00;
	margin-top: 100px;
	text-align: center;
    }
    #pPassword {
	font-size: 10pt;
	font-family: 'Press Start 2P', cursive;
	color: #AC1D00;
	text-align: center;
    }
    #Login {
	font-size: 8pt;
	font-family: 'Press Start 2P', cursive;
	width: 80px;
	height: 40px;
	color: #820000;
	border:2px dotted #D54A00;
	border-radius: 5px;
	background-color: #D56B00;
	margin-top: 20px;
	margin-left: 47.5%;
    }
	</style>
	<div id='Logo'><img src='http://i.imgur.com/dEdk8No.png'></div>
	<div id='Panel'>
	<p id='pUsername'>Username</p>
	<input id='Username' Type='text' placeholder='Username'>
	<p id='pPassword'>Password</p>
	<input id='Password' Type='password' placeholder='Password'><br>
	<input id='Login' type='submit' value='Log in'></input>
	</div>
	</body>
    </html>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}