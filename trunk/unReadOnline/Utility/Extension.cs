using System;
using System.Web;


namespace unReadOnline.Utility
{
    /// <summary>
    ///扩展String 
    /// </summary>
    public static class StringExtension
    {
        public static string UrlEncode(this string target)
        {
            if (target.IndexOf(" ") > -1)
            {
                target = target.Replace(" ", "_");
            }

            if (target.IndexOf("#") > -1)
            {
                target = target.Replace("#", "_sharp_");
            }

            if (target.IndexOf("&") > -1)
            {
                target = target.Replace("&", "_amp_");
            }

            if (target.IndexOf(".") > -1)
            {
                target = target.Replace(".", "_dot_");
            }

            if (target.IndexOf("/") > -1)
            {
                target = target.Replace("/", "_fws_");
            }

            if (target.IndexOf("\\") > -1)
            {
                target = target.Replace("\\", "_bks_");
            }

            target = HttpUtility.UrlEncode(target);

            return target;
        }

        public static string UrlDecode(this string target)
        {
            target = HttpUtility.UrlDecode(target);

            if (target.IndexOf("_") > -1)
            {
                target = target.Replace("_", " ");
            }

            if (target.IndexOf("_sharp_") > -1)
            {
                target = target.Replace("_sharp_", "#");
            }

            if (target.IndexOf("_amp_") > -1)
            {
                target = target.Replace("_amp_", "&");
            }

            if (target.IndexOf("_dot_") > -1)
            {
                target = target.Replace("_dot_", ".");
            }

            if (target.IndexOf("_fws_") > -1)
            {
                target = target.Replace("_fws_", "/");
            }

            if (target.IndexOf("_bks_") > -1)
            {
                target = target.Replace("_bks_", "\\");
            }

            return target;
        }
    }
}
