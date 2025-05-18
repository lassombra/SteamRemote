using DV.CabControls;
using DV.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamRemote.DummyControl
{
	internal class DummyControlImpl : ControlImplBase, IScrollable
	{
		protected override InteractionHandPoses GenericHandPoses => new InteractionHandPoses();

		public override void ForceEndInteraction()
		{
			return;
		}

		void OnDisable()
		{
			// If object will destroy in the end of current frame...
			if (gameObject.activeInHierarchy)
			{
				Debug.LogError("DummyControlImpl: OnDisable() called while object is still active in hierarchy. This should not happen. Please report this to the mod author.");
			}
			// If object just deactivated..
			else
			{

			}
		}

		public bool IsAtEnd(ScrollAction action)
		{
			float notchstep = 1.0f / ((DummyControlSpec)spec).notches;
			if (action == ScrollAction.ScrollUp)
			{
				return Value >= notchstep * (((DummyControlSpec)spec).notches -1);
			}
			else if (action == ScrollAction.ScrollDown)
			{
				return Value <= notchstep;
			}
			return false;
		}

		public override bool IsGrabbed()
		{
			return false;
		}

		public void Scroll(ScrollAction action, ScrollSource source = ScrollSource.Mouse)
		{
			if (spec == null)
			{
				spec = base.GetComponent<DummyControlSpec>();
				if (spec == null)
				{
					Main.Logger.Error("DummyControlImpl: spec is null. This should not happen. Please report this to the mod author.");
					return;
				}
			}
			float notchstep = 1.0f / ((DummyControlSpec)spec).notches;
			if (action == ScrollAction.ScrollUp)
			{
				SetValue(Mathf.Min(1.0f, Value + notchstep));
			}
			else if (action == ScrollAction.ScrollDown)
			{
				SetValue(Mathf.Max(0.0f, Value - notchstep));
			}
		}

		protected override void AcceptSetValue(float newValue)
		{
			return;
		}
	}
}
