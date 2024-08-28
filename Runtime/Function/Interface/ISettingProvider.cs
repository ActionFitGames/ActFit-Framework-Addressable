
namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ISettingProvider
    {
        ExceptionHandleTypes GetExceptionType { get; }
        bool GetDontLoadConfig { get; }
    }
}