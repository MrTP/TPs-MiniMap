using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace TPsMap
{
    public static class Extensions
    {
        public static void SetPosition(this GameObject gameObject, float x, float y, float dx, float dy)
        {
            
            gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(x, y);
            gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(x + dx, y + dy);
            gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            
        }

        public static Vector2 rotate(this Vector2 v, float delta)
        {
            return new Vector2(
                v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
                v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
        }

        public static Sprite CreateSpriteFromFile(string name)
        {
            Texture2D tex = new Texture2D(1, 1);

            tex.LoadImage(File.ReadAllBytes($"{Path.GetDirectoryName(typeof(TPsMap).Assembly.Location)}/textures/" + name));
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));

        }

    }
}
