using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace libra.util
{
    static class RegularHelper
    {

        /// <summary>
        /// 验证Email格式
        /// </summary>
        /// <param name="str_Email"></param>
        /// <returns></returns>
        public static bool IsEmail(string str_Email)
        {
            return Regex.IsMatch(str_Email, @"^([/w-/.]+)@((/[[0-9]{1,3}/.[0-9] {1,3}/.[0-9]{1,3}/.)|(([/w-]+/.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(/)?]$");
        }

        /// <summary>
        /// 验证IP地址格式
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IsIP(string IP)
        {
            string num = "(25[0-5]|2[0-4]//d|[0-1]//d{2}|[1-9]?//d)";
            return Regex.IsMatch(IP, ("^" + num + "//." + num + "//." + num + "//." + num + "$"));
        }

        /// <summary>
        /// 验证URl网址格式
        /// </summary>
        /// <param name="str_url"></param>
        /// <returns></returns>
        public static bool IsUrl(string str_url)
        {
            return Regex.IsMatch(str_url, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&$%\$#\=~])*$");
        }
           
        /*
        string regstr = @"(?i)(?<=<td.*?.*?>)[^<]+(?=</td>)"; //提取td的文字           
        string regstr = @"<a\s+href=(?<url>.+?)>(?<content>.+?)</a>";   //提取链接的内容
        string regstr = @"<td.+?><a\s+href=(?<url>.+?)>(?<content>.+?)</a></td>";  //提取TD中链接的内容
        string regstr = @"<td.+?><span.+?>(?<content>.+?)</span></td>";  //提取TD中span的内容
        string regstr = @"<td.+?>(?<content>.+?)</td>";   //获取TD之间所有的内容
        string regstr = @"<td>(?<content>.+?)-<font color=#0000ff>推荐</font></td>"; //获取内容
        */
    }
}
