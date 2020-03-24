namespace TPsMap
{
	using System;
	using System.Collections.Generic;
    using System.IO;
    using System.Text;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public class Marker : MonoBehaviour
	{
		private static Sprite playerSprite = null;
		private static Sprite markerSprite = null;
		private static Sprite upSprite = null;
		private static Sprite downSprite = null;

		private void Awake()
		{
			this.rectTranform = base.GetComponent<RectTransform>();

		}

		private void Update()
		{

		}

		private RectTransform rectTranform;

		public static Sprite PlayerSprite { get
			{
				if (playerSprite == null)
				{
					playerSprite = Extensions.CreateSpriteFromFile("player.png");
				}
				return playerSprite;
			}
		}

		public static Sprite MarkerSprite
		{
			get
			{
				if (markerSprite == null)
				{
					markerSprite = Extensions.CreateSpriteFromFile("marker.png");
				}
				return markerSprite;
			}
		}

		public static Sprite DownSprite
		{
			get
			{
				if (downSprite == null)
				{
					downSprite = Extensions.CreateSpriteFromFile("marker_down.png");
				}
				return downSprite;
			}
		}
		public static Sprite UpSprite
		{
			get
			{
				if (upSprite == null)
				{
					upSprite = Extensions.CreateSpriteFromFile("marker_up.png");
				}
				return upSprite;
			}
		}
	}
}

