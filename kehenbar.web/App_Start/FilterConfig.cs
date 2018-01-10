using System.Web;
using System.Web.Mvc;

namespace kehenbar.web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

    }
    /// <summary>
    /// 过滤器
    /// 验证是否登录 - 管理员
    /// </summary>
    public class IsLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = filterContext.HttpContext.Session["UserId"] + "";
            if (string.IsNullOrEmpty(userId) || int.Parse(userId) !=2)
            {
                //如果验证失败，则返回登陆页
                filterContext.Result = new RedirectResult("/admin/login");
            }
        }
    }

    /// <summary>
    /// 过滤器
    /// 验证是否登录 - 会员
    /// </summary>
    public class IsLoginMember : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string userId = filterContext.HttpContext.Session["UserId"] + "";
            if (string.IsNullOrEmpty(userId))
            {
                //如果验证失败，则返回登陆页
                filterContext.Result = new RedirectResult("/member/login");
            }
        }
    }
}