using WebCore9.Core.Abstractions;
using WebCore9.Core.Models;

namespace WebCore9.Infrastructure.Services;

public sealed class HomeService : IHomeService
{
    private static readonly string[] SharedScripts =
    [
        "~/Scripts/ng2/polyfills.js",
        "~/Scripts/ng2/vendors.js"
    ];

    public HomeInfoDto GetHomeInfo()
    {
        return new HomeInfoDto
        {
            DefaultModule = "module1",
            AvailableModules = ["module1", "module2"],
            LegacyController = "Home"
        };
    }

    public ModuleInfoDto GetModule1Info()
    {
        return new ModuleInfoDto
        {
            RouteKey = "module1",
            Title = "Module 1",
            ScriptFiles = [.. SharedScripts, "~/Scripts/ng2/module1.js"],
            RootElementTag = "my-angular-app",
            LoadingText = "Loading...."
        };
    }

    public ModuleInfoDto GetModule2Info()
    {
        return new ModuleInfoDto
        {
            RouteKey = "module2",
            Title = "Module 2",
            ScriptFiles = [.. SharedScripts, "~/Scripts/ng2/module2.js"],
            RootElementTag = "tour-of-heroes",
            LoadingText = "Loading...."
        };
    }
}
