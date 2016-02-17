using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVManager2.avManager
{
    static class Config
    {
        public const string IMG_PATH = @"H:\avdb\";

        public const string ACTRESS_PATH = IMG_PATH + @"actress\";

        public static List<string> VIDEO_PATH = new List<string>() { IMG_PATH + @"video\", @"G:\avdb\video\" };

        ///// <summary>
        ///// 第一个放视频的目录
        ///// </summary>
        //public const string VIDEO_PATH = IMG_PATH + @"video\";

        ///// <summary>
        ///// 第二个放视频的目录
        ///// </summary>
        //public const string VIDEO_PATH1 = @"G:\avdb\video\";
    }
}
