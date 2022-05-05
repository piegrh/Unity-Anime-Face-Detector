using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Ulbe.Utils
{
    public class ImageUtils : MonoBehaviour
    {
        public static void SaveImageToPersistentDataPath(byte[] image, string fileName)
        {
            string path = $"{Application.persistentDataPath}/{fileName}";
            File.WriteAllBytes(path, image);
        }

        public static Texture2D FlipTexture(Texture2D original)
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
    }
}