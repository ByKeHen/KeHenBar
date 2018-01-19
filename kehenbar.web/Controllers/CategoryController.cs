using kehenbar.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    [IsLogin]
    public class CategoryController : Controller
    {
        //
        // GET: /Category/
        kehenbarEntities db = new kehenbarEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var data = db.mb_categorys.ToList();
            
            return View(data);
        }

        public ActionResult Edit(int id)
        {
            var data = db.mb_categorys.Where(o=>o.cid == id).FirstOrDefault();
            return View(data);
        }

        /// <summary>
        /// 添加文章分类
        /// </summary>
        /// <param name="mingcheng"></param>
        /// <param name="shifouXS"></param>
        /// <returns></returns>
        public string Add(string mingcheng, int shifouXS)
        {
            mb_categorys c = db.mb_categorys.Where(o => o.cname == mingcheng).FirstOrDefault();
            if (c!=null)
            {
                return "已经存在这个分类";
            }
            else
            {
                c = new mb_categorys();
                c.cname = mingcheng;
                c.cshow = shifouXS;
                c.ctime = DateTime.Now;

                db.mb_categorys.Add(c);
                int res = db.SaveChanges();
                if (res > 0)
                {
                    return "添加成功";
                }
                else
                {
                    return "添加失败";
                }
            }
        }

        public string Save(string mingcheng, int shifouXS, string tubiao, int cid)
        {
            mb_categorys ob = db.mb_categorys.Where(o => o.cid == cid).FirstOrDefault();
            if (ob == null)
            {
                return "没有找到这个分类";
            }
            else
            {
                ob.cname = mingcheng;
                ob.cshow = shifouXS;
                ob.pic = tubiao;
                int res = db.SaveChanges();
                if (res > 0)
                {
                    return "修改成功";
                }
                else
                {
                    return "修改失败";
                }
            }
        }
    }
}
