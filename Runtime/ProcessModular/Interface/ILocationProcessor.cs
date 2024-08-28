
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ActFitFramework.Standalone.AddressableSystem
{
    public interface ILocationProcessor
    {
        UniTask<IList<IResourceLocation>> LoadLocationsAsync(string labelReferenceString, Type type = null);
    }
}