using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace ConServer.credits
{
    class getoffers : IRequestHandler
    {
        public void HandleRequest(HttpListenerContext context)
        {
            var res = Encoding.UTF8.GetBytes(
"<Offers><Tok>zXuyW</Tok><Exp>238</Exp><Offer><Id>4</Id><Price>100</Price><RealmGold>10000</RealmGold><CheckoutJWT>10000</CheckoutJWT><Data>4</Data><Currency>USD</Currency></Offer><Offer><Id>3</Id><Price>50</Price><RealmGold>5000</RealmGold><CheckoutJWT>5000</CheckoutJWT><Data>3</Data><Currency>USD</Currency></Offer><Offer><Id>2</Id><Price>15</Price><RealmGold>1500</RealmGold><CheckoutJWT>1500</CheckoutJWT><Data>2</Data><Currency>USD</Currency></Offer><Offer><Id>0</Id><Price>10</Price><RealmGold>1000</RealmGold><CheckoutJWT>1000</CheckoutJWT><Data>1</Data><Currency>USD</Currency></Offer><Offer><Id>1</Id><Price>5</Price><RealmGold>500</RealmGold><CheckoutJWT>500</CheckoutJWT><Data>2</Data><Currency>USD</Currency></Offer></Offers>");
            context.Response.OutputStream.Write(res, 0, res.Length);
        }
    }
}