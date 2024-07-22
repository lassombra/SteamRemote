using DV.Logic.Job;
using DV.RemoteControls;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamRemote
{
	[HarmonyPatch(typeof(RemoteControllerModule))]
	internal class RemoteControlPatch
	{
		private static HashSet<RemoteControllerModule> activeCoroutines = new HashSet<RemoteControllerModule>();
		[HarmonyPrefix]
		[HarmonyPatch(typeof(RemoteControllerModule), nameof(RemoteControllerModule.UpdateBrake))]
		public static bool UpdateBrake(RemoteControllerModule __instance, float factor)
		{
			var trainCar = __instance.GetComponentInParent<TrainCar>();
			if (trainCar.brakeSystem.selfLappingController)
			{
				return true;
			}
			var motion = Mathf.Sign(factor) * 0.33f;
			var brakes = trainCar.GetComponent<SimController>().controlsOverrider.Brake;
			brakes.Set(Mathf.Clamp01(brakes.Value + motion));
			return false;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(RemoteControllerModule), nameof(RemoteControllerModule.GetReverserSymbol))]
		public static bool GetReverserSymbol(RemoteControllerModule __instance, ref string __result)
		{
			var control = __instance.GetComponentInParent<TrainCar>().GetComponentInChildren<SimController>().controlsOverrider;
			var reverser = control?.Reverser;
			if (reverser == null)
			{
				return true;
			}
			if (reverser.Value > (ReverserControl.NEUTRAL_VALUE + 0.05f))
			{
				__result = "F";
			} else if (reverser.Value < (ReverserControl.NEUTRAL_VALUE - 0.05f)) {
				__result = "R";
			} else
			{
				__result = "N";
			}
			return false;
		}

		//[HarmonyPostfix]
		//[HarmonyPatch(typeof(RemoteControllerModule), nameof(RemoteControllerModule.PairRemoteController))]
		//public static void PairRemoteController(RemoteControllerModule __instance)
		//{
		//	if (__instance.IsPaired && !activeCoroutines.Contains(__instance))
		//	{
		//		activeCoroutines.Add(__instance);
		//		__instance.StartCoroutine(CutoffCoroutine(__instance));
		//	}
		//}

		//private static IEnumerator CutoffCoroutine(RemoteControllerModule instance)
		//{
		//	Main.Logger.Log("Starting cutoff coroutine");
		//	var trainCar = instance.GetComponentInParent<TrainCar>();
		//	var baseControls = trainCar.GetComponentInChildren<SimController>().controlsOverrider;
		//	var lastSpeed = 0f;
		//	while (instance.IsPaired)
		//	{
		//		var speed = Mathf.Abs(instance.GetForwardSpeed());
		//		Main.Logger.Log("Checking speed " + speed);
		//		Main.Logger.Log("Checking Throttle " + baseControls.Throttle.Value);
		//		if (speed <= 0.5f)
		//		{
		//			switch (instance.GetReverserSymbol())
		//			{
		//				case "F": baseControls.Reverser.Set(1f); break;
		//				case "R": baseControls.Reverser.Set(0f); break;
		//			}
		//		}
		//		else if (speed > 0.5f && baseControls.Throttle.Value <= 0.9f)
		//		{
		//			switch(instance.GetReverserSymbol())
		//			{
		//				case "F": baseControls.Reverser.Set(ReverserControl.NEUTRAL_VALUE + 0.1f); break;
		//				case "R": baseControls.Reverser.Set(ReverserControl.NEUTRAL_VALUE - 0.1f); break;
		//			}
		//		}
		//		else if (baseControls.Throttle.Value > 0.95f && lastSpeed > speed)
		//		{
		//			switch (instance.GetReverserSymbol())
		//			{
		//				case "F": baseControls.Reverser.Set(Mathf.Clamp01(baseControls.Reverser.Value + 0.07f)); break;
		//				case "R": baseControls.Reverser.Set(Mathf.Clamp01(baseControls.Reverser.Value - 0.07f)); break;
		//			}
		//		}
		//		lastSpeed = speed;
		//		yield return WaitFor.Seconds(0.5f);
		//	}
		//	activeCoroutines.Remove(instance);
		//}
	}
}
