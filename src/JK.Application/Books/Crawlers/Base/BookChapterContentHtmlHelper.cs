using System.Text.RegularExpressions;

namespace JK.Books.Crawlers
{
   
    public static class BookChapterHtmlHelper
    {
        public static string GetChineseTitle(string title)
        {
            string reg = "[0-9]\\d*";
            var m = Regex.Match(title, reg);
            if (m.Success)
            {
                if (int.TryParse(m.Value, out int result))
                {
                    if (result < 100000)
                    {
                        int index = title.IndexOf(m.Value);
                        if (index < 3)
                        {
                            if (!title.Contains("第") && !title.Contains("章"))
                            {
                                title = title.Replace(m.Value, $"第{NumberToChinese(result)}章");
                            }
                            else
                            {
                                title = title.Replace(m.Value, NumberToChinese(result));
                            }
                        }
                    }
                }
            }
            return title;
        }
        
        /// <summary>
        /// 数字转中文
        /// </summary>
        /// <param name="number">eg: 22</param>
        /// <returns></returns>
        private static string NumberToChinese(int number)
        {
            string s = number.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.");
            string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\　　.]|$))))", "${b}${z}");
            return Regex.Replace(d, ".", m => "负元空零一二三四五六七八九空空空空空空空分角十百千万亿兆京垓秭穰"[m.Value[0] - '-'].ToString());
        }
    }
}