using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssetManager : MonoBehaviour
{
    public List<Sprite> _randomImages;
    private string _hostUrl = "https://picsum.photos";
    private int _imageWidth = 100;
    private int _imageHeight = 100;
    private int _maxImages = 50;

    public Sprite GetRandomImage()
    {
        return _randomImages[Utilities.GetRandomListIndex(_randomImages.Count)];
    }

    // Start is called before the first frame update
    private void Start()
    {
        GetImages();
    }

    public bool WereImagesFetched()
    {
        return _randomImages.Count == _maxImages;
    }

    private void GetImages()
    {
        string fullUrl = $"{_hostUrl}/{_imageWidth}/{_imageHeight}";
        
        for (int i = 0; i < _maxImages; i++)
        {
            StartCoroutine(GetImage(fullUrl));
        }
    }

    private IEnumerator GetImage(String url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"An error has occured while trying to fetch an image from the specified host. The error was '{request.error}'");
            }

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        
            _randomImages.Add(sprite);
        }
    }
}
