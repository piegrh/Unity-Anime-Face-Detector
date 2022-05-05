# Unity Anime face detector
Detect anime faces in texture and images in Unity.  
This Library is basically a port of [lbpcascade_animeface by nagadomi](https://github.com/nagadomi/lbpcascade_animeface) to C# using [Emgu CV](https://www.emgu.com/wiki/index.php/Main_Page) (an OpenCV wrapper).  Also some unity integration stuff.
  
 ![alt text](https://i.imgur.com/DSVgVOo.gif)  
## Example together with Steamworks.NET
Quit the application if we detect an anime face in this user's steam profile image.

```c#
public class ScanSteamProfileImage : MonoBehaviour
{
    public Texture2D Texture;

    public void Start()
    {
        StartCoroutine(CheckUserProfile());
    }

    public IEnumerator CheckUserProfile()
    {
        int imageId = 0;
        
        // Download profile image
        yield return new WaitWhile(() =>
        {
            imageId = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
            return imageId == -1;
        }); 
        
        Texture = GetSteamImageAsTexture(imageId);

        // #############################################
        // ## Scan image for anime faces.             ##
        // #############################################
        AnimeFaceDetector.Instance.IsAnimeImage(Texture, (profileImageHasAnimeFace) => {
          if (profileImageHasAnimeFace)
                Application.Quit(); // Bye bye!
        });
    }

    public Texture2D GetSteamImageAsTexture(int imgId)
    {
        bool valid = SteamUtils.GetImageSize(imgId, out var width, out var height);

        if (valid == false)
            return new Texture2D(1, 1, TextureFormat.ARGB32, false);

        int pixels = (int)(width * height * 4);
        byte[] image = new byte[pixels];

        if (SteamUtils.GetImageRGBA(imgId, image, pixels) == false)
            return new Texture2D(1, 1, TextureFormat.ARGB32, false);

        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
        texture.LoadRawTextureData(image);
        texture = FlipTexture(texture); // SteamWorks.NET gives you an upside down image.
        texture.Apply();
        
        return texture;
    }

    static Texture2D FlipTexture(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);
        int xN = original.width;
        int yN = original.height;
        for (int i = 0; i < xN; i++)
          for (int j = 0; j < yN; j++)
            flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
        return flipped;
    }
}
```
