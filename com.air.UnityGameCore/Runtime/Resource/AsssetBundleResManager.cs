using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Air.UnityGameCore.Runtime.Resource
{
    /// <summary>
    /// AssetBundle 资源管理器
    /// </summary>
    public class AsssetBundleResManager : ResManager
    {
        // Bundle 加载器
        private readonly AssetBundleLoader _bundleLoader;
        
        // AssetBundle 路径解析函数
        private readonly Func<string, string> _bundlePathResolver;

        public AsssetBundleResManager(
            string bundleRootPath,
            Func<string, string> bundlePathResolver = null,
            Func<string, string[]> dependenciesResolver = null)
        {
            _bundlePathResolver = bundlePathResolver ?? DefaultBundlePathResolver;
            _bundleLoader = new AssetBundleLoader(bundleRootPath, dependenciesResolver);
        }

        private string DefaultBundlePathResolver(string assetPath)
        {
            // 默认实现：可以根据实际项目需求修改
            return assetPath.ToLower();
        }

        protected override void LoadAssetAsync<T>(string path, ResLoadInfo<T> loadInfo, ELoadType loadType)
        {
            // 解析 Bundle 路径
            var bundlePath = _bundlePathResolver(path);
            
            // 先加载 AssetBundle
            _bundleLoader.LoadBundleAsync(bundlePath, bundle =>
            {
                if (bundle == null)
                {
                    Debug.LogError($"Failed to load AssetBundle: {bundlePath} for asset: {path}");
                    OnAssetLoadCompleted(path, loadInfo, null, loadType);
                    return;
                }

                // 保存 BundlePath 用于后续卸载
                loadInfo.BundlePath = bundlePath;

                // 从 AssetBundle 加载资源
                var request = bundle.LoadAssetAsync<T>(path);
                request.completed += _ =>
                {
                    var asset = request.asset as T;
                    OnAssetLoadCompleted(path, loadInfo, asset, loadType);
                };
            });
        }

        protected override void OnUnloadAsset(IResLoadInfo loadInfo)
        {
            // 卸载资源
            if (loadInfo is ResLoadInfo<Object> typedLoadInfo)
            {
                typedLoadInfo.Asset = null;
                
                // 使用 BundleLoader 卸载对应的 AssetBundle
                if (!string.IsNullOrEmpty(typedLoadInfo.BundlePath))
                {
                    _bundleLoader.UnloadBundle(typedLoadInfo.BundlePath);
                }
            }
        }

        /// <summary>
        /// 卸载所有未使用的 AssetBundle
        /// </summary>
        public void UnloadUnusedBundles()
        {
            _bundleLoader.UnloadUnusedBundles();
        }

        /// <summary>
        /// 清空所有资源和 AssetBundle
        /// </summary>
        public void Clear()
        {
            _loadInfoDict.Clear();
            _loadCallback.Clear();
            _loadInstCallback.Clear();
            _bundleLoader.Clear();
        }
    }
}