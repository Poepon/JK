using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace JK.MultiThemes
{

    public class ThemeProvider : IThemeProvider
    {
        #region Fields

        private ConcurrentDictionary<string, ThemeItem> keyTheme;
        protected readonly object SyncObj = new object();
        private List<ThemeItem> _themeItems;
        private string DefaultTheme;
        #endregion

        #region Constructors

        public ThemeProvider(IOptionsMonitor<ThemeOptions> options,
            IThemeConfigStore themeConfigStore)
        {
            DefaultTheme = options.CurrentValue.DefaultTheme;
            _themeItems = new List<ThemeItem>();
            if (options.CurrentValue.Themes != null)
            {
                _themeItems.AddRange(options.CurrentValue.Themes);
            }
            int tryCount = 0;
            List<ThemeItem> t = null;
            do
            {
                try
                {
                    t = themeConfigStore.GetThemes();
                }
                catch (Exception)
                {

                }
                if (t != null)
                {
                    break;
                }
                tryCount++;
            } while (tryCount < 3);

            if (t != null && t.Count > 0)
            {
                _themeItems.AddRange(t);
            }

            if (options.CurrentValue.Themes != null && options.CurrentValue.Themes.Count > 0)
            {
                _themeItems.AddRange(t);
            }

            keyTheme = new ConcurrentDictionary<string, ThemeItem>();
        }

        #endregion


        #region Methods

        public ThemeItem GetWorkingTheme(string domain)
        {
            string key = domain;
            if (keyTheme.ContainsKey(key))
            {
                return keyTheme[key];
            }

            lock (SyncObj)
            {
                if (keyTheme.ContainsKey(key))
                {
                    return keyTheme[key];
                }
                ThemeItem curTheme = null;

                if (GetThemes() != null && GetThemes().Count > 0)
                {
                    var query = GetThemes().OrderBy(t => t.Order).Where(t =>
                        t.SupportDomainAdapter && t.Domains.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Any(s => s == domain || t.SupportRegex && new Regex(s).IsMatch(domain)));
                    curTheme = curTheme = query.FirstOrDefault(t => t.Order == 1);
                    if (curTheme == null)
                    {
                        curTheme = query.FirstOrDefault();
                    }

                    if (curTheme == null)
                    {
                        curTheme = GetThemes().FirstOrDefault();
                    }
                    if (curTheme == null)
                    {
                        curTheme = GetThemes().FirstOrDefault(t =>
                            t.ThemeName == DefaultTheme);
                    }
                }

                keyTheme[key] = curTheme;
                return curTheme;
            }
        }

        public void SetWorkingTheme(string domain, string themeName)
        {
            string key = domain;
            lock (SyncObj)
            {
                keyTheme[key] = GetThemes().SingleOrDefault(t => t.ThemeName == themeName);
            }
        }

        public ThemeItem GetTheme(string themeName)
        {
            return GetThemes().SingleOrDefault(x => x.ThemeName.Equals(themeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public IList<ThemeItem> GetThemes()
        {
            return _themeItems;
        }

        public bool ThemeExists(string themeName)
        {
            return GetThemes().Any(configuration => configuration.ThemeName.Equals(themeName, StringComparison.CurrentCultureIgnoreCase));
        }

        public string GetWorkingThemeDirPath(string domain, bool isMob)
        {
            var theme = GetWorkingTheme(domain);
            if (theme != null)
            {
                if (isMob && !string.IsNullOrWhiteSpace(theme.MobThemeDirPath))
                {
                    return theme.MobThemeDirPath;
                }

                return theme.ThemeDirPath;
            }

            return string.Empty;
        }

        #endregion
    }
}
