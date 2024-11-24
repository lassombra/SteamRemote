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
		public static Settings Settings { get; private set; }

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			Logger = modEntry.Logger;
			var harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(modEntry.Assembly);
			Settings = Settings.Load<Settings>(modEntry);
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;
			return true;
		}

		public static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Draw(modEntry);
		}
		public static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Settings.Save(modEntry);
		}
	}
}
