using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV;
using DV.MultipleUnit;
using DV.RemoteControls;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using HarmonyLib;
using LocoSim.Definitions;
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
						var frontAdapter = CreateAdapter(prefab, "[front dummy cable]");
						var rearAdapter = CreateAdapter(prefab, "[rear dummy cable]");
						var mu = prefab.AddComponent<MultipleUnitModule>();
						prefab.AddComponent<MuCableBlocker>();
						mu.frontCableAdapter = frontAdapter;
						mu.rearCableAdapter = rearAdapter;
						var cylinderCockControl = CreateCylinderCockControl(prefab);
						if (cylinderCockControl != null)
						{
							var overrider = UpdateControlsOverrider(prefab, cylinderCockControl);
							var enhancer = mu.gameObject.AddComponent<SteamMuEnhancer>();
							enhancer.controlsOverrider = overrider;
							enhancer.muModule = mu;
						}
					}
				}
			});
		}

		private static SteamControlsOverrider UpdateControlsOverrider(GameObject prefab, CylinderCockControl cylinderCockControl)
		{
			var baseControlsOverrider = prefab.GetComponentInChildren<BaseControlsOverrider>();
			var steamControlsOverrider = baseControlsOverrider.gameObject.AddComponent<SteamControlsOverrider>();
			steamControlsOverrider.cylinderCock = cylinderCockControl;
			return steamControlsOverrider;
		}

		private static CouplingHoseMultipleUnitAdapter CreateAdapter(GameObject prefab, string name)
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

		private static CylinderCockControl? CreateCylinderCockControl(GameObject prefab)
		{
			var controls = prefab.GetComponentsInChildren<ExternalControlDefinition>();
			var control = (from c in controls
						   where c.ID == "cylinderCock"
						   select c).First();
			if (control == null)
			{
				return null;
			}
			var cylinderCockControl = control.gameObject.AddComponent<CylinderCockControl>();
			cylinderCockControl.portId = control.ID + ".EXT_IN";
			return cylinderCockControl;
		}
	}
}
