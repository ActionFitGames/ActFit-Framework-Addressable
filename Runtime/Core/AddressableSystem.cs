using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public class AddressableSystem
    {
        #region Fields

        private static volatile AddressableSystem _sInstance;

        public static AddressableSystem Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    DeLog.Log("Initialize the AddressableSystem instance.");
                    
                }

                return _sInstance;
            }
        }

        #endregion



        #region Constructor

        private AddressableSystem()
        {
            
        }

        #endregion
    }
}