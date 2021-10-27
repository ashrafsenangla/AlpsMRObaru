using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace AspNetGroupBasedPermissions
{
    public static class HMTLHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }
        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }

        public static string IsChoosed(this HtmlHelper html, string controllers = "", string actions = "", string cssClass = "selected")
        {
            ViewContext viewContext = html.ViewContext;
            bool isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            RouteValueDictionary routeValues = viewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (String.IsNullOrEmpty(actions))
                actions = currentAction;

            if (String.IsNullOrEmpty(controllers))
                controllers = currentController;

            string[] acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : String.Empty;
        }

 

    public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string itemText, string actionName, string controllerName, MvcHtmlString[] childElements = null)
    {
        var currentAction = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
        var currentController = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
        string finalHtml;
        var linkBuilder = new TagBuilder("a");
        var liBuilder = new TagBuilder("li");
 
        if (childElements != null && childElements.Length > 0)
        {
            linkBuilder.MergeAttribute("href", "#");
            linkBuilder.AddCssClass("dropdown-toggle");
            linkBuilder.InnerHtml = itemText + " <b class=\"caret\"></b>";
            linkBuilder.MergeAttribute("data-toggle", "dropdown");
            var ulBuilder = new TagBuilder("ul");
            ulBuilder.AddCssClass("dropdown-menu");
            ulBuilder.MergeAttribute("role", "menu");
            foreach (var item in childElements)
            {
                ulBuilder.InnerHtml += item + "\n";
            }
 
            liBuilder.InnerHtml = linkBuilder + "\n" + ulBuilder;
            liBuilder.AddCssClass("dropdown");
            if (controllerName == currentController)
            {
                liBuilder.AddCssClass("active");
            }
 
            finalHtml = liBuilder.ToString() + ulBuilder;
        }
        else
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
            linkBuilder.MergeAttribute("href", urlHelper.Action(actionName, controllerName));
            linkBuilder.SetInnerText(itemText);
            liBuilder.InnerHtml = linkBuilder.ToString();
            if (controllerName == currentController && actionName == currentAction)
            {
                liBuilder.AddCssClass("active");
            }
 
            finalHtml = liBuilder.ToString();
        }
 
        return new MvcHtmlString(finalHtml);
    }

    public static string IsActive(this HtmlHelper htmlHelper, string controller, string action)
    {
        var routeData = htmlHelper.ViewContext.RouteData;

        var routeAction = routeData.Values["action"].ToString();
        var routeController = routeData.Values["controller"].ToString();

        var returnActive = (controller == routeController && action == routeAction);

        return returnActive ? "active" : "";
    }

    public static MvcHtmlString IsActive(this HtmlHelper htmlHelper, string action, string controller, string activeClass, string inActiveClass = "")
    {
        var routeData = htmlHelper.ViewContext.RouteData;

        var routeAction = routeData.Values["action"].ToString();
        var routeController = routeData.Values["controller"].ToString();

        var returnActive = (controller == routeController && action == routeAction);

        return new MvcHtmlString(returnActive ? activeClass : inActiveClass);
    }


	}
}
