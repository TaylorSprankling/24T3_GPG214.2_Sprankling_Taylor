using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAssetBundleGameObject : MonoBehaviour
{
    private string folderPath = Application.streamingAssetsPath;
    [SerializeField] private string assetBundleFileName;
    private string filePath;

    private AssetBundle assetBundle;
    [SerializeField] private string gameObjectName;
    private GameObject gameObjectInstance;

    [SerializeField] private KeyCode loadKey = KeyCode.None;

    private void Awake()
    {
        filePath = Path.Combine(folderPath, assetBundleFileName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(loadKey))
        {
            LoadGameObject();
        }
    }

    private void LoadGameObject()
    {
        if (gameObjectInstance != null)
        {
            return;
        }

        if (File.Exists(filePath))
        {
            assetBundle = AssetBundle.LoadFromFile(filePath);
        }
        else
        {
            Debug.Log($"{filePath} doesn't exist or isn't found");
        }

        if (assetBundle == null)
        {
            return;
        }

        var gameObjectPrefab = assetBundle.LoadAsset<GameObject>(gameObjectName);

        if (gameObjectPrefab != null)
        {
            gameObjectInstance = Instantiate(gameObjectPrefab, gameObject.transform);
            Debug.Log($"{gameObjectName} loaded from asset bundle: {filePath}");
        }

        assetBundle.Unload(false);
    }
}
