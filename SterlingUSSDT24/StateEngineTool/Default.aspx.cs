using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    StateEngineDbContext db = new StateEngineDbContext();
    List<USSDState> sts;
    protected void Page_Load(object sender, EventArgs e)
    { 
        sts = db.USSDStates.Where(x => x.op_id == 0).ToList();
        //GridView1.DataSource = sts;
        //GridView1.DataBind();
        
         
    }

 
}