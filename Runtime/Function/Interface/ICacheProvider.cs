
using System.Collections.Generic;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ICacheProvider
    {
        public List<string> GetLabelReferencesString { get; }
        public Dictionary<string, List<string>> GetLabelLocationsMap { get; }
        public Dictionary<string, AddressableKey> GetAssetLocationsMap { get; }     
    }
}