# Animefacedetector

## Example together with Steamworks SDK

```c#
    public Texture2D Texture;

    public void Start()
    {
        StartCoroutine(CheckUserProfile());
    }

    public IEnumerator CheckUserProfile()
    {
        int imageId = 0;

        yield return new WaitWhile(() =>
        {
            imageId = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
            return imageId == 0 || imageId == -1;
        });

        Texture = GetSteamImageAsTexture(imageId);
        AnimeFaceDetector.Instance.IsAnimeImage(Texture, Callback);
    }

    public Texture2D GetSteamImageAsTexture(int imgId)
    {
        bool valid = SteamUtils.GetImageSize(imgId, out var width, out var height);

        if (valid == false)
            return null;

        int pixels = (int)(width * height * 4);
        byte[] image = new byte[pixels];

        if (SteamUtils.GetImageRGBA(imgId, image, pixels) == false)
            return null;

        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
        texture.LoadRawTextureData(image);
        texture.Apply();
        texture = FlipTexture(texture);

        return texture;
    }

    public void Callback(bool value)
    {
        if (value)
            Application.Quit();
    }

    static Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
        int xN = original.width;
        int yN = original.height;
        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
            }
        }
        flipped.Apply();
        return flipped;
    }
```
