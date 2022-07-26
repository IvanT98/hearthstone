using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Responsible for retrieving assets required for the game.
///
/// Currently, only fetches images from the remote host.
/// </summary>
public class AssetManager : MonoBehaviour
{
    private readonly List<Sprite> _randomImages = new();
    private const string HostUrl = "https://picsum.photos";
    private const int ImageWidth = 100;
    private const int ImageHeight = 100;
    private const int MaxImages = 50;

    /// <summary>
    ///  Picks a random image from the fetched images and returns it.
    /// </summary>
    /// <returns>A random fetched image.</returns>
    public Sprite GetRandomImage()
    {
        return _randomImages.Count == 0 ? null : _randomImages[Utilities.GetRandomListIndex(_randomImages.Count)];
    }
    
    /// <summary>
    /// Checks if the maximum number of images were fetched from the remote host.
    /// </summary>
    /// <returns>true if all images were fetched, false otherwise.</returns>
    public bool WereImagesFetched()
    {
        return _randomImages.Count == MaxImages;
    }

    /// <summary>
    /// Builds the full remote host url and starts fetching the images.
    /// </summary>
    private void GetImages()
    {
        var fullUrl = $"{HostUrl}/{ImageWidth}/{ImageHeight}";
        
        for (var i = 0; i < MaxImages; i++)
        {
            StartCoroutine(GetImage(fullUrl));
        }
    }

    /// <summary>
    /// Fetches an image from the remote host.
    /// </summary>
    /// <param name="url">The full url of the remote host.</param>
    /// <returns>Asynchronous operation for fetching the image.</returns>
    /// <exception cref="Exception">If the remote host does not return an image, an exception is raised to indicate the issue.</exception>
    private IEnumerator GetImage(string url)
    {
        using var request = UnityWebRequestTexture.GetTexture(url);
        
        // Wait for the web request to resolve
        yield return request.SendWebRequest();

        // If the request response is not a success, raise an exception.
        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new Exception($"An error has occured while trying to fetch an image from the specified host. The error was '{request.error}'");
        }

        // Create a sprite based on the fetched image.
        var texture = DownloadHandlerTexture.GetContent(request);
        var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        
        _randomImages.Add(sprite);
    }
    
    /// <summary>
    /// Starts the image fetching.
    /// </summary>
    private void Start()
    {
        GetImages();
    }
}
