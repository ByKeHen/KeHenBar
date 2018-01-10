using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kehenbar.web.Models
{
    /// <summary>
    /// 上传类，回传信息使用
    /// </summary>
    public class Upload
    {
        public int code { get; set; }   //0表示成功，其它失败
        public string msg { get; set; } //提示信息 //一般上传失败后返回
        public Data data = new Data();
    }

    public class Data
    {
        public string src { get; set; }     //"图片路径"
        public string title { get; set; }   //"图片名称" //可选
    }
}