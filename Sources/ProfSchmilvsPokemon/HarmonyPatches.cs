using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine;
using Verse;

namespace ProfSchmilvsPokemon
{

	//thanks to roxxploxx for the tutorial! https://github.com/roxxploxx/RimWorldModGuide/wiki/SHORTTUTORIAL:-Harmony
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		
		static HarmonyPatches()
		{
			HarmonyInstance harmony = HarmonyInstance.Create("rimworld.ProfSchmilvsPokemon");

			//methods to patch
			MethodInfo targetmethod1 = AccessTools.Method(typeof(RimWorld.ITab_Pawn_Character),"FillTab");
			MethodInfo targetmethod2 = AccessTools.Method(typeof(Verse.Map),"ConstructComponents");
			MethodInfo targetmethod3 = AccessTools.Method(typeof(Verse.Map),"MapPostTick");
			MethodInfo targetmethod4 = AccessTools.Method(typeof(RimWorld.CompPower),"PostSpawnSetup");

			//patch methods
			HarmonyMethod prefixmethod1 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("FillTab_Prefix"));
			HarmonyMethod prefixmethod2 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("ConstructComponents_Prefix"));
			HarmonyMethod prefixmethod3 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("MapPostTick_Prefix"));
			HarmonyMethod prefixmethod4 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("PostSpawnSetup_Prefix"));

			//patches
			harmony.Patch( targetmethod1, prefixmethod1, null ) ;
			harmony.Patch( targetmethod2, prefixmethod2, null ) ;
			harmony.Patch( targetmethod3, prefixmethod3, null ) ;
			harmony.Patch( targetmethod4, prefixmethod4, null ) ;
		}
			
		public static void FillTab_Prefix() {
			RimWorld.CharacterCardUtility.PawnCardSize.y = DefDatabase<RimWorld.SkillDef>.AllDefsListForReading.Count * 47.5f;
		}

		public static void ConstructComponents_Prefix(Verse.Map __instance) {
			ProfSchmilvsPokemon.Spawner.spawnerPokemon = new ProfSchmilvsPokemon.Spawner_WildPokemon (__instance);
		}

		public static void MapPostTick_Prefix() {
			try
			{
				ProfSchmilvsPokemon.Spawner.spawnerPokemon.WildPokemonSpawnerTick();
			}catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}

		public static void PostSpawnSetup_Prefix(RimWorld.CompPower __instance) {
			ProfSchmilvsPokemon.Spawner.spawnerPokemon.InkrementPower (__instance);
		}

	}

}