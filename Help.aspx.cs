using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Help : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["func"] == null)
            {
                Param_Func = "";
                this.pl_uc.Visible = true;                
            }
            else
            {
                Param_Func = Request.QueryString["func"].ToString();
            }

            switch(Param_Func){
                case "ProdPic":
                case "Certification":
                    this.pl_Cert.Visible = true;
                    break;

                case "ProdPic_Group":
                    this.pl_PicGroup.Visible = true;
                    break;

                case "xxx":
                    break;

                default:
                    this.pl_uc.Visible = true;
                    break;
            }
      
        }
    }

    /// <summary>
    /// 來源功能
    /// </summary>
    private string _Param_Func;
    public string Param_Func
    {
        get;
        set;
    }
}