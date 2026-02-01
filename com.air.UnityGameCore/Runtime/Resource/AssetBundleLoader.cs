using System;
using System.Collections.Generic;
using UnityEngine;

namespace Resource
{
    /// <summary>
    /// AssetBundle 信息
    /// </summary>
    public class AssetBundleInfo
    {
        public string BundlePath;
        public AssetBundle Bundle;
        public int RefCount;
        public List<string> Dependencies;
    }

    /// <summary>
    /// AssetBundle 加载器 - 负责 AssetBundle 的加载、卸载和依赖管理
    /// </summary>
    public class AssetBundleLoader
    {
        private readonly Dictionary<string, AssetBundleInfo> _bundleInfoDict = new();
        
        // AssetBundle 根目录
        private readonly string _bundleRootPath;
        
        // 依赖关系获取函数，可以从 Manifest 获取
        private readonly Func<string, string[]> _dependenciesResolver;

        public AssetBundleLoader(string bundleRootPath, Func<string, string[]> dependenciesResolver = null)
        {
            _bundleRootPath = bundleRootPath;
            _dependenciesResolver = dependenciesResolver ?? (_ => Array.Empty<string>());
        }

        /// <summary>
        /// 异步加载 AssetBundle
        /// </summary>
        public void LoadBundleAsync(string bundlePath, Action<AssetBundle> callback)
        {
            // 如果 Bundle 已经加载
            if (_bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo))
            {
                bundleInfo.RefCount++;
                callback?.Invoke(bundleInfo.Bundle);
                return;
            }

            // 先加载依赖
            var dependencies = _dependenciesResolver(bundlePath);
            if (dependencies != null && dependencies.Length > 0)
            {
                LoadDependenciesAsync(dependencies, () =>
                {
                    LoadBundleFromFile(bundlePath, dependencies, callback);
                });
            }
            else
            {
                LoadBundleFromFile(bundlePath, null, callback);
            }
        }

        /// <summary>
        /// 异步加载所有依赖
        /// </summary>
        private void LoadDependenciesAsync(string[] dependencies, Action onComplete)
        {
            if (dependencies == null || dependencies.Length == 0)
            {
                onComplete?.Invoke();
                return;
            }

            int loadedCount = 0;
            int totalCount = dependencies.Length;

            foreach (var dep in dependencies)
            {
                LoadDependencyBundleAsync(dep, () =>
                {
                    loadedCount++;
                    if (loadedCount >= totalCount)
                    {
                        onComplete?.Invoke();
                    }
                });
            }
        }

        /// <summary>
        /// 加载依赖的 Bundle
        /// </summary>
        private void LoadDependencyBundleAsync(string bundlePath, Action callback)
        {
            // 如果依赖 Bundle 已经加载
            if (_bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo))
            {
                bundleInfo.RefCount++;
                callback?.Invoke();
                return;
            }

            // 加载依赖的 Bundle
            var fullPath = System.IO.Path.Combine(_bundleRootPath, bundlePath);
            var request = AssetBundle.LoadFromFileAsync(fullPath);
            request.completed += operation =>
            {
                var loadRequest = operation as AssetBundleCreateRequest;
                var bundle = loadRequest?.assetBundle;

                if (bundle != null)
                {
                    var newBundleInfo = new AssetBundleInfo
                    {
                        BundlePath = bundlePath,
                        Bundle = bundle,
                        RefCount = 1,
                        Dependencies = new List<string>()
                    };
                    _bundleInfoDict[bundlePath] = newBundleInfo;
                }
                else
                {
                    Debug.LogError($"Failed to load dependency AssetBundle: {bundlePath}");
                }

                callback?.Invoke();
            };
        }

        /// <summary>
        /// 从文件加载 Bundle
        /// </summary>
        private void LoadBundleFromFile(string bundlePath, string[] dependencies, Action<AssetBundle> callback)
        {
            var fullPath = System.IO.Path.Combine(_bundleRootPath, bundlePath);
            var request = AssetBundle.LoadFromFileAsync(fullPath);
            request.completed += operation =>
            {
                var loadRequest = operation as AssetBundleCreateRequest;
                var bundle = loadRequest?.assetBundle;

                if (bundle != null)
                {
                    var newBundleInfo = new AssetBundleInfo
                    {
                        BundlePath = bundlePath,
                        Bundle = bundle,
                        RefCount = 1,
                        Dependencies = dependencies != null ? new List<string>(dependencies) : new List<string>()
                    };
                    _bundleInfoDict[bundlePath] = newBundleInfo;
                    callback?.Invoke(bundle);
                }
                else
                {
                    Debug.LogError($"Failed to load AssetBundle from path: {fullPath}");
                    callback?.Invoke(null);
                }
            };
        }

        /// <summary>
        /// 获取已加载的 Bundle
        /// </summary>
        public AssetBundle GetLoadedBundle(string bundlePath)
        {
            return _bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo) ? bundleInfo.Bundle : null;
        }

        /// <summary>
        /// 检查 Bundle 是否已加载
        /// </summary>
        public bool IsBundleLoaded(string bundlePath)
        {
            return _bundleInfoDict.ContainsKey(bundlePath) && _bundleInfoDict[bundlePath].Bundle != null;
        }

        /// <summary>
        /// 卸载 AssetBundle
        /// </summary>
        public void UnloadBundle(string bundlePath)
        {
            if (!_bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo))
            {
                return;
            }

            bundleInfo.RefCount--;

            // 引用计数为 0 时才真正卸载 Bundle
            if (bundleInfo.RefCount <= 0)
            {
                // 先卸载依赖
                if (bundleInfo.Dependencies != null && bundleInfo.Dependencies.Count > 0)
                {
                    foreach (var dep in bundleInfo.Dependencies)
                    {
                        UnloadDependencyBundle(dep);
                    }
                }

                // 卸载自己
                if (bundleInfo.Bundle != null)
                {
                    bundleInfo.Bundle.Unload(true);
                    bundleInfo.Bundle = null;
                }
                
                _bundleInfoDict.Remove(bundlePath);
            }
        }

        /// <summary>
        /// 卸载依赖的 Bundle
        /// </summary>
        private void UnloadDependencyBundle(string bundlePath)
        {
            if (!_bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo))
            {
                return;
            }

            bundleInfo.RefCount--;

            // 引用计数为 0 时才真正卸载依赖 Bundle
            if (bundleInfo.RefCount <= 0)
            {
                if (bundleInfo.Bundle != null)
                {
                    bundleInfo.Bundle.Unload(true);
                    bundleInfo.Bundle = null;
                }
                
                _bundleInfoDict.Remove(bundlePath);
            }
        }

        /// <summary>
        /// 卸载所有未使用的 Bundle
        /// </summary>
        public void UnloadUnusedBundles()
        {
            var bundlesToRemove = new List<string>();
            
            foreach (var kvp in _bundleInfoDict)
            {
                if (kvp.Value.RefCount <= 0)
                {
                    bundlesToRemove.Add(kvp.Key);
                }
            }

            foreach (var bundlePath in bundlesToRemove)
            {
                var bundleInfo = _bundleInfoDict[bundlePath];
                if (bundleInfo.Bundle != null)
                {
                    bundleInfo.Bundle.Unload(true);
                    bundleInfo.Bundle = null;
                }
                _bundleInfoDict.Remove(bundlePath);
            }
        }

        /// <summary>
        /// 清空所有 Bundle
        /// </summary>
        public void Clear()
        {
            foreach (var kvp in _bundleInfoDict)
            {
                if (kvp.Value.Bundle != null)
                {
                    kvp.Value.Bundle.Unload(true);
                }
            }
            _bundleInfoDict.Clear();
        }

        /// <summary>
        /// 获取所有已加载的 Bundle 信息（调试用）
        /// </summary>
        public Dictionary<string, AssetBundleInfo> GetAllBundles()
        {
            return _bundleInfoDict;
        }

        /// <summary>
        /// 获取 Bundle 引用计数（调试用）
        /// </summary>
        public int GetBundleRefCount(string bundlePath)
        {
            return _bundleInfoDict.TryGetValue(bundlePath, out var bundleInfo) ? bundleInfo.RefCount : 0;
        }
    }
}
