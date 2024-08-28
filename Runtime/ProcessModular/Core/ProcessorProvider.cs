
using Cysharp.Threading.Tasks;

namespace ActFitFramework.Standalone.AddressableSystem
{
    /// <summary>
    /// Provides various processors such as InitializeProcessor, LocationProcessor, LoadProcessor, 
    /// and ReleaseProcessor for the AddressableSystem. It manages the initialization and retrieval 
    /// of these processors.
    /// </summary>
    internal class ProcessorProvider : IProcessorProvider
    {
        #region Member

        private readonly AddressableSystem _addressableSystem;
        private readonly ProcessCallbackSystem _processCallbackSystem;

        private readonly IInitializeProcessor _initializeProcessor;
        private readonly ILocationProcessor _locationProcessor;
        private readonly ILoadProcessor _loadProcessor;
        private readonly IReleaseProcessor _releaseProcessor;

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorProvider"/> class.
        /// </summary>
        /// <param name="addressableSystem">The addressable system instance to associate with the processors.</param>
        /// <param name="processCallbackSystem">The callback system for processing events and state changes.</param>
        internal ProcessorProvider(AddressableSystem addressableSystem, ProcessCallbackSystem processCallbackSystem)
        {
            _addressableSystem = addressableSystem;
            _processCallbackSystem = processCallbackSystem;
            
            AddressableMonoBehavior.CreateAddressableMonoBehavior();

            _initializeProcessor = new InitializeProcessor(_processCallbackSystem);
            _locationProcessor = new LocationProcessor(_processCallbackSystem);
            _loadProcessor = new LoadProcessor(_addressableSystem);
            _releaseProcessor = new ReleaseProcessor(_addressableSystem);

            AddressableMonoBehavior.ActivateFromInitializeAsync(_initializeProcessor, _locationProcessor,
                OnInitializeValidate).Forget();
        }

        /// <summary>
        /// Callback method that is invoked after initialization to validate the process.
        /// </summary>
        /// <param name="isComplete">Indicates whether the initialization was successful.</param>
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

        /// <summary>
        /// Retrieves the LoadProcessor instance.
        /// </summary>
        /// <returns>The LoadProcessor associated with the AddressableSystem.</returns>
        public ILoadProcessor GetLoader()
        {
            return _loadProcessor;
        }

        /// <summary>
        /// Retrieves the ReleaseProcessor instance.
        /// </summary>
        /// <returns>The ReleaseProcessor associated with the AddressableSystem.</returns>
        public IReleaseProcessor GetReleaser()
        {
            return _releaseProcessor;
        }
    }
}