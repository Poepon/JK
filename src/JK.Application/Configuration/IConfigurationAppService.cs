using System.Threading.Tasks;
using JK.Configuration.Dto;

namespace JK.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
