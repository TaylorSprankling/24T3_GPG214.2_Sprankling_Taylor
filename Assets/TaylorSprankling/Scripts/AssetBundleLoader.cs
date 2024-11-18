using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
    private string folderPath = Application.streamingAssetsPath;
    [SerializeField] private string assetBundleFileName;
    private string filePath;

    [SerializeField] private string assetName;
    private GameObject assetInstance;

    [SerializeField] private KeyCode loadKey = KeyCode.None;

    private void Awake()
    {
        filePath = Path.Combine(folderPath, assetBundleFileName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(loadKey))
        {
            StartCoroutine(LoadGameObject());
        }
    }

    private IEnumerator LoadGameObject()
    {
        if (string.IsNullOrEmpty(assetName) || string.IsNullOrEmpty(assetBundleFileName) || !File.Exists(filePath))
        {
            Debug.Log("Asset file name or path was not provided or doesn't exist");
            yield break;
        }

        if (assetInstance != null)
        {
            Debug.Log($"{assetName} asset has already been loaded");
            yield break;
        }

        AssetBundleCreateRequest bundleRequest = null;
        if (File.Exists(filePath))
        {
            yield return bundleRequest = AssetBundle.LoadFromFileAsync(filePath);
        }
        else
        {
            Debug.Log($"{filePath} doesn't exist or isn't found");
        }

        var loadedAssetBundle = bundleRequest.assetBundle;
        if (loadedAssetBundle == null)
        {
            Debug.Log($"Failed to load {assetBundleFileName}");
            yield break;
        }

        var assetLoadRequest = loadedAssetBundle.LoadAssetAsync<GameObject>(assetName);
        yield return assetLoadRequest;

        var gameObjectPrefab = assetLoadRequest.asset as GameObject;
        if (gameObjectPrefab != null)
        {
            assetInstance = Instantiate(gameObjectPrefab, gameObject.transform);
            Debug.Log($"{assetName} loaded from asset bundle: {filePath}");
        }

        loadedAssetBundle.Unload(false);
    }
}
