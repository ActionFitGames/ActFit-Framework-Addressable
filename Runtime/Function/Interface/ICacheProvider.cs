
using System.Collections.Generic;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ICacheProvider
    {
        public List<string> GetLabelStrings { get; }
        public Dictionary<string, AddressableKey> GetAssetKeysMap { get; }     
    }
}