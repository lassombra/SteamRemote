using DV.MultipleUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamRemote
{
	public class SteamMuEnhancer : MonoBehaviour
	{
		public MultipleUnitModule? muModule;
		public SteamControlsOverrider? controlsOverrider;

		internal void SetupListeners(bool set)
		{
			if (controlsOverrider == null)
			{
				return;
			}
			if (set)
			{
				if (controlsOverrider.CylinderCock != null)
				{
					controlsOverrider.CylinderCock.ControlUpdated += this.OnCylinderCockUpdated;
				}
			}
			else
			{
				if (controlsOverrider.CylinderCock != null)
				{
					controlsOverrider.CylinderCock.ControlUpdated -= this.OnCylinderCockUpdated;
				}
			}
		}

		private void OnCylinderCockUpdated(float val)
		{
			if (muModule != null && muModule.UseWireless && muModule?.RemoteChannel.Transmitter == muModule)
			{
#pragma warning disable CS8602 // Dereference of a possibly null reference.
				foreach (var otherModule in muModule.RemoteChannel.devices)
				{
					if (otherModule != muModule) {
						otherModule.gameObject.GetComponent<SteamMuEnhancer>()?.SetCylinderCocks(val);
					}
				}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			}
		}

		private void SetCylinderCocks(float val)
		{
			if (controlsOverrider?.CylinderCock != null)
			{
				controlsOverrider.CylinderCock.MUOverride(val);
			}
		}
	}
}
