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
            var settings = new Dictionary<string, string>
            {

            };


            var connectionBuilder = new ConnectionBuilder(settings);

            using (var context = new RfcContext(connectionBuilder.Build()))
            {
                var response = context.PingAsync().GetAwaiter().GetResult();
            }
        }
    }
}