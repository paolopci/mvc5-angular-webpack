using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHomeService
{
    HomeInfoDto GetHomeInfo();

    ModuleInfoDto? GetModuleInfo(string key);
}
