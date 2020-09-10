using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PKLib_Method.Methods;
using ProdCheckData.Controllers;

/// <summary>
/// 使用SelectPDF 產生 PDF
/// </summary>
/// <remarks>
/// GetFile_CheckView.ashx
/// </remarks>
public partial class myProdCheck_Html_CheckView : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //取得資料
        LookupData();
    }


    /// <summary>
    /// 取得資料
    /// </summary>
    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            return;
        }


        //填入資料
        string corp = query.Corp_UID.ToString();
        string modelno = query.ModelNo;
        string shipFrom = query.ShipFrom;
        string qcCate = query.QC_Category;

        this.lt_ErpID.Text = query.FirstID + " - " + query.SecondID;
        this.lt_Vendor.Text = query.VendorName;
        this.lt_Address.Text = query.VendorAddress;
        this.lt_ModelNo.Text = modelno;
        this.lt_ModelName.Text = query.ModelName;

        //取得檢驗項目
        this.lt_ItemContent.Text = Get_CheckItems(shipFrom, modelno, qcCate);
    }


    /// <summary>
    /// 取得檢驗項目
    /// </summary>
    /// <param name="shipFrom"></param>
    /// <param name="modelNo"></param>
    /// <returns></returns>
    private string Get_CheckItems(string shipFrom, string modelNo, string qcCate)
    {
        //----- 宣告:資料參數 -----
        ProdCheckRepository _data = new ProdCheckRepository();
        StringBuilder html = new StringBuilder();

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetCheckItems(shipFrom, modelNo, qcCate);

        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            return "";
        }

        //項次從 A 開始
        int row = 65;
        foreach (var item in query)
        {
            html.AppendLine("<tr>");
            //項次, 內容, 編號1-20
            html.AppendLine("<td>{0}</td><td style=\"text-align:left\">{1}</td>{2}".FormatThis(
                fn_stringFormat.Chr(row)
                , item.Spec
                , Get_EmptyColumn(20, false)
                ));
            html.AppendLine("</tr>");


            row++;
        }

        //return
        return html.ToString();
    }


    public string Get_EmptyColumn(int colNum, bool showNum)
    {
        string html = "";

        for (int col = 1; col <= (colNum <= 0 ? 1 : colNum); col++)
        {
            string colcontent = col < 10 ? "&nbsp;" + col.ToString() : col.ToString();

            html += "<td {1}>{0}</td>".FormatThis(
                showNum ? colcontent : "&nbsp;"
                , showNum ? "class=\"setBg\"" : "");
        }

        return html;
    }


    /// <summary>
    /// 取得參數 - DataID
    /// </summary>
    public string Req_DataID
    {
        get
        {
            String data = Request.QueryString["DataID"];
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Req_DataID = value;
        }
    }
    private string _Req_DataID;
}