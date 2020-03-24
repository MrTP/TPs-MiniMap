using BepInEx;
using RoR2;
using System.IO;
using TPsMap;
using UnityEngine;
using UnityEngine.UI;

namespace TPsMap
{
    //This is an example plugin that can be put in BepInEx/plugins/ExamplePlugin/ExamplePlugin.dll to test out.
    //It's a very simple plugin that adds Bandit to the game, and gives you a tier 3 item whenever you press F2.
    //Lets examine what each line of code is for:

    //This attribute specifies that we have a dependency on R2API, as we're using it to add Bandit to the game.
    //You don't need this if you're not using R2API in your plugin, it's just to tell BepInEx to initialize R2API before this plugin so it's safe to use R2API.
    [BepInDependency("com.bepis.r2api")]

    //This attribute is required, and lists metadata for your plugin.
    //The GUID should be a unique ID for this plugin, which is human readable (as it is used in places like the config). I like to use the java package notation, which is "com.[your name here].[your plugin name here]"
    //The name is the name of the plugin that's displayed on load, and the version number just specifies what version the plugin is.
    [BepInPlugin("com.tp.tpsmap", "TP's Map", "1.0")]

    //This is the main declaration of our plugin class. BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    //BaseUnityPlugin itself inherits from MonoBehaviour, so you can use this as a reference for what you can declare and use in your plugin class: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class TPsMap : BaseUnityPlugin
    {
        private GameObject HUDroot = null;

        private GameObject miniMap { get; set; }

        //The Awake() method is run at the very start when the game is initialized.
        public void Awake()
        {
            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);

            HUDroot = self.transform.root.gameObject;
            miniMap = new GameObject("MiniMap");
            miniMap.transform.SetParent(HUDroot.transform);
            miniMap.AddComponent<RectTransform>();
            miniMap.AddComponent<Image>();
            miniMap.GetComponent<Image>().sprite = MiniMap.Sprite;
            
            float factor = Screen.width / (float)Screen.height;

            float height = 0.15f;
            float width = height / factor;
            miniMap.GetComponent<RectTransform>().anchorMin = new Vector2(0.85f - width, 0.1f);
            miniMap.GetComponent<RectTransform>().anchorMax = new Vector2(0.85f + width , 0.1f + height * 2f);
            miniMap.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            miniMap.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            
            miniMap.AddComponent<MiniMap>();
        }

        //The Update() method is run on every frame of the game.
        public void Update()
        {
            
        }




    }
}