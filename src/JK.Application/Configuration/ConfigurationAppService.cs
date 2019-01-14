using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using JK.Configuration.Dto;

namespace JK.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : JKAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
