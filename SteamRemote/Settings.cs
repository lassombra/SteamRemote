using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace SteamRemote
{
	public class Settings : UnityModManager.ModSettings, IDrawable
	{
		[Draw(DrawType.Toggle, Label = "Add remote capability to S282")] public bool applyToLocoHeavy = false;
		[Draw(DrawType.Toggle, Label = "Add Wireless MU capability")] public bool applyMU = true;
		public void OnChange()
		{ }

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}
	}
}
