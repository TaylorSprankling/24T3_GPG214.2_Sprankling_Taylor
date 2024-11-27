using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{
    [SerializeField] private bool downloadOnStart = true;
    [SerializeField] private bool downloadOnKeypress = false;
    [SerializeField] private KeyCode downloadKey = KeyCode.None;
    [SerializeField] private List<MetadataFile> filesToDownload = new List<MetadataFile>();

    private void Start()
    {
        if (downloadOnStart)
        {
            StartCoroutine(CheckAndDownloadFiles());
        }
    }

    private void Update()
    {
        if (downloadOnKeypress && Input.GetKeyDown(downloadKey))
        {
            StartCoroutine(CheckAndDownloadFiles());
        }
    }

    private IEnumerator CheckAndDownloadFiles()
    {
        foreach (MetadataFile metadata in filesToDownload)
        {
            // Get the meta data version
            metadata.CheckLocalVersion();

            // Create folder if path currently doesn't exist
            if (!Directory.Exists(metadata.LocalFolderPath))
            {
                Directory.CreateDirectory(metadata.LocalFolderPath);
                Debug.Log($"Creating folder path: {metadata.LocalFolderPath}");
            }

            // Download new meta data
            yield return StartCoroutine(DownloadFile(metadata.DirectMetadataDownloadLink, metadata.LocalMetadataFilePath));
            yield return new WaitForEndOfFrame();

            // Check if file needs updating
            if (metadata.FileNeedsUpdating() || !File.Exists(metadata.LocalFilePath))
            {
                // Delete the local files
                metadata.DeleteLocalFile();

                // Download new files
                if (!string.IsNullOrEmpty(metadata.LocalMetadataFilePath) && !string.IsNullOrEmpty(metadata.DirectMetadataDownloadLink))
                {
                    Debug.Log($"Starting download for {metadata.fileName}{metadata.associatedFileExtension}");
                    yield return StartCoroutine(DownloadFile(metadata.RemoteFileDownloadLink, metadata.LocalFilePath));
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    Debug.LogError("Failed to obtain a valid download link, or local metadata path is not valid.");
                }
            }

            yield return null;
        }
    }

    private IEnumerator DownloadFile(string fileLink, string savePath)
    {
        if (string.IsNullOrEmpty(fileLink))
        {
            Debug.LogError("Invalid file link");
            yield break;
        }

        UnityWebRequest request = UnityWebRequest.Get(fileLink); // Create the request for the download
        yield return request.SendWebRequest(); // Start downloading and wait until completed

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to download file: {request.error}");
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)); // Check directory exists
            File.WriteAllBytes(savePath, request.downloadHandler.data); // Write files to destination
            Debug.Log($"File downloaded successfully to: {savePath}");
        }
    }
}
