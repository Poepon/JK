using System.Collections.Generic;

namespace JK.MultiThemes
{
    /// <summary>
    /// 皮肤的其它加载方式
    /// </summary>
    public interface IThemeConfigStore
    {
        List<ThemeItem> GetThemes();
    }
    public class NullThemeConfigStore : IThemeConfigStore
    {
        public List<ThemeItem> GetThemes()
        {
            return new List<ThemeItem>();
        }
    }
}