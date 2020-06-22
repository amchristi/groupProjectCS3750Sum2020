using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString GetUserProfileImage(this HtmlHelper html, byte[] image)
        {
            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
            return new MvcHtmlString("<img src='" + img + "' />");
        }
    }
}