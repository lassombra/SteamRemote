using HarmonyLib;
using System;
using System.Reflection;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using DV;

namespace SteamRemote
{
    public class Main
    {
		public static ModEntry.ModLogger Logger { get; private set; }

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			Logger = modEntry.Logger;
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(modEntry.Assembly);
			return true;
		}
	}
}
