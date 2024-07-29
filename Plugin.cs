using BepInEx;
using System.Linq;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace OneTimeZone
{
	[BepInPlugin("dev.birgerev.OneTimeZone", "OneTimeZone", "1.0.0")]
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
            //Access method LocationSettingsClass.Location.Class816.method_0(bool x)
            return AccessTools.GetDeclaredMethods(typeof(LocationSettingsClass.Location.Class816)).FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
            });
        }

        [PatchPostfix]
        public static void Postfix(ref LocationSettingsClass.Location.Class816 __instance, ref bool __result, string x)
        {
            //bool __result returns whether we should show the timezone dialouge
            //string x is the map name

            //Only show dialouge on factory since time is static there
            bool allowSelectTimezone = x == "factory4_day" || x == "factory4_night";

            __result = allowSelectTimezone;
        }
    }
}