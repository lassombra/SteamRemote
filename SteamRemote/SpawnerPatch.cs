using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV;
using DV.RemoteControls;
using DV.Simulation.Cars;
using HarmonyLib;

namespace SteamRemote
{
	[HarmonyPatch(typeof(CarSpawner), "Awake")]
	public class SpawnerPatch
	{
		static void Prefix()
		{
			Main.Logger.Log("Parsing Prefabs");
			Globals.G.Types.Liveries.ForEach(type =>
			{
				if (type.v1 == DV.ThingTypes.TrainCarType.LocoS060)
				{
					var prefab = type.prefab;
					if (prefab.GetComponentInChildren<ILocomotiveRemoteControl>() == null)
					{
						var controller = type.prefab.AddComponent<RemoteControllerModule>();
						controller.powerFuseId = "fuseboxDummy.ELECTRONICS_MAIN";
						prefab.GetComponent<SimController>().remoteController = controller;
					}
				}
			});
		}
	}
}
