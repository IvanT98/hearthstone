using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetManager : MonoBehaviour
{
    private readonly List<Sprite> _randomImages = new();
    private const string HostUrl = "https://picsum.photos";
    private const int ImageWidth = 100;
    private const int ImageHeight = 100;
    private const int MaxImages = 50;

    public Sprite GetRandomImage()
    {
        return _randomImages.Count == 0 ? null : _randomImages[Utilities.GetRandomListIndex(_randomImages.Count)];
    }

    public bool WereImagesFetched()
    {
        return _randomImages.Count == MaxImages;
    }

    private void GetImages()
    {
        var fullUrl = $"{HostUrl}/{ImageWidth}/{ImageHeight}";
        
        for (var i = 0; i < MaxImages; i++)
        {
            StartCoroutine(GetImage(fullUrl));
        }
    }

    private IEnumerator GetImage(String url)
    {
        using var request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new Exception($"An error has occured while trying to fetch an image from the specified host. The error was '{request.error}'");
        }

        var texture = DownloadHandlerTexture.GetContent(request);
        var sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        
        _randomImages.Add(sprite);
    }
    
    private void Start()
    {
        GetImages();
    }
}
