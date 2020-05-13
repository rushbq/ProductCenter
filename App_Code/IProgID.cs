using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/*
 * [自訂Interface]
 * 設定主版的編號, 子版在執行時填入所屬編號, 讓選單可以判斷要highlight 哪個項目
 * 
 * 
 * -- 子版範例 --
 *  if (false == setProgIDs.setID(this.Master, "第一層編號", "第二層編號"))
    {
        throw new Exception("目前的Masterpage 沒有實作 IProgID,無法傳值");
    }
 */

public interface IProgID
{
    void setProgID(string UpID, string SubID);
}

public class setProgIDs
{
    /// <summary>
    /// 設定編號
    /// </summary>
    /// <param name="myObj">Master主版面</param>
    /// <param name="UpID">第一層編號</param>
    /// <param name="SubID">第二層編號</param>
    /// <returns></returns>
    public static bool setID(object myObj, string UpID, string SubID)
    {
        try
        {
            IProgID master = myObj as IProgID;
            if (master == null)
            {
                return false;
            }
            master.setProgID(UpID, SubID);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}