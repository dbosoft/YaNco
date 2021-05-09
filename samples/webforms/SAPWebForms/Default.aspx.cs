using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dbosoft.YaNco;

namespace SAPWebForms
{
    public partial class _Default : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // this application has a special method to set the SAP connection. 
            // it has to be passed as a single configuration string in user secrets:
            //
            //   <secrets ver="1.0">
            // <secret name="SAPConnectionString" value="ashost=<hostname>;sysnr=00;client=<client>;username="<username>";password="<password>"" />
            // </secrets>
            var connString = ConfigurationManager.AppSettings["SAPConnectionString"];
            var settings = connString
                .Split(';')
                .Select(HttpUtility.HtmlDecode).Select(x => x.Trim('"', '\'').Split('='))
                .ToDictionary(sa => sa[0], sa => sa[1].Trim('"', '\''));


            var connectionBuilder = new ConnectionBuilder(settings);

            using (var context = new RfcContext(connectionBuilder.Build()))
            {
                context.CallFunction("BAPI_COMPANYCODE_GETLIST",
                        Output: f => f
                            .MapTable("COMPANYCODE_LIST", s =>
                                from code in s.GetField<string>("COMP_CODE")
                                from name in s.GetField<string>("COMP_NAME")
                                select (code, name)))
                    .Match(
                        r =>
                        {
                            Session["Companies"] = r;
                        },
                        l => Session["ErrorMessage"] = l.Message).GetAwaiter().GetResult();

            }
        }
    }
}