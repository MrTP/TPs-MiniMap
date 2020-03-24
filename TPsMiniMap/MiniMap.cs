namespace TPsMap
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEngine;
	using UnityEngine.UI;
	using BepInEx;
	using RoR2;
	using System.Reflection;
    using System.Linq;

    public class MiniMap : MonoBehaviour
	{
		private static Sprite sprite = null;

		private float width = 400;
		private float height = 400;
		private float rotation = 0;


		private void Awake()
		{
			if (typesToCheck == null)
			{
				typesToCheck = (from t in typeof(ChestRevealer).Assembly.GetTypes()
										where typeof(IInteractable).IsAssignableFrom(t)
										select t).ToArray<Type>();
			}

			RoR2.SceneCamera.onSceneCameraPreRender += SceneCamera_onSceneCameraPreRender;
			this.rectTranform = base.GetComponent<RectTransform>();
			this.markerPrefab = new GameObject("Marker");
			markerPrefab.AddComponent<Marker>();
			markerPrefab.AddComponent<Image>();
			
			markerPrefab.AddComponent<RectTransform>();
			markerPrefab.gameObject.SetPosition(0.48f, 0.48f, 0.04f, 0.04f);
			markerPrefab.GetComponent<Image>().sprite = Sprite.Create(new Texture2D(0, 0), new Rect(), Vector2.zero);

			this.north = UnityEngine.Object.Instantiate<GameObject>(this.markerPrefab, this.rectTranform);
			north.gameObject.SetPosition(0.46f, 0.90f, 0.08f, 0.08f);
			north.GetComponent<Image>().sprite = Extensions.CreateSpriteFromFile("north.png");
		}

		private void AllocateMarkers()
		{
			foreach (Marker marker in markers)
			{
				Destroy(marker.gameObject);
			}
			markers.Clear();
			
			if (PlayerCharacterMasterController.instances.Count > 0)
			{
				CharacterBody body = LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetCurrentBody();
				center = body.transform.position;
				
				Marker newMarker = UnityEngine.Object.Instantiate<GameObject>(this.markerPrefab, this.rectTranform).GetComponent<Marker>();
				newMarker.gameObject.SetPosition(0.465f, 0.465f, 0.07f, 0.07f);
				newMarker.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
				this.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotation);
				newMarker.GetComponent<Image>().sprite = Marker.PlayerSprite;
				newMarker.GetComponent<Image>().color = new Color(50 / 256f, 240 / 256f, 30 / 256f);
				markers.Add(newMarker);
			}


			foreach (PlayerCharacterMasterController other in PlayerCharacterMasterController.instances)
			{
				if (other.networkUser != LocalUserManager.GetFirstLocalUser().currentNetworkUser)
				{
					AddMarker(other.master.GetBodyObject().transform, 2, new Color(30 / 256f, 160 / 256f, 13 / 256f));
				}
			}

			for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
			{
				try
				{
					CharacterMaster master = CharacterMaster.readOnlyInstancesList[i];
					if (master == null || master.GetBody() == null)
					{
						continue;
					}
					CharacterBody body = master.GetBody();
					if (!body.isPlayerControlled)
					{
						Color color = Color.red;
						float size = 1.5f;
						if (body.isElite)
						{
							color = Color.blue;
							
						}
						if (body.isBoss)
						{
							size = 3;
						}
						AddMarker(body.transform, size, color);
					}
				}
				catch
				{
				}
			}

			Type[] array = MiniMap.typesToCheck;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MonoBehaviour monoBehaviour in InstanceTracker.FindInstancesEnumerable(array[i]))
				{
					if (((IInteractable)monoBehaviour).ShouldShowOnScanner())
					{
						AddMarker(monoBehaviour.transform, 2, Color.yellow);
					}
				}
			}
		}

		private void SceneCamera_onSceneCameraPreRender(SceneCamera sceneCamera)
		{
			rotation = sceneCamera.camera.transform.rotation.eulerAngles.y + 180f;
		}

		private void UpdateLayout()
		{
			this.AllocateMarkers();
		}


		private void Update()
		{
			UpdateLayout();
			steps++;
			if (steps > 3)
			{
				steps = 0;
				

			}
		}

		private void AddMarker(Transform transform, float size, Color color)
		{
			Vector2 local = ToLocal(center, transform.position);
			if ((local.x - 0.5f) * (local.x - 0.5f) + (local.y - 0.5f) * (local.y - 0.5f) < 0.2f)
			{
				Marker newMarker = UnityEngine.Object.Instantiate<GameObject>(this.markerPrefab, this.rectTranform).GetComponent<Marker>();

				float sizeFactor = size / 100f;

				newMarker.gameObject.SetPosition(local.x - sizeFactor, local.y - sizeFactor, sizeFactor * 2f, sizeFactor * 2f);
				newMarker.gameObject.GetComponent<Image>().color = color;
				float y = transform.position.y - center.y;
				float tolerance = 20;
				if (y > tolerance)
				{
					newMarker.gameObject.GetComponent<Image>().sprite = Marker.UpSprite;
				}
				else if(y < -tolerance)
				{
					newMarker.gameObject.GetComponent<Image>().sprite = Marker.DownSprite;
				}
				else
				{
					newMarker.gameObject.GetComponent<Image>().sprite = Marker.MarkerSprite;
				}
				newMarker.transform.rotation = Quaternion.Euler(0, 0, 0);
				markers.Add(newMarker);
			}
		}

		private RectTransform rectTranform;

		public GameObject markerPrefab;

		[HideInInspector]
		[SerializeField]
		private List<Marker> markers = new List<Marker>();

		private int steps = 0;
		private static Type[] typesToCheck = null;
		private Vector3 center = Vector3.zero;
		private GameObject north;

		public static Sprite Sprite
		{
			get
			{
				if (sprite == null)
				{
					sprite = Extensions.CreateSpriteFromFile("map.png");
				}
				return sprite;
			}
			set
			{
				sprite = value;
			}
		}

		private Vector2 Rotate(Vector2 vector, float angle)
		{
			angle += 180f;
			angle = angle / 180f * Mathf.PI;
			Vector2 output = Vector2.zero;
			output.x = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
			output.y = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
			return output;
		}

		private Vector2 ToLocal(Vector3 center, Vector3 position)
		{
			Vector3 offset = center - position;
			float x = offset.x + width / 2f;
			float y = offset.z + height / 2f;
			return new Vector2(x / width, y / height);
		}


	}
}

