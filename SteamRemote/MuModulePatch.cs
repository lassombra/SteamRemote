using DV.MultipleUnit;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamRemote
{
	[HarmonyPatch(typeof(MultipleUnitModule))]
	public class MuModulePatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(MultipleUnitModule.SetupListeners))]
		public static void SetupListeners_Postfix(bool set, MultipleUnitModule __instance) {
			var steamMuEnhancer = __instance.gameObject.GetComponent<SteamMuEnhancer>();
			if (steamMuEnhancer != null)
			{
				steamMuEnhancer.SetupListeners(set);
			}
		}
	}
}
