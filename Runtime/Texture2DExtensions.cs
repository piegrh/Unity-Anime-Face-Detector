using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ulbe.Extensions
{
    public static class Texture2DExtensions
    {
        public static Texture2D DeCompress(this Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear
            );
            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        /// <summary>
        /// Flips y axis.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Texture2D FlipY(this Texture2D original)
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
}
