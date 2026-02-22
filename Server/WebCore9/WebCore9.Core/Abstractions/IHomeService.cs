using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHomeService
{
    HomeInfoDto GetHomeInfo();

    ModuleInfoDto GetModule1Info();

    ModuleInfoDto GetModule2Info();
}
