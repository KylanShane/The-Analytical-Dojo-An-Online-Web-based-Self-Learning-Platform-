using System;
using System.Web;
using System.Web.UI;
public class DojoPage : Page {
 public int UserId { get { return Convert.ToInt32(Session["UserId"]); } }
 public string Csrf { get { return Convert.ToString(Session["DojoCsrf"]); } }
 protected override void OnInit(EventArgs e) {
  if(Session["UserId"]==null){Response.Redirect("Login.aspx",true);return;}
  Response.Cache.SetCacheability(HttpCacheability.NoCache); Response.Cache.SetNoStore();
  if(Session["DojoCsrf"]==null) Session["DojoCsrf"]=Guid.NewGuid().ToString("N");
  ViewStateUserKey=Session.SessionID;
  if(Request.HttpMethod=="POST" && Request.Form["dojoCsrf"]!=Csrf) throw new HttpException(400,"Please reload the page and try again.");
  base.OnInit(e);
 }
 public static string H(object v){return HttpUtility.HtmlEncode(Convert.ToString(v));}
 public int QueryId(string key){int n; return int.TryParse(Request.QueryString[key],out n)&&n>0?n:0;}
}
