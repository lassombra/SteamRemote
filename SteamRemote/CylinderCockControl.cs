using DV.HUD;
using DV.Simulation.Controllers;

namespace SteamRemote
{
	public class CylinderCockControl : OverridableBaseControl
	{
		public override InteriorControlsManager.ControlType ControlType => InteriorControlsManager.ControlType.CylCock;
	}
}
