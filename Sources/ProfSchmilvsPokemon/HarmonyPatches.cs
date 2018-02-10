using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ProfSchmilvsPokemon
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		// this static constructor runs to create a HarmonyInstance and install a patch.
		static HarmonyPatches()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("rimworld.ProfSchmilvsPokemon");

			// find the FillTab method of the class RimWorld.ITab_Pawn_Character
			MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.ITab_Pawn_Character),"FillTab");

			// find the static method to call before (i.e. Prefix) the targetmethod
			HarmonyMethod prefixmethod = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("FillTab_Prefix"));

			// patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
			harmony.Patch( targetmethod, prefixmethod, null ) ;
		}

		// This method is now always called right before RimWorld.ITab_Pawn_Character.FillTab.
		// So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window.
		// The class RimWorld.ITab_Pawn_Character is static so there is no this __instance.
		public static void FillTab_Prefix() {
			RimWorld.CharacterCardUtility.PawnCardSize.y = DefDatabase<RimWorld.SkillDef>.AllDefsListForReading.Count * 47.5f;
		}

	}

}

