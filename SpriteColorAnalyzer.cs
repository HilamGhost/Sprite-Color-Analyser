/*
```
⠀⠀⠀⠀⠀⢀⡠⠔⠂⠉⠉⠉⠉⠐⠦⡀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⢀⠔⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡄⠀⠀⠀⠀⠀
⠀⠀⢠⠋⠀⠀⠀⠀⠖⠉⢳⠀⠀⢀⠔⢢⠸⠀⠀⠀⠀⠀
⠀⢠⠃⠀⠀⠀⠀⢸⠀⢀⠎⠀⠀⢸⠀⡸⠀⡇⠀⠀⠀⠀
⠀⡜⠀⠀⠀⠀⠀⠀⠉⠁⠾⠭⠕⠀⠉⠀⢸⠀⢠⢼⣱⠀
⠀⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡌⠀⠈⠉⠁⠀   hilam was here.
⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡇⠀⠀⣖⡏⡇  
⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢄⠀⠀⠈⠀
⢸⠀⢣⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⡬⠇⠀⠀⠀
⠀⡄⠘⠒⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢣⠀⠀⠀⠀
⠀⢇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡀⠀⠀⠀
⠀⠘⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡤⠁⠀⠀⠀
⠀⠀⠘⠦⣀⠀⢀⡠⣆⣀⣠⠼⢀⡀⠴⠄⠚⠀⠀⠀⠀⠀
```
*/


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HilamPrototypes
{
    public static class SpriteColorAnalyzer
    {
        private static float _colorTolerance = 0.05f;

        /// <summary>
        /// Checks the wanted color is in the sprite. 
        /// </summary>
        /// <param name="sprite"> The sprite you want to check.</param>
        /// <param name="wantedColor"> The color you want the check in the sprite. </param>
        /// <param name="dominanceLimit"> The minimum percentage of the color occupies. </param>
        /// <returns></returns>
        public static bool IsColorDominant(Sprite sprite, Color wantedColor, float dominanceLimit)
        {
            var colorsIncluded = AnalyzeSpriteColor(sprite);
            var colorComparer = new ColorComparer(_colorTolerance);
            
            foreach (var color in colorsIncluded)
            {
                if (colorComparer.Equals(color.Key, wantedColor) && color.Value > dominanceLimit)
                {
                    Debug.Log($"{color.Key} and {color.Value}%");
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Checks all colors of the sprite and gives you the percentage each color occupies.
        /// </summary>
        /// <param name="sprite"> The sprite you want to check.</param>
        /// <returns></returns>
        public static Dictionary<Color, float> AnalyzeSpriteColor(Sprite sprite)
        {
            Texture2D tex = MakeTextureReadable(sprite.texture);
            Rect rect = sprite.textureRect;

            int x = (int)rect.x;
            int y = (int)rect.y;
            int width = (int)rect.width;
            int height = (int)rect.height;

            Color[] pixels = tex.GetPixels(x, y, width, height);

            Dictionary<Color, int> colorCounts = new Dictionary<Color, int>(new ColorComparer(_colorTolerance));

            foreach (Color c in pixels)
            {
                if (c.a < 0.1f) continue;

                Color opaqueColor = new Color(c.r, c.g, c.b, 1f);

                if (colorCounts.ContainsKey(opaqueColor))
                    colorCounts[opaqueColor]++;
                else
                    colorCounts[opaqueColor] = 1;
            }

            int total = colorCounts.Values.Sum();

            Dictionary<Color, float> ColorIncluded = new();
            foreach (var pair in colorCounts.OrderByDescending(p => p.Value))
            {
                float percent = (pair.Value / (float)total) * 100f;
                ColorIncluded.Add(pair.Key, percent);
            }

            return ColorIncluded;
        }

        private static string ColorToName(Color c)
        {
            if (c == Color.black) return "black";
            if (c == Color.white) return "white";
            if (c == Color.red) return "red";
            if (c == Color.green) return "green";
            if (c == Color.blue) return "blue";
            return $"R:{c.r:F2} G:{c.g:F2} B:{c.b:F2}";
        }
        
        /// <summary>
        /// Making the sprite’s texture readable
        /// </summary>
        private static Texture2D MakeTextureReadable(Texture2D source)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(source, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D readableTex = new Texture2D(source.width, source.height);
            readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableTex.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return readableTex;
        }
    }
    class ColorComparer : IEqualityComparer<Color>
    {
        private float tolerance;

        public ColorComparer(float tolerance)
        {
            this.tolerance = tolerance;
        }

        public bool Equals(Color x, Color y)
        {
            return Mathf.Abs(x.r - y.r) < tolerance &&
                   Mathf.Abs(x.g - y.g) < tolerance &&
                   Mathf.Abs(x.b - y.b) < tolerance;
        }

        public int GetHashCode(Color obj)
        {
            int r = Mathf.RoundToInt(obj.r / tolerance);
            int g = Mathf.RoundToInt(obj.g / tolerance);
            int b = Mathf.RoundToInt(obj.b / tolerance);
            return r * 1000000 + g * 1000 + b;
        }
    }
}