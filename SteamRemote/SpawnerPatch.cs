using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV;
using DV.MultipleUnit;
using DV.RemoteControls;
using DV.Simulation.Cars;
using HarmonyLib;
using UnityEngine;

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
				if (type.v1 == DV.ThingTypes.TrainCarType.LocoS060 || (type.v1 == DV.ThingTypes.TrainCarType.LocoSteamHeavy && Main.Settings.applyToLocoHeavy))
				{
					var prefab = type.prefab;
					if (prefab.GetComponentInChildren<ILocomotiveRemoteControl>() == null)
					{
						var controller = type.prefab.AddComponent<RemoteControllerModule>();
						controller.powerFuseId = "fuseboxDummy.ELECTRONICS_MAIN";
						prefab.GetComponent<SimController>().remoteController = controller;
					}
				}
				if ((type.v1 == DV.ThingTypes.TrainCarType.LocoS060 || type.v1 == DV.ThingTypes.TrainCarType.LocoSteamHeavy) && Main.Settings.applyMU)
				{
					var prefab = type.prefab;
					if (prefab.GetComponentInChildren<MultipleUnitModule>() == null)
					{
						var frontAdapter = createAdapter(prefab, "[front dummy cable]");
						var rearAdapter = createAdapter(prefab, "[rear dummy cable]");
						var mu = prefab.AddComponent<MultipleUnitModule>();
						prefab.AddComponent<MuCableBlocker>();
						mu.frontCableAdapter = frontAdapter;
						mu.rearCableAdapter = rearAdapter;
					}
				}
			});
		}

		private static CouplingHoseMultipleUnitAdapter createAdapter(GameObject prefab, string name)
		{
			var front = new GameObject();
			front.name = name;
			var frontRig = front.AddComponent<CouplingHoseRig>();
			frontRig.ropeAnchor = front.transform;
			var frontAdapter = front.AddComponent<CouplingHoseMultipleUnitAdapter>();
			frontRig.adapter = frontAdapter;
			frontAdapter.rig = frontRig;
			front.transform.parent = prefab.transform;
			return frontAdapter;
		}
	}
}
