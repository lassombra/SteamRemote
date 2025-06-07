using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV;
using DV.CabControls;
using DV.CabControls.Spec;
using DV.KeyboardInput;
using DV.MultipleUnit;
using DV.RemoteControls;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using DV.UI;
using HarmonyLib;
using LocoSim.Definitions;
using SteamRemote.DummyControl;
using UnityEngine;

namespace SteamRemote
{
	[HarmonyPatch(typeof(CarSpawner), "Awake")]
	public class SpawnerPatch
	{
		static void Prefix()
		{
			ControlsInstantiator.TypeMap.Add(typeof(DummyControlSpec), new ControlsInstantiator.Impl { pc = typeof(DummyControlImpl), vr = typeof(DummyControlImpl) });
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
						var dynamicBrakeControl = CreateDynamicBrakeControl(prefab, type.interiorPrefab);
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
			var spec = control.GetComponent<ControlSpec>();
			var cylinderCockControl = control.gameObject.AddComponent<CylinderCockControl>();
			cylinderCockControl.spec = spec;
			cylinderCockControl.portId = control.ID + ".EXT_IN";
			return cylinderCockControl;
		}

		private static DynamicBrakeControl CreateDynamicBrakeControl(GameObject prefab, GameObject interiorPrefab)
		{
			var dynamicBrakeObject = CreateDynamicBrakeControlGameObject(prefab);
			var dynamicBrakeControl = CreateDynamicBrake(dynamicBrakeObject, prefab);
			CreateDummyControl(dynamicBrakeObject, prefab, interiorPrefab);
			return dynamicBrakeControl;
		}

		private static ControlSpec CreateDummyControl(GameObject dynamicBrakeObject, GameObject prefab, GameObject interiorPrefab)
		{
			Main.Logger.Log($"Creating dummy control on interior {interiorPrefab}");
			var dummyControlGameObject = new GameObject();
			dummyControlGameObject.name = "dynamicBrakeDummy";
			dummyControlGameObject.transform.parent = interiorPrefab.transform;
			Main.Logger.Log($"Dummy control made {dummyControlGameObject}");
			var portFeeder = dummyControlGameObject.AddComponent<InteractablePortFeeder>();
			portFeeder.portId = "dynamicBrake.EXT_IN";
			Main.Logger.Log($"Dummy control port {portFeeder.portId}");
			var spec = dummyControlGameObject.AddComponent<DummyControlSpec>();
			spec.notches = 7;
			interiorPrefab.GetComponentInChildren<InteractablePortFeedersController>().entries =
				interiorPrefab.GetComponentInChildren<InteractablePortFeedersController>().entries.AddToArray(portFeeder);
			AddKeyboardAndJoystickControlsForDynamicBrake(dummyControlGameObject, prefab, interiorPrefab);
			return spec;
		}

		private static void AddKeyboardAndJoystickControlsForDynamicBrake(GameObject dummyControlGameObject, GameObject prefab, GameObject interiorPrefab)
		{
			var keyboard = dummyControlGameObject.AddComponent<MouseScrollKeyboardInput>();
			keyboard.scrollAction = new AKeyboardInput.ActionReference();
			keyboard.scrollAction.name = "DynamicBrakeIncremental";
			var joystick = dummyControlGameObject.AddComponent<AnalogSetValueJoystickInput>();
			joystick.action = new AKeyboardInput.ActionReference();
			joystick.action.name = "DynamicBrakeAbsolute";
			var interactibleControl = interiorPrefab.GetComponentInChildren<InteractablesKeyboardControl>();
			interactibleControl.entries =
				interiorPrefab.GetComponentInChildren<InteractablesKeyboardControl>().entries.AddRangeToArray(new AKeyboardInput[] { keyboard, joystick } );
		}

		private static DynamicBrakeControl CreateDynamicBrake(GameObject dynamicBrakeObject, GameObject prefab)
		{
			var dynamicBrakeControlDef = dynamicBrakeObject.AddComponent<ExternalControlDefinition>();
			dynamicBrakeControlDef.ID = "dynamicBrake";
			var dynamicBrakeControl = dynamicBrakeObject.AddComponent<DynamicBrakeControl>();
			dynamicBrakeControl.portId = dynamicBrakeControlDef.ID + ".EXT_IN";
			prefab.GetComponentInChildren<BaseControlsOverrider>().dynamicBrake = dynamicBrakeControl;
			var simController = prefab.GetComponentInChildren<SimController>();
			simController.connectionsDefinition.executionOrder = simController.connectionsDefinition.executionOrder.AddToArray(dynamicBrakeControlDef);
			return dynamicBrakeControl;
		}

		private static GameObject CreateDynamicBrakeControlGameObject(GameObject prefab)
		{
			var controls = prefab.GetComponentsInChildren<ExternalControlDefinition>().First();
			var dynamicBrakeObject = new GameObject();
			dynamicBrakeObject.name = "dynamicBrakeControl";
			dynamicBrakeObject.transform.parent = controls.transform.parent;
			return dynamicBrakeObject;
		}
	}
}
