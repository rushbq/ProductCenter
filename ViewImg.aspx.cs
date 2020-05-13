using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Net;
using System.IO;
using ExtensionMethods;

public partial class _Test_ViewImg : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        String reqImgUrl = Request.QueryString["url"];
        String reqWidth = Request.QueryString["width"];
        String reqHeight = Request.QueryString["height"];
        String ErrMsg;
        //判斷來源路徑
        if (false == fn_Extensions.String_字數(reqImgUrl, "1", "255", out ErrMsg))
        {
            return;
        }
        //判斷來源寬度
        if (false == fn_Extensions.Num_正整數(reqWidth, "0", "1024", out ErrMsg))
        {
            return;
        }
        //判斷來源高度
        if (false == fn_Extensions.Num_正整數(reqHeight, "0", "1024", out ErrMsg))
        {
            return;
        }
        //產生縮圖
        renderThumb(reqImgUrl, Convert.ToInt32(reqWidth), Convert.ToInt32(reqHeight));
    }

    /// <summary>
    /// 即時產生縮圖
    /// </summary>
    /// <param name="inputImg">來源圖檔路徑</param>
    /// <param name="w">寬</param>
    /// <param name="h">高</param>
    /// <returns></returns>
    /// <remarks></remarks>
    void renderThumb(string inputImg, int w, int h)
    {
        int width = 0;
        int height = 0;

        WebClient wc = new WebClient();
        MemoryStream ms = new MemoryStream(wc.DownloadData(inputImg));
        System.Drawing.Image image = new Bitmap(ms);

        //取得圖檔寬高
        width = image.Width;
        height = image.Height;

        //重新設定寬高 (等比例)
        if (!(width < w & height < h))
        {
            if (width > height)
            {
                h = w * height / width;
            }
            else
            {
                w = h * width / height;
            }
        }

        //產生縮圖
        System.Drawing.Bitmap img = new System.Drawing.Bitmap(w, h);
        Graphics graphic = Graphics.FromImage(img);
        //將品質設定為HighQuality
        graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        //重畫縮圖
        graphic.DrawImage(image, 0, 0, w, h);

        image.Dispose();

        //輸出縮圖
        System.IO.MemoryStream ms_r = new System.IO.MemoryStream();
        img.Save(ms_r, System.Drawing.Imaging.ImageFormat.Jpeg);
        HttpContext.Current.Response.ClearContent();
        HttpContext.Current.Response.ContentType = "image/Jpeg";
        HttpContext.Current.Response.BinaryWrite(ms_r.ToArray());

        img.Dispose();
        graphic.Dispose();

    }
}