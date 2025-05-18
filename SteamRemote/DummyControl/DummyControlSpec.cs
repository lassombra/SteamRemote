using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV.CabControls.Spec;

namespace SteamRemote.DummyControl
{
	internal class DummyControlSpec : ControlSpec, INotchedSpec
	{
		public override InteractableTag InteractableTag => InteractableTag.None;
		public bool IsNotched
		{
			get
			{
				return useSteppedJoint;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000021C7 File Offset: 0x000003C7
		public int NotchCount
		{
			get
			{
				return notches;
			}
		}

		public int notches = 20;
		public bool useSteppedJoint = true;
	}
}
