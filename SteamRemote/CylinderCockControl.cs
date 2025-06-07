using DV.CabControls.Spec;
using DV.HUD;
using DV.Simulation.Controllers;

namespace SteamRemote
{
	public class CylinderCockControl : OverridableBaseControl
	{
		public ControlSpec spec;

		public override InteriorControlsManager.ControlType ControlType => InteriorControlsManager.ControlType.CylCock;
	}
}
