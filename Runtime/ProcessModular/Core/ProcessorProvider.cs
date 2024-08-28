
using Cysharp.Threading.Tasks;

namespace ActFitFramework.Standalone.AddressableSystem
{
    internal class ProcessorProvider : IProcessorProvider
    {
        #region Member

        private readonly AddressableSystem _addressableSystem;
        private readonly ProcessCallbackSystem _processCallbackSystem;

        private readonly IInitializeProcessor _initializeProcessor;
        private readonly ILocationProcessor _locationProcessor;
        private readonly ILoadProcessor _loadProcessor;

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addressableSystem"></param>
        /// <param name="processCallbackSystem"></param>
        internal ProcessorProvider(AddressableSystem addressableSystem, ProcessCallbackSystem processCallbackSystem)
        {
            _addressableSystem = addressableSystem;
            _processCallbackSystem = processCallbackSystem;
            
            AddressableMonoBehavior.CreateAddressableMonoBehavior();

            _initializeProcessor = new InitializeProcessor(_processCallbackSystem);
            _locationProcessor = new LocationProcessor(_processCallbackSystem);
            _loadProcessor = new LoadProcessor(_addressableSystem);

            AddressableMonoBehavior.ActivateFromInitializeAsync(_initializeProcessor, _locationProcessor,
                OnInitializeValidate).Forget();
        }

        private void OnInitializeValidate(bool isComplete)
        {
            _addressableSystem.IsInitialize = isComplete;

            if (_addressableSystem.IsInitialize)
            {
                DeLog.Log("[Addressable System] Activate Initialize async Complete.");
            }
            else
            {
                DeLog.LogError("[Addressable System] Activate Initialize async Failed.");
            }
        }

        public ILoadProcessor GetLoader()
        {
            return _loadProcessor;
        }
    }
}