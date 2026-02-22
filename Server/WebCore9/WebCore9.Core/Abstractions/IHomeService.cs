using WebCore9.Core.Models;

namespace WebCore9.Core.Abstractions;

public interface IHomeService
{
    HomeInfoDto GetHomeInfo();

    IReadOnlyList<ModuleInfoDto> GetModules();

    ModuleInfoDto? GetModuleInfo(string key);

    LoaderInfoDto GetLoaderInfo();

    LoaderChunkManifestDto GetLoaderChunkManifest();
}
