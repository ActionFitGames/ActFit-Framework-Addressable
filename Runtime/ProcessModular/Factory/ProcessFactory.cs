
namespace ActFitFramework.Standalone.AddressableSystem
{
    internal static class ProcessFactory
    {
        internal static IProcessorProvider GetAddressableProcessorProvider(
            AddressableSystem addressableSystem, ProcessCallbackSystem processCallbackSystem)
        {
            DeLog.Log("[Process Factory] Create Addressable processor provider.");
            return new ProcessorProvider(addressableSystem, processCallbackSystem);
        }
    }
}