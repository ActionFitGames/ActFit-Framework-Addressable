
using System.IO;
using UnityEngine;

namespace ActFitFramework.Standalone.AddressableSystem.Editor
{
    public static class EditorExtensions
    {
        /// <summary>
        /// Ensures that a specified directory exists, creating it if necessary.
        /// Logs the creation of the directory.
        /// </summary>
        /// <param name="path">The path to the directory to ensure exists.</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }
            
            Directory.CreateDirectory(path);
            Debug.Log($"Created Directory: {path}");
        }
    }
}