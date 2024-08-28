
using System.Collections.Generic;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ICacheProvider
    {
        List<string> GetLabelStrings { get; }
        Dictionary<string, int> GetAssetKeysMap { get; }     
    }
}