using BepInEx;
using System.Linq;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using EFT.UI.Matchmaker;
using TMPro;
using EFT;
using UnityEngine.UI;
using UnityEngine;

namespace OneTimeZone
{
	[BepInPlugin("dev.birgerev.OneTimeZone", "OneTimeZone", "1.1.0")]
	public class OneTimeZonePlugin : BaseUnityPlugin
	{
		public void Awake()
        {
            new LocationTimezonePatch().Enable();
        }
	}

    public class LocationTimezonePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LocationConditionsPanel).GetMethod("Set");
        }

        [PatchPostfix]
        public static void Postfix(ref LocationConditionsPanel __instance, ref RaidSettings raidSettings, ref TextMeshProUGUI ____nextPhaseTime, ref Toggle ____amTimeToggle, ref Toggle ____pmTimeToggle)
        {
            if (raidSettings == null) return;
            if (____nextPhaseTime == null) return;
            if (____amTimeToggle == null) return;
            if (____pmTimeToggle == null) return;

            //Only show dialouge on factory since time is static there
            bool allowSelectTimezone = raidSettings.LocationId == "factory4_day" || raidSettings.LocationId == "factory4_night";
            
            ____nextPhaseTime.alpha = allowSelectTimezone ? 1 : 0;
            ____pmTimeToggle.targetGraphic.color = allowSelectTimezone ? new Color(1, 1, 1, 1) : new Color(0,0,0,0);
            ____pmTimeToggle.interactable = allowSelectTimezone;

            if (!allowSelectTimezone)
            {
                ____amTimeToggle.isOn = true;
                ____pmTimeToggle.isOn = false;
                raidSettings.SelectedDateTime = JsonType.EDateTime.CURR;
            }
        }
    }
}