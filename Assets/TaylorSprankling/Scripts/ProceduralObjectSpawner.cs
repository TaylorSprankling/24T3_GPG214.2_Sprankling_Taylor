using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralObjectSpawner : MonoBehaviour
{
    [SerializeField] private Texture2D proceduralTexture;

    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private float objectAnchorHeight;

    [SerializeField] private float raycastHeight;
    [SerializeField] private float spacing;
    [SerializeField] private float requiredChannelStrength;
    [SerializeField] private float pixelsPerObject;

    private void Start()
    {
        SpawnObjectFromImage();
    }

    private void SpawnObjectFromImage()
    {
        if (proceduralTexture == null)
        {
            Debug.LogError("No texture referenced");
            return;
        }

        bool[,] occupiedPixels = new bool[proceduralTexture.width, proceduralTexture.height];

        int objectCount = 0;

        for (float y = 0; y < proceduralTexture.height; y += pixelsPerObject)
        {
            for (float x = 0; x < proceduralTexture.width; x += pixelsPerObject)
            {
                // Check to see if I can spawn the object
                if (CanSpawnObject(proceduralTexture, Mathf.FloorToInt(x), Mathf.FloorToInt(y), occupiedPixels))
                {
                    MarkOccupied(Mathf.FloorToInt(x), Mathf.FloorToInt(y), occupiedPixels);

                    Vector3 spawnPosition = new Vector3(x * spacing, raycastHeight, y * spacing) + transform.position;

                    if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, raycastHeight * 2))
                    {
                        spawnPosition.y = hit.point.y + objectAnchorHeight;
                        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    }

                    objectCount++;
                }
            }
        }

        Debug.Log($"Objects spawned in: {objectCount}");
    }

    // Check if there's enough surrounding space to spawn the object
    private bool CanSpawnObject(Texture2D image, int startX, int startY, bool[,] occupied)
    {
        int redPixelCount = 0;

        for (int y = 0; y < Mathf.CeilToInt(pixelsPerObject); y++)
        {
            for (int x = 0; x < Mathf.CeilToInt(pixelsPerObject); x++)
            {
                int pixelX = startX + x;
                int pixelY = startY + y;

                if (pixelX >= image.width || pixelY >= image.height)
                {
                    continue;
                }

                if (occupied[pixelX, pixelY])
                {
                    return false;
                }
                
                Color pixelColour = image.GetPixel(pixelX, pixelY);
                if (pixelColour.r > requiredChannelStrength)
                {
                    redPixelCount++;
                }
            }
        }
        return redPixelCount >= pixelsPerObject;
    }

    // Set the location provided and esure neighbours are set to occupied
    private void MarkOccupied(int startX, int startY, bool[,] occupied)
    {
        for (int y = 0; y < Mathf.CeilToInt(pixelsPerObject); y++)
        {
            for (int x = 0; x < Mathf.CeilToInt(pixelsPerObject); x++)
            {
                int pixelX = startX + x;
                int pixelY = startY + y;

                if (pixelX >= occupied.GetLength(0) || pixelY >= occupied.GetLength(1))
                {
                    continue;
                }

                occupied[pixelX, pixelY] = true;
            }
        }
    }
}