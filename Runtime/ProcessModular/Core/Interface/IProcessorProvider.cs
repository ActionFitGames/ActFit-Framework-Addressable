
namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface IProcessorProvider
    {
        ILoadProcessor GetLoader();
        IReleaseProcessor GetReleaser();
    }
}