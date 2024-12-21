using DV.Simulation.Cars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamRemote
{
	public class SteamControlsOverrider : MonoBehaviour
	{
		public CylinderCockControl? CylinderCock => this.cylinderCock ?? null;
		public CylinderCockControl? cylinderCock;
	}
}
