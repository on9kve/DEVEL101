using System.Configuration;

namespace DEVEL101.Properties
{
    // Redirect storage to %LOCALAPPDATA%\DEVEL101\user.config
    [SettingsProvider(typeof(LocalSettingsProvider))]
    internal sealed partial class Settings { }
}
