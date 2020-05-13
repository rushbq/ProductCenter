using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using EcLifeData.Controllers;
using EcLifeData.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PKLib_Method.Methods;


public partial class EcLife_ProductEdit : SecurityIn
{
    public string ErrMsg;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //[權限判斷]
                if (fn_CheckAuth.CheckAuth_User("110", out ErrMsg) == false)
                {
                    Response.Redirect(string.Format("../Unauthorized.aspx?ErrMsg={0}", HttpUtility.UrlEncode(ErrMsg)), true);
                    return;
                }

                //隱藏所有的Placeholder
                this.ph_ErrMessage.Visible = false;
                this.ph_Msg1.Visible = false;
                this.ph_eclife_Msg1.Visible = false;
                this.ph_eclife_Msg2.Visible = false;
                this.ph_Data.Visible = false;

                //判斷品號是否為空
                if (string.IsNullOrEmpty(Req_DataID))
                {
                    this.ph_ErrMessage.Visible = true;
                    return;
                }


                //判斷是否有Token
                if (string.IsNullOrEmpty(fn_Param.Token))
                {
                    //重新設定API值
                    SetWebInfo();
                }

                //分類處理
                #region >> 分類處理 <<

                //判斷分類Cookie是否為空
                if (string.IsNullOrEmpty(fn_Param.Category))
                {
                    //重新設定分類Cookie
                    ResetAllClass();
                }

                //頁面載入 - 重設分類選單
                DropDownList Menu1 = this.ddl_HouseNo;
                DropDownList Menu2 = this.ddl_LargeNo;
                DropDownList Menu3 = this.ddl_MediumNo;
                ResetMenuObj(Menu1, "lv1");
                ResetMenuObj(Menu2, "lv2");
                ResetMenuObj(Menu3, "lv3");

                //設定大分類
                SetMenu(Menu1, "lv1");

                #endregion

                //Get Data
                LookupData();
                LookupData_Tags();

            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    #region -- 資料顯示 --

    private void LookupData()
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();


        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);


        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();


        //----- 資料整理:繫結 ----- 
        if (query == null)
        {
            #region >> 自動Insert <<

            //判斷是否有新增過資料, 沒有則Insert
            var data = new Product
            {
                ModelNo = Req_DataID,
                PicUrl = GetData_MainPic(GetPicUrl(Req_DataID), Req_DataID), //自動取得圖片(依順序,有圖的第一張)
                SyncStatus = "A",
                Create_Who = fn_Param.CurrentUser.ToString()
            };
            //----- 方法:新增資料 -----
            if (false == _data.Create(data))
            {
                this.ph_ErrMessage.Visible = true;
                return;
            }
            else
            {
                //>>> 建立Log <<<
                CreateLog(Req_DataID, 1, "新增商品資料", "");

                //重新導向
                Response.Redirect(Page_CurrentUrl);
                return;
            }

            #endregion
        }

        //取得API商品狀態
        string[] ProdStatus = GetProdStatus(Req_DataID);

        //填入欄位資料
        string stName = ProdStatus[1];
        this.lt_ProdStatus.Text = stName;
        this.hf_ProdStatus.Value = ProdStatus[0];

        this.lb_Cls1.Text = query.HouseName;
        this.lb_Cls2.Text = query.LargeName;
        this.lb_Cls3.Text = query.MediumName;

        this.tb_ProductName.Text = HttpUtility.HtmlDecode(query.ProductName);
        this.tb_SPName.Text = query.SPName;
        this.tb_Barcode.Text = query.BarCode;
        this.tb_ProductMemo.Text = query.ProductMemo;
        this.tb_MaxBuy.Text = query.MaxBuy.ToString();
        this.tb_MaxBuy_Tot.Text = query.MaxBuyTot.ToString();

        //Price
        //this.tb_Price_Cost.Text = query.Price_Cost.ToString();
        this.lt_Price_Sale.Text = fn_stringFormat.C_format(query.Price_Sale.ToString());
        this.lt_Price_Spical.Text = fn_stringFormat.C_format(query.Price_Spical.ToString());

        //Desc
        this.lt_Desc_Classics.Text = query.Desc_Classics.Replace("\r\n", "<br>");
        this.lt_Desc_Feature.Text = query.Desc_Feature;
        this.lt_Desc_Standards.Text = query.Desc_Standards;
        this.lt_Desc_Introduce.Text = query.Desc_Introduce;
        this.tb_Desc_Services.Text = query.Desc_Services;

        //維護資訊
        //this.lt_Creater.Text = item.Create_Name;
        //this.lt_CreateTime.Text = item.Create_Time;
        //this.lt_Updater.Text = item.Update_Name;
        //this.lt_UpdateTime.Text = item.Update_Time;

        //商品圖 - 選擇列表
        string picGroup = GetPicUrl(Req_DataID);
       
        this.lt_Pics.Text = GetData_AllPic(picGroup, Req_DataID);

        //Log顯示
        this.lt_logBase.Text = GetLogHtml("1");
        this.lt_logPic.Text = GetLogHtml("2");
        this.lt_logStock.Text = GetLogHtml("3");
        this.lt_logApi.Text = GetLogHtml("4");

        //顯示資料區塊
        this.ph_Data.Visible = true;

        //非新的資料, 不檢查必填
        if (!query.SyncStatus.Equals("A"))
        {
            this.rfv_ddl_HouseNo.Enabled = false;
            this.rfv_ddl_LargeNo.Enabled = false;
            this.rfv_ddl_MediumNo.Enabled = false;
        }

        /*
          [判斷同步狀態]
             A:新資料,未完善 ([Create])
             B:商品資料未提報 ([Update] prodStatus=error)
             C:資料異動後未同步 ([Update] prodStatus=ok)
             D:沒事
         */
        if (query.IsSync.Equals("N"))
        {
            switch (query.SyncStatus)
            {
                case "A":
                    //新資料,未完善
                    this.ph_Msg1.Visible = true;
                    break;

                case "B":
                    //商品資料未提報
                    this.ph_eclife_Msg1.Visible = !stName.Equals("商品尚在待審中");
                    break;

                case "C":
                    //資料異動後未同步
                    this.ph_eclife_Msg2.Visible = true;
                    break;

                default:
                    break;
            }
        }


        //Release
        query = null;
    }

    /// <summary>
    /// 取得Log資訊
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetLogHtml(string type)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _dataList = new EcLifeRepository();
        StringBuilder html = new StringBuilder();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetLog(Req_DataID, type);

        //----- 資料整理 ----- 
        foreach (var item in query)
        {
            html.Append("<tr>");
            html.Append("<td>{0}</td>".FormatThis(item.LogName));
            html.Append("<td>{0}</td>".FormatThis(item.LogValue));
            html.Append("<td>{0}</td>".FormatThis(item.LogWho));
            html.Append("<td>{0}</td>".FormatThis(item.LogTime));
            html.Append("</tr>");
        }

        //Release
        query = null;

        //return
        return html.ToString();
    }
    #endregion


    #region -- 資料顯示:產品關鍵字 --

    /// <summary>
    /// 顯示產品關鍵字
    /// </summary>
    private void LookupData_Tags()
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _dataList = new EcLifeRepository();

        //----- 原始資料:取得所有資料 -----
        var query = _dataList.GetTags(Req_DataID);

        //----- 資料整理:繫結 ----- 
        this.lv_Tags.DataSource = query;
        this.lv_Tags.DataBind();

        //Release
        query = null;
    }

    protected void lv_Tags_ItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (e.Item.ItemType == ListViewItemType.DataItem)
        {
            //取得Key值
            int Get_DataID = Convert.ToInt16(((HiddenField)e.Item.FindControl("hf_DataID")).Value);

            //----- 宣告:資料參數 -----
            EcLifeRepository _data = new EcLifeRepository();


            //----- 設定:資料欄位 -----
            var data = new Tags
            {
                TagID = Get_DataID,
                ModelNo = Req_DataID
            };

            //----- 方法:刪除資料 -----
            if (false == _data.Delete_Tag(data))
            {
                this.ph_ErrMessage.Visible = true;
                return;
            }
            else
            {
                //導向本頁
                Response.Redirect(Page_CurrentUrl + "#setTags");
            }

        }
    }

    #endregion


    #region -- 資料編輯 --

    /// <summary>
    /// 商品資料更新
    /// </summary>
    protected void lbtn_Save_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();

        //----- 設定:資料欄位 -----
        var data = new Product
        {
            ModelNo = Req_DataID,
            HouseNo = (this.ddl_HouseNo.SelectedIndex > 0) ? this.ddl_HouseNo.SelectedValue : "",
            HouseName = (this.ddl_HouseNo.SelectedIndex > 0) ? this.ddl_HouseNo.SelectedItem.Text : "",
            LargeNo = (this.ddl_LargeNo.SelectedIndex > 0) ? this.ddl_LargeNo.SelectedValue : "",
            LargeName = (this.ddl_LargeNo.SelectedIndex > 0) ? this.ddl_LargeNo.SelectedItem.Text : "",
            MediumNo = (this.ddl_MediumNo.SelectedIndex > 0) ? this.ddl_MediumNo.SelectedValue : "",
            MediumName = (this.ddl_MediumNo.SelectedIndex > 0) ? this.ddl_MediumNo.SelectedItem.Text : "",
            ProductName = HttpUtility.HtmlEncode(this.tb_ProductName.Text),
            SPName = this.tb_SPName.Text,
            MType = Req_DataID,

            ProductMemo = this.tb_ProductMemo.Text,
            //Desc_Classics = this.tb_Desc_Classics.Text,
            //Desc_Feature = this.tb_Desc_Feature.Text,
            //Desc_Standards = this.tb_Desc_Standards.Text,
            //Desc_Introduce = this.tb_Desc_Introduce.Text,
            Desc_Services = this.tb_Desc_Services.Text,

            MaxBuy = string.IsNullOrEmpty(this.tb_MaxBuy.Text) ? 1 : Convert.ToInt32(this.tb_MaxBuy.Text),
            MaxBuyTot = string.IsNullOrEmpty(this.tb_MaxBuy_Tot.Text) ? 1 : Convert.ToInt32(this.tb_MaxBuy_Tot.Text),
            PicUrl = GetData_MainPic(GetPicUrl(Req_DataID), Req_DataID), //自動取得圖片(依順序,有圖的第一張)

            //B:商品資料未提報 ([Update] prodStatus=error); C:資料異動後未同步 ([Update] prodStatus=ok)
            SyncStatus = this.hf_ProdStatus.Value.Equals("ok") ? "C" : "B",
            Update_Who = fn_Param.CurrentUser.ToString()
        };


        //----- 方法:更新資料 -----
        if (false == _data.Update(data))
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //>>> 建立Log <<<
            CreateLog(Req_DataID, 1, "更新商品資料", "");

            //導向本頁
            Response.Redirect(Page_CurrentUrl);
        }
    }


    /// <summary>
    /// 加入關鍵字關聯 - PKWeb
    /// </summary>
    protected void lbtn_AddTag_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();

        //----- 設定:資料欄位 -----
        var data = new Tags
        {
            ModelNo = Req_DataID,
            TagID = Convert.ToInt16(this.tb_TagID.Text),
            TagName = this.tb_TagID.Text.Equals("0") ? this.Rel_Tag.Text : this.tb_TagName.Text
        };

        //----- 方法:更新資料 -----
        if (false == _data.Create_Tags(data))
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //>>> 建立Log <<<
            CreateLog(Req_DataID, 1, "更新商品資料", "加入關鍵字");

            //導向本頁
            Response.Redirect(Page_CurrentUrl + "#setTags");
        }
    }


    /// <summary>
    /// 建立Log
    /// </summary>
    /// <param name="ModelNo"></param>
    /// <param name="type">1:商品資料 / 2:商品圖異動 / 3:庫存異動 / 4:API</param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    void CreateLog(string ModelNo, short type, string name, string value)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();

        //----- 設定:資料欄位 -----
        var data = new Log
        {
            ModelNo = ModelNo,
            LogType = type,
            LogName = name,
            LogValue = value,
            LogWho = fn_Param.CurrentUser.ToString()
        };

        //----- 方法:更新資料 -----
        if (false == _data.Create_Log(data))
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }
    }

    #endregion


    #region -- API資料處理 --

    /// <summary>
    /// 商品提報, prodNew (API3)
    /// </summary>
    protected void lbtn_ProdNew_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }

        //API-網址
        string url = "https://api-proskit.eclife.com.tw/v1/prodNew/";
        //官網影片字串
        string pkWebVideo = _data.GetPKWeb_Video(Req_DataID, "zh-tw");

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("OwnProductNO", HttpUtility.UrlEncode(query.ModelNo.Left(20).ToUpper()));  //(*) OwnProductNO (string(20)), 廠商貨號(寶工型號)
        reqParams.Add("H", query.HouseNo);  //(*) H (string), 寶工品牌網站的商品分類1 (由API2取得 HouseNo)
        reqParams.Add("L", query.LargeNo);  //(*) L (string), 寶工品牌網站的商品分類2 (由API2取得 LargeNO)
        reqParams.Add("M", query.MediumNo);  //(*) M (string), 寶工品牌網站的商品分類3 (由API2取得 MediumNO)
        reqParams.Add("S", "");  //(*) S (string), 寶工品牌網站的商品分類4 (由API2取得 MediumSubNO)
        reqParams.Add("ProductName", HttpUtility.UrlEncode(HttpUtility.HtmlDecode(query.ProductName).Left(50)));  //(*) ProductName (string(50)), 產品名稱
        reqParams.Add("mtype", query.MType.Left(30).ToUpper());  //(*) mtype (string(30)), 寶工型號
        reqParams.Add("barCode", query.BarCode.Left(20));  //barCode (string(20)), 國際條碼
        reqParams.Add("max_buy", query.MaxBuy.ToString());  //(*) max_buy (integer)(default:1), 一次可購量(需大於0,default:1)
        reqParams.Add("max_buy_tot", query.MaxBuyTot.ToString());  //(*) max_buy_tot (integer)(default:1), 當日總訂量(需大於0,default:1)
        reqParams.Add("productmemo", HttpUtility.UrlEncode(query.ProductMemo.Left(12)));  //productmemo (string(12)), 推薦說明(最多12個中文字)
        reqParams.Add("keyword", HttpUtility.UrlEncode(Get_KeywordString().Left(100)));  //keyword (string(100)), 搜尋關鍵字 (空格做分隔)
        reqParams.Add("cost", query.Price_Cost.ToString());  //cost (integer), 成本 (未稅)
        reqParams.Add("saleprice", query.Price_Sale.ToString());  //saleprice (integer), 市價
        reqParams.Add("spicalprice", query.Price_Spical.ToString());  //spicalprice (integer), 網路價
        reqParams.Add("classics", HttpUtility.UrlEncode(query.Desc_Classics));  //(*) classics (string(100)), 商品特色(No HTML)
        reqParams.Add("feature", HttpUtility.UrlEncode(pkWebVideo + query.Desc_Feature));  //(*) feature (string), 商品介紹
        reqParams.Add("standards", HttpUtility.UrlEncode(query.Desc_Standards));  //(*) standards (string), 商品規格
        reqParams.Add("introduce", HttpUtility.UrlEncode(query.Desc_Introduce));  //(*) introduce (string), 包裝附件(No HTML)
        reqParams.Add("services", HttpUtility.UrlEncode(query.Desc_Services));  //(*) services (string), 保固說明(No HTML)
        reqParams.Add("image", string.IsNullOrEmpty(query.PicUrl) ? "" : query.PicUrl);  //(*) image (string), 主商品圖URL (尺寸: 500*500) (副檔名限制.jpg .gif .png)

        query = null;


        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, reqHeaders);

        //解析Json
        JObject json;

        try
        {
            json = JObject.Parse(reqResult);
        }
        catch (Exception ex)
        {
            this.lt_ShowMsg.Text = "同步失敗,回傳格式不正確<br/>" + ex.Message.ToString();
            this.ph_ErrMessage.Visible = true;
            return;
        }

        //取得Json元素
        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();

        //結果處理
        if (!rspStatus.Equals("ok"))
        {
            this.lt_ShowMsg.Text = rspMessage;
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //----- 方法:更新狀態 -----
            var data = new Product
            {
                ModelNo = Req_DataID,
                SyncStatus = "D",
                IsSync = "Y"
            };

            if (false == _data.Update_Status(data))
            {
                this.ph_ErrMessage.Visible = true;
                return;
            }

            //>>> 建立Log <<<
            CreateLog(Req_DataID, 4, "API資料處理", "商品提報");

            //Redirect
            Response.Redirect(Page_CurrentUrl);
        }
    }


    /// <summary>
    /// 商品更新, prodUpdate (API5)
    /// </summary>
    protected void lbtn_ProdUpdate_Click(object sender, EventArgs e)
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();
        Dictionary<int, string> search = new Dictionary<int, string>();

        //----- 原始資料:條件篩選 -----
        search.Add((int)mySearch.DataID, Req_DataID);

        //----- 原始資料:取得所有資料 -----
        var query = _data.GetDataList(search).Take(1).FirstOrDefault();

        if (query == null)
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }


        //API - 網址
        string url = "https://api-proskit.eclife.com.tw/v1/prodUpdate/";
        //官網影片字串
        string pkWebVideo = _data.GetPKWeb_Video(Req_DataID, "zh-tw");

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("OwnProductNO", HttpUtility.UrlEncode(query.ModelNo.Left(20).ToUpper()));  //(*) OwnProductNO (string(20)), 廠商貨號(寶工型號)
        reqParams.Add("H", query.HouseNo);  //(*) H (string), 寶工品牌網站的商品分類1 (由API2取得 HouseNo)
        reqParams.Add("L", query.LargeNo);  //(*) L (string), 寶工品牌網站的商品分類2 (由API2取得 LargeNO)
        reqParams.Add("M", query.MediumNo);  //(*) M (string), 寶工品牌網站的商品分類3 (由API2取得 MediumNO)
        reqParams.Add("S", "");  //(*) S (string), 寶工品牌網站的商品分類4 (由API2取得 MediumSubNO)
        reqParams.Add("ProductName", HttpUtility.UrlEncode(HttpUtility.HtmlDecode(query.ProductName).Left(50)));  //(*) ProductName (string(50)), 產品名稱
        reqParams.Add("barCode", query.BarCode.Left(20));  //barCode (string(20)), 國際條碼
        reqParams.Add("max_buy", query.MaxBuy.ToString());  //(*) max_buy (integer)(default:1), 一次可購量(需大於0,default:1)
        reqParams.Add("max_buy_tot", query.MaxBuyTot.ToString());  //(*) max_buy_tot (integer)(default:1), 當日總訂量(需大於0,default:1)
        reqParams.Add("productmemo", HttpUtility.UrlEncode(query.ProductMemo.Left(12)));  //productmemo (string(12)), 推薦說明(最多12個中文字)
        reqParams.Add("keyword", HttpUtility.UrlEncode(Get_KeywordString().Left(100)));  //keyword (string(100)), 搜尋關鍵字 (空格做分隔)
        reqParams.Add("classics", HttpUtility.UrlEncode(query.Desc_Classics));  //(*) classics (string(100)), 商品特色(No HTML)
        reqParams.Add("feature", HttpUtility.UrlEncode(pkWebVideo + query.Desc_Feature));  //(*) feature (string), 商品介紹
        reqParams.Add("standards", HttpUtility.UrlEncode(query.Desc_Standards));  //(*) standards (string), 商品規格
        reqParams.Add("introduce", HttpUtility.UrlEncode(query.Desc_Introduce));  //(*) introduce (string), 包裝附件(No HTML)
        reqParams.Add("services", HttpUtility.UrlEncode(query.Desc_Services));  //(*) services (string), 保固說明(No HTML)

        query = null;


        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, reqHeaders);

        //解析Json
        JObject json;

        try
        {
            json = JObject.Parse(reqResult);
        }
        catch (Exception ex)
        {
            this.lt_ShowMsg.Text = "同步失敗,回傳格式不正確<br/>" + ex.Message.ToString();
            this.ph_ErrMessage.Visible = true;
            return;
        }

        //取得Json元素
        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();

        //結果處理
        if (!rspStatus.Equals("ok"))
        {
            this.lt_ShowMsg.Text = rspMessage;
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //----- 方法:更新狀態 -----
            var data = new Product
            {
                ModelNo = Req_DataID,
                SyncStatus = "D",
                IsSync = "Y"
            };

            if (false == _data.Update_Status(data))
            {
                this.ph_ErrMessage.Visible = true;
                return;
            }


            //>>> 建立Log <<<
            CreateLog(Req_DataID, 4, "API資料處理", "商品更新");

            //Redirect
            Response.Redirect(Page_CurrentUrl);
        }
    }


    /// <summary>
    /// 商品圖異動, prodImage (API6)
    /// </summary>
    protected void lbtn_ProdImg_Click(object sender, EventArgs e)
    {
        //取得資料
        string inputData = this.tb_PicUrl.Text;
        if (string.IsNullOrEmpty(inputData))
        {
            this.lt_ShowMsg.Text = "資料未正確填寫，請重新確認。";
            this.ph_ErrMessage.Visible = true;
            return;
        }

        //API - 網址
        string url = "https://api-proskit.eclife.com.tw/v1/prodImage/";

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("OwnProductNO", Req_DataID);  //(*) OwnProductNO (string(20)), 廠商貨號(寶工型號)
        reqParams.Add("image", inputData);  //主商品圖URL 限上傳 .jpg .gif .png

        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, reqHeaders);

        //解析Json
        JObject json;

        try
        {
            json = JObject.Parse(reqResult);
        }
        catch (Exception)
        {
            this.lt_ShowMsg.Text = "同步失敗,請確認網址是否正確";
            this.ph_ErrMessage.Visible = true;
            return;
        }

        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();

        //結果處理
        if (!rspStatus.Equals("ok"))
        {
            this.lt_ShowMsg.Text = rspMessage;
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //>>> 建立Log <<<
            CreateLog(Req_DataID, 2, "API資料處理", "商品圖異動:" + inputData);

            //Redirect
            Response.Redirect(Page_CurrentUrl);
        }
    }


    /// <summary>
    /// 庫存異動, prodStock (API7)
    /// </summary>
    protected void lbtn_ProdStock_Click(object sender, EventArgs e)
    {
        //取得資料
        string inputData = this.tb_Stock.Text;
        if (string.IsNullOrEmpty(inputData))
        {
            this.lt_ShowMsg.Text = "資料未正確填寫，請重新確認。";
            this.ph_ErrMessage.Visible = true;
            return;
        }

        //API - 網址
        string url = "https://api-proskit.eclife.com.tw/v1/prodStock/";

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("OwnProductNO", Req_DataID);  //(*) OwnProductNO (string(20)), 廠商貨號(寶工型號)
        reqParams.Add("stock", inputData);  //庫存

        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, reqHeaders);

        //解析Json

        //解析Json
        JObject json;

        try
        {
            json = JObject.Parse(reqResult);
        }
        catch (Exception)
        {
            this.lt_ShowMsg.Text = "同步失敗,請確認資料是否填寫正確";
            this.ph_ErrMessage.Visible = true;
            return;
        }

        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();

        //結果處理
        if (!rspStatus.Equals("ok"))
        {
            this.lt_ShowMsg.Text = rspMessage;
            this.ph_ErrMessage.Visible = true;
            return;
        }
        else
        {
            //>>> 建立Log <<<
            CreateLog(Req_DataID, 2, "API資料處理", "庫存異動:" + inputData);

            //Redirect
            Response.Redirect(Page_CurrentUrl);
        }
    }


    /// <summary>
    /// 取得關鍵字-字串(from PKWeb)
    /// </summary>
    /// <returns></returns>
    private string Get_KeywordString()
    {
        // 宣告
        EcLifeRepository _dataList = new EcLifeRepository();
        string k = "";

        //取得資料
        var query = _dataList.GetTags(Req_DataID);
        foreach (var item in query)
        {
            k += item.TagName + " ";
        }

        query = null;

        return k;

    }
    #endregion


    #region -- 分類設定 --

    /// <summary>
    /// 重設分類選單
    /// </summary>
    private void ResetMenuObj(DropDownList menu, string type)
    {
        //清空Item
        menu.Items.Clear();

        //設定預設值
        switch (type)
        {
            case "lv1":
                menu.Items.Insert(0, new ListItem("請選擇大分類", ""));

                break;


            case "lv2":
                menu.Items.Insert(0, new ListItem("請先選擇大分類", ""));

                break;


            case "lv3":
                menu.Items.Insert(0, new ListItem("請先選擇中分類", ""));

                break;
        }
    }

    /// <summary>
    /// 設定選單
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="type"></param>
    private void SetMenu(DropDownList menu, string type)
    {
        ListItem[] myMenu = null;

        switch (type)
        {
            case "lv1":
                myMenu = GetBigClass(Data_Category);

                break;


            case "lv2":
                myMenu = GetLargeClass(Data_Category, this.ddl_HouseNo.SelectedValue);

                break;


            case "lv3":
                myMenu = GetMedClass(Data_Category, this.ddl_HouseNo.SelectedValue, this.ddl_LargeNo.SelectedValue);

                break;
        }

        if (myMenu != null)
        {
            menu.Items.AddRange(myMenu);
        }
    }


    /// <summary>
    /// Method - 取得大分類
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    private ListItem[] GetBigClass(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        //解析Json
        JObject jData = JObject.Parse(json);

        //將指定內容轉化為JArray
        JArray aryObj = (JArray)jData["data"];

        //Linq查詢
        var queryLv1 = aryObj
            .Select(i => new
            {
                HouseNo = i["HouseNo"].ToString(),
                HouseName = i["HouseName"].ToString()
            });

        //init
        int totalCnt = queryLv1.Count();
        int row = 0;

        //Dropdonwlist Items
        ListItem[] setMenu = new ListItem[totalCnt];

        foreach (var item in queryLv1)
        {
            setMenu[row] = new ListItem(item.HouseName, item.HouseNo);

            row++;
        }

        //output
        return setMenu;
    }


    /// <summary>
    /// Method - 取得中分類
    /// </summary>
    /// <param name="json"></param>
    /// <param name="parentID"></param>
    private ListItem[] GetLargeClass(string json, string parentID)
    {
        if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(parentID))
        {
            return null;
        }

        //解析Json
        JObject jData = JObject.Parse(json);

        //將指定內容轉化為JArray
        JArray aryObj = (JArray)jData["data"];

        //Linq查詢
        var queryLv1 = aryObj
            .Where(i => i["HouseNo"].ToString().Equals(parentID))
            .Select(i => new
            {
                subCategory = (JArray)i["subCategory"]
            }).FirstOrDefault();

        var queryLv2 = queryLv1.subCategory
            .Select(o => new
            {
                LargeNo = o["LargeNO"].ToString(),
                LargeName = o["LargeName"].ToString()
            });

        //init
        int totalCnt = queryLv2.Count();
        int row = 0;

        //Dropdonwlist Items
        ListItem[] setMenu = new ListItem[totalCnt];

        foreach (var item in queryLv2)
        {
            setMenu[row] = new ListItem(item.LargeName, item.LargeNo);

            row++;
        }

        //output
        return setMenu;
    }


    /// <summary>
    /// Method - 取得小分類
    /// </summary>
    /// <param name="json"></param>
    /// <param name="topID"></param>
    /// <param name="parentID"></param>
    private ListItem[] GetMedClass(string json, string topID, string parentID)
    {
        if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(topID) || string.IsNullOrEmpty(parentID))
        {
            return null;
        }

        //解析Json
        JObject jData = JObject.Parse(json);

        //將指定內容轉化為JArray
        JArray aryObj = (JArray)jData["data"];

        //Linq查詢
        var queryLv1 = aryObj
            .Where(i => i["HouseNo"].ToString().Equals(topID))
            .Select(i => new
            {
                subCategory = (JArray)i["subCategory"]
            }).FirstOrDefault();

        var queryLv2 = queryLv1.subCategory
            .Where(o => o["LargeNO"].ToString().Equals(parentID))
            .Select(o => new
            {
                subCategory = (JArray)o["subCategory"]
            }).FirstOrDefault();

        var queryLv3 = queryLv2.subCategory
            .Select(x => new
            {
                MediumNo = x["MediumNO"].ToString(),
                MediumName = x["MediumName"].ToString()
            });

        //init
        int totalCnt = queryLv3.Count();
        int row = 0;

        //Dropdonwlist Items
        ListItem[] setMenu = new ListItem[totalCnt];

        foreach (var item in queryLv3)
        {
            setMenu[row] = new ListItem(item.MediumName, item.MediumNo);

            row++;
        }

        //output
        return setMenu;
    }


    /// <summary>
    /// Menu事件 - 大分類 選擇
    /// </summary>
    protected void ddl_HouseNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Reset Menu, 中分類/小分類
        DropDownList Menu2 = this.ddl_LargeNo;
        DropDownList Menu3 = this.ddl_MediumNo;
        ResetMenuObj(Menu2, "lv2");
        ResetMenuObj(Menu3, "lv3");

        //Set 中分類
        SetMenu(Menu2, "lv2");
    }

    /// <summary>
    /// Menu事件 - 中分類 選擇
    /// </summary>
    protected void ddl_LargeNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Reset Menu, 小分類
        DropDownList Menu3 = this.ddl_MediumNo;
        ResetMenuObj(Menu3, "lv3");

        //Set 小分類
        SetMenu(Menu3, "lv3");
    }


    /// <summary>
    /// API - 取得所有分類
    /// </summary>
    private string GetAllClass()
    {
        //網址
        string url = "https://api-proskit.eclife.com.tw/v1/getCategory/";

        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string resault = JsonConvert.SerializeObject(CustomExtension.WebRequest_POST(false, url, null, reqHeaders)).ToString();
        return resault;
    }

    /// <summary>
    /// Cookie - 重新設定分類
    /// </summary>
    private void ResetAllClass()
    {
        //----- 宣告:資料參數 -----
        EcLifeRepository _data = new EcLifeRepository();

        _data.Update_ApiData("category", GetAllClass());

        _data = null;

        Response.Redirect(Page_CurrentUrl);
    }


    /// <summary>
    /// 按鈕 - 重設分類Cookie
    /// </summary>
    protected void lbtn_ClsReset_Click(object sender, EventArgs e)
    {
        //Reset
        ResetAllClass();

        //重新導向
        Response.Redirect(Page_CurrentUrl);
    }

    #endregion


    #region -- 其他設定 --

    /// <summary>
    /// 重新設定Token / 分類
    /// </summary>
    private void SetWebInfo()
    {
        string token = "";

        #region -- API取得資料 --

        //網址
        string url = "https://api-proskit.eclife.com.tw/v1/token/";

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("apikey", fn_Param.ApiKey);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, null);

        //解析Json
        JObject json = JObject.Parse(reqResult);

        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();
        token = (string)json["data"]["token"];

        //錯誤處理
        if (!rspStatus.Equals("ok"))
        {
            this.ph_ErrMessage.Visible = true;
            return;
        }

        #endregion


        #region -- 更新API值 --

        //----- 宣告:資料參數 -----
        EcLifeRepository _data1 = new EcLifeRepository();
        EcLifeRepository _data2 = new EcLifeRepository();

        if (false == _data1.Update_ApiData("token", token))
        {
            Response.Write("token壞了");
        }


        if (false == _data2.Update_ApiData("category", GetAllClass()))
        {
            Response.Write("category壞了");
        }


        _data1 = null;
        _data2 = null;

        #endregion
    }

    /// <summary>
    /// 取得商品審核狀態
    /// </summary>
    /// <param name="modelNo">品號</param>
    /// <returns>
    /// [0]: status (ok / error)
    /// [1]: message文字描述
    /// </returns>
    private string[] GetProdStatus(string modelNo)
    {
        //網址
        string url = "https://api-proskit.eclife.com.tw/v1/prodStatus/";

        //參數
        Dictionary<string, string> reqParams = new Dictionary<string, string>();
        reqParams.Add("OwnProductNO", modelNo);

        //標頭
        Dictionary<string, string> reqHeaders = new Dictionary<string, string>();
        reqHeaders.Add("token", fn_Param.Token);

        //結果
        string reqResult = CustomExtension.WebRequest_POST(false, url, reqParams, reqHeaders);

        //解析Json
        JObject json = JObject.Parse(reqResult);

        string rspStatus = json["status"].ToString();
        string rspMessage = json["message"].ToString();
        // = (string)json["data"]["cno"];
        string[] myStatus = { rspStatus, rspMessage };

        //回傳
        return myStatus;
    }


    /// <summary>
    /// 取得產品圖片
    /// </summary>
    /// <param name="modelNo">品號</param>
    /// <returns></returns>
    private string GetPicUrl(string modelNo)
    {
        using (SqlCommand cmd = new SqlCommand())
        {
            StringBuilder sbSQL = new StringBuilder();

            //[SQL] - SQL Statement
            sbSQL.AppendLine(" SELECT myData.Model_No");
            //先取電商主圖(Pic10 -> Pic01其他照舊)
            sbSQL.AppendLine(" , (SELECT TOP 1 (ISNULL(Pic10,'') + '|' + ISNULL(Pic01,'') + '|' + ISNULL(Pic02,'') + '|' + ISNULL(Pic03,'') + '|' + ISNULL(Pic04,'') ");
            sbSQL.AppendLine("    + '|' + ISNULL(Pic05,'') + '|' + ISNULL(Pic07,'') + '|' + ISNULL(Pic08,'') + '|' + ISNULL(Pic09,'')) AS PicGroup");
            sbSQL.AppendLine("     FROM ProdPic_Photo WITH (NOLOCK) WHERE (ProdPic_Photo.Model_No = myData.Model_No)");
            sbSQL.AppendLine("   ) AS PhotoGroup ");
            sbSQL.AppendLine(" FROM Prod_Item myData WITH (NOLOCK) ");
            sbSQL.AppendLine(" WHERE (UPPER(Model_No) = UPPER(@ModelNo))");

            //[SQL] - SQL Source
            cmd.CommandText = sbSQL.ToString();

            //[SQL] - Parameters
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("ModelNo", modelNo);

            //取得資料
            using (DataTable DT = dbConClass.LookupDT(cmd, out ErrMsg))
            {
                if (DT.Rows.Count == 0)
                {
                    return "";
                }

                string getPhotos = DT.Rows[0]["PhotoGroup"].ToString();
                //判斷是否為空值
                if (string.IsNullOrEmpty(getPhotos))
                {
                    return "";
                }
                else
                {
                    //圖片組合
                    return getPhotos;
                }
            }
        }
    }

    /// <summary>
    /// 取得圖片集
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <param name="type">Slide or PicList</param>
    /// <remarks>
    /// 圖片要給直接路徑
    /// </remarks>
    private string GetData_AllPic(string PhotoGroup, string Model_No)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }


        //拆解圖片值 "|"
        StringBuilder Photo = new StringBuilder();
        string[] strAry = Regex.Split(PhotoGroup, @"\|{1}");

        Photo.Append("<div class=\"row\">");

        for (int row = 0; row < strAry.Length; row++)
        {
            if (!string.IsNullOrEmpty(strAry[row].ToString()))
            {
                string PicUrl = "{0}ProductPic/{1}/{2}/{3}".FormatThis(
                    System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"]
                    , Model_No, "1", "500x500_" + strAry[row].ToString());

                Photo.Append("<div class=\"col s6 m4\">");
                Photo.Append("<div class=\"card\">");
                Photo.Append(" <div class=\"card-image\">");

                Photo.Append("<a href=\"javascript:void(0)\" onclick=\"sendPicUrl('{0}')\"><img src=\"{0}\" alt=\"{0}\"></a>".FormatThis(PicUrl));

                Photo.Append(" </div>");
                Photo.Append("</div>");
                Photo.Append("</div>");

            }
        }

        Photo.Append("</div>");

        return Photo.ToString();
    }


    /// <summary>
    /// 取得產品主圖
    /// </summary>
    /// <param name="PhotoGroup">圖片集合</param>
    /// <param name="Model_No">品號</param>
    /// <returns></returns>
    private string GetData_MainPic(string PhotoGroup, string Model_No)
    {
        //判斷參數
        if (string.IsNullOrEmpty(Model_No))
        {
            return "";
        }

        //拆解圖片值 "|"
        string Photo = "";
        string[] strAry = System.Text.RegularExpressions.Regex.Split(PhotoGroup, @"\|{1}");
        for (int row = 0; row < strAry.Length; row++)
        {
            if (false == string.IsNullOrEmpty(strAry[row].ToString()))
            {
                Photo = strAry[row].ToString();
                break;
            }
        }

        //判斷是否有圖片
        if (string.IsNullOrEmpty(Photo))
        {
            return "";
        }
        else
        {
            return "{3}ProductPic/{0}/{1}/{2}".FormatThis(
                Model_No
                , "1"
                , "500x500_{0}".FormatThis(Photo)
                , System.Web.Configuration.WebConfigurationManager.AppSettings["RefUrl"]);
        }
    }

    #endregion


    #region -- 參數設定 --
    /// <summary>
    /// 資料編號
    /// </summary>
    private string _Req_DataID;
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

    /// <summary>
    /// 分類json字串
    /// </summary>
    private string _Data_Category;
    public string Data_Category
    {
        get
        {
            String data = JsonConvert.DeserializeObject(fn_Param.Category).ToString();
            return string.IsNullOrEmpty(data) ? "" : data.ToString();
        }
        set
        {
            this._Data_Category = value;
        }
    }

    /// <summary>
    /// 目前頁面網址
    /// </summary>
    private string _Page_CurrentUrl;
    public string Page_CurrentUrl
    {
        get
        {
            return "{0}EcLife/ProductEdit.aspx?DataID={1}".FormatThis(Application["WebUrl"], Server.UrlEncode(Req_DataID));
        }
        set
        {
            this._Page_CurrentUrl = value;
        }
    }
    #endregion


}