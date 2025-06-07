using DV.Simulation.Cars;
using HarmonyLib;
using LocoSim.Implementations;

namespace SteamRemote
{
	[HarmonyPatch(typeof(BaseControlsOverrider))]
	public class BaseControlsOverriderPatch
	{
		[HarmonyPatch(nameof(BaseControlsOverrider.Init))]
		[HarmonyPostfix]
		public static void Init(TrainCar car, SimulationFlow simFlow, BaseControlsOverrider __instance)
		{
			var steamControlOverrider = __instance.gameObject.GetComponent<SteamControlsOverrider>();
			if (steamControlOverrider?.cylinderCock != null)
			{
				steamControlOverrider.cylinderCock.Init(car, simFlow, steamControlOverrider.cylinderCock.spec);
			}
		}
	}
}
