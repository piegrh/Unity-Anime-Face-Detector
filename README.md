# Unity Anime Face Detector
Detect anime faces in texture and images in Unity.  
This Library is basically a port of [lbpcascade_animeface by nagadomi](https://github.com/nagadomi/lbpcascade_animeface) to C# using [Emgu CV](https://www.emgu.com/wiki/index.php/Main_Page) (an OpenCV wrapper).  Also some unity integration stuff.
  
 ![alt text](https://i.imgur.com/DSVgVOo.gif)  
## Example together with  [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET)
Quit the application if we detect an anime face in this user's steam profile image.
```c#
public class ScanSteamProfileImage : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(CheckSteamUserProfileAvatar());
    }

    public IEnumerator CheckSteamUserProfileAvatar()
    {
        // Download profile image
        int imageId = 0;
        yield return new WaitWhile(() =>
        {
            imageId = SteamFriends.GetLargeFriendAvatar(SteamUser.GetSteamID());
            return imageId == -1;
        }); 
        
        // Check image
        Texture2D texture = GetSteamImageAsTexture(imageId);
        AnimeFaceDetector.Instance.IsAnimeImage(texture, (profileImageHasAnimeFace) => {
          if (profileImageHasAnimeFace)
                Application.Quit(); // Bye bye!
        });
    }

    public static Texture2D GetSteamImageAsTexture(int imgId)
    {
        bool valid = SteamUtils.GetImageSize(imgId, out var width, out var height);

        if (valid == false)
            return new Texture2D(1, 1, TextureFormat.ARGB32, false);

        int size = (int)(width * height * 4);
        byte[] image = new byte[size];

        if (SteamUtils.GetImageRGBA(imgId, image, size) == false)
            return new Texture2D(1, 1, TextureFormat.ARGB32, false);

        Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
        texture.LoadRawTextureData(image);
        texture = texture.FlipY(); // SteamWorks.NET gives you an upside down image.
        texture.Apply();
        
        return texture;
    }
}
```
![alt text](https://i.imgur.com/McMJsyR.jpg)  
