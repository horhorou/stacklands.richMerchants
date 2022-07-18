using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Sokpop;



namespace RichMod
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {

        public const string pluginGuid = "horhorou.stacklands.richMerchants";
        public const string pluginName = "Rich Merchants";
        public const string pluginVersion = "1.0.0.0";




        public void Awake()
        {
            InitializeMod();
        }


        private void InitializeMod()
        {
            Harmony harmony = new Harmony(pluginGuid);
            MethodInfo original = AccessTools.Method(typeof(Market), "SellWithMarket");
            MethodInfo patched = AccessTools.Method(typeof(Main), "SellWithMarket_Patched");

            harmony.Patch(original, new HarmonyMethod(patched));
        }


        public static bool SellWithMarket_Patched(Market __instance)
        {
            GameCard child = __instance.MyGameCard.Child;

            GameCard gameCard = null;
            if (child.Child != null && WorldManager.instance.CardCanBeSold(child.Child, true, false))
            {
                gameCard = child.Child;
            }
            child.RemoveFromStack();
            if (gameCard != null)
            {
                __instance.MyGameCard.Child = gameCard;
                gameCard.Parent = __instance.MyGameCard;
            }
            QuestManager.instance.SpecialActionComplete("sell_at_market", __instance);
            GameCard gameCard2 = WorldManager.instance.SellCard(__instance.transform.position, child, 5f, false);
            WorldManager.instance.StackSend(gameCard2.GetRootCard(), __instance.MyGameCard);
            return false;
        }

    }
}
