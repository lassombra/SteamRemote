using DV.MultipleUnit;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamRemote
{
	[HarmonyPatch(typeof(MultipleUnitCable))]
	internal class MUCablePatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(nameof(MultipleUnitCable.Connect))]
		public static bool PatchConnect(MultipleUnitCable other, bool playAudio, MultipleUnitCable __instance)
		{
			if (__instance.muModule.GetComponent<MuCableBlocker>() != null)
			{
				return false;
			}
			return true;
		}
	}
}
