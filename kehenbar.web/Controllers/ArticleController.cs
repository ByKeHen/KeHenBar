using myBlog.model;
using myBlog.web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myBlog.web.Controllers
{
    
    public class ArticleController : Controller
    {
        //
        // GET: /Article/

        myBlogEntities db = new myBlogEntities();
        [IsLogin]
        public ActionResult Index()
        {
            List<mb_categorys> cateList = db.mb_categorys.ToList();
            
            return View(cateList);
        }
        [IsLogin]
        public ActionResult List()
        {
            List<GetArticleList_Result> articleList = GetListByPage(20, 1);
            ViewBag.PageCount = GetPageCount(20);
            return View(articleList);
        }
        [IsLogin]
        public ActionResult Edit(int id)
        {
            List<mb_categorys> cateList = db.mb_categorys.ToList();
            ViewBag.Article = db.mb_articles.Where(o => o.aid == id).FirstOrDefault();
            return View(cateList);
        }

        /// <summary>
        /// 分页获取文章列表 - 后台使用
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [IsLogin]
        public string ListByPage(int page)
        {
            List<GetArticleList_Result> articleList = GetListByPage(20, page);
            string json = JsonConvert.SerializeObject(articleList);
            return json;
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="mingcheng"></param>
        /// <param name="zuozhe"></param>
        /// <param name="neirong"></param>
        /// <param name="guanjianzi"></param>
        /// <param name="jianjie"></param>
        /// <returns></returns>
        [IsLogin]
        [ValidateInput(false)]
        public string Add(int fenlei,string mingcheng, string zuozhe, string neirong, string guanjianzi, string jianjie)
        {
            mb_articles article = new mb_articles();
            article.aAuthor = zuozhe;
            article.acontent = neirong;
            article.adatetime = DateTime.Now;
            article.adescription = jianjie;
            article.akeywords = guanjianzi;
            article.aname = mingcheng;
            article.atitle = mingcheng;
            article.uid = int.Parse(Session["uid"] + "");
            article.cid = fenlei;

            db.mb_articles.Add(article);
            int res = db.SaveChanges();
            if (res > 0)
            {
                return "ok";
            }
            else
            {
                return "no";
            }
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="mingcheng"></param>
        /// <param name="zuozhe"></param>
        /// <param name="neirong"></param>
        /// <param name="guanjianzi"></param>
        /// <param name="jianjie"></param>
        /// <returns></returns>
        [IsLogin]
        [ValidateInput(false)]
        public string EditSave(int aid,int fenlei, string mingcheng, string zuozhe, string neirong, string guanjianzi, string jianjie)
        {
            mb_articles article = db.mb_articles.Where(o => o.aid == aid).FirstOrDefault();
            if (article == null)
            {
                return "no";
            }
            else
            {
                article.aAuthor = zuozhe;
                article.acontent = neirong;
                article.adescription = jianjie;
                article.akeywords = guanjianzi;
                article.aname = mingcheng;
                article.atitle = mingcheng;
                article.cid = fenlei;

                int res = db.SaveChanges();
                if (res > 0)
                {
                    return "ok";
                }
                else
                {
                    return "no";
                }
            }
        }
        /// <summary>
        /// 文章内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Info(int id)
        {
            mb_articles article = db.mb_articles.Where(o => o.aid == id).FirstOrDefault();
            ViewBag.PingLun = GetPinLunList(id);
            return View(article);
        }

        /// <summary>
        /// 发布评论
        /// </summary>
        /// <param name="pinglun"></param>
        /// <param name="aid"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public string PingLun(string pinglun, int aid) 
        {
            
            mb_comments com = new mb_comments();
            com.aid = aid;
            com.cContent = pinglun;
            com.ctime = DateTime.Now;
            if (!string.IsNullOrEmpty(Session["uid"] + ""))
            {
                com.uid = int.Parse(Session["uid"] + "");
            }

            db.mb_comments.Add(com);
            int res = db.SaveChanges();
            if (res > 0)
            {
                return "评论成功";
            }
            else
            {
                return "评论失败";
            }
        }

        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public int GetPageCount(int pageSize)
        {
            int count = db.mb_articles.Count();
            int page = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
            return page;
        }

        /// <summary>
        /// 分页获取文章数据
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="page">第几页</param>
        /// <returns></returns>
        public List<GetArticleList_Result> GetListByPage(int pageSize, int page)
        {

            var data = db.GetArticleList()
                .OrderByDescending(o => o.aid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new
                {
                    o.pic,
                    o.atitle,
                    o.aid,
                    o.adatetime,
                    o.aAuthor
                })
                .ToList();

            string json = JsonConvert.SerializeObject(data);

            return JsonConvert.DeserializeObject<List<GetArticleList_Result>>(json);
        }

        /// <summary>
        /// 分页获取文章数据JSON
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="page">第几页</param>
        /// <returns></returns>
        public string GetJsonListByPage(int pageSize, int page)
        {
            List<GetArticleList_Result> articleList = GetListByPage(pageSize, page);
            string json = JsonConvert.SerializeObject(articleList);
            return json;
        }

        /// <summary>
        /// 根据文章ID获取评论列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<mb_comments> GetPinLunList(int id)
        {
            List<mb_comments> pl = db.mb_comments.Where(o => o.aid == id)
                .OrderByDescending(o=>o.cid)
                .ToList();
            return pl;
        }

        /// <summary>
        /// 获取最热文章列表
        /// </summary>
        /// <returns></returns>
        public List<GetArticleListZR_Result> GetArticleListZR()
        {
            var data = db.GetArticleListZR().ToList();

            return data;
        }


    }
}
