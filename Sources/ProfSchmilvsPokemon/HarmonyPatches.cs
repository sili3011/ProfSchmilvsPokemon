using Harmony;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Xml;
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
			MethodInfo targetmethod5 = AccessTools.Method(typeof(RimWorld.StorytellerUI),"DrawStorytellerSelectionInterface");
			MethodInfo targetmethod6 = AccessTools.Method(typeof(RimWorld.Page_SelectStoryteller),"CanDoNext");

			//patch methods
			HarmonyMethod prefixmethod1 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("FillTab_Prefix"));
			HarmonyMethod prefixmethod2 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("ConstructComponents_Prefix"));
			HarmonyMethod prefixmethod3 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("MapPostTick_Prefix"));
			HarmonyMethod prefixmethod4 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("PostSpawnSetup_Prefix"));
			HarmonyMethod prefixmethod5 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("CanDoNext_Prefix"));

			HarmonyMethod postfixmethod1 = new HarmonyMethod(typeof(ProfSchmilvsPokemon.HarmonyPatches).GetMethod("DrawStorytellerSelectionInterface_Postfix"));

			//patches
			harmony.Patch( targetmethod1, prefixmethod1, null ) ;
			harmony.Patch( targetmethod2, prefixmethod2, null ) ;
			harmony.Patch( targetmethod3, prefixmethod3, null ) ;
			harmony.Patch( targetmethod4, prefixmethod4, null ) ;
			harmony.Patch( targetmethod6, prefixmethod5, null ) ;

			harmony.Patch( targetmethod5, null, postfixmethod1 ) ;
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

		public static void DrawStorytellerSelectionInterface_Postfix(Rect rect, ref StorytellerDef chosenStoryteller, ref DifficultyDef difficulty, Listing_Standard selectedStorytellerInfoListing) {
			Rect rect2 = new Rect(rect.width-410f, 10f, 400f, 50f);
			Widgets.CheckboxLabeled (rect2, "Completely replace vanilla animals with RimMon", ref PokemonConfig.startWith);
		}

		public static void CanDoNext_Prefix() {
			if (PokemonConfig.startWith) {
				DefDatabase<PawnKindDef>.Clear ();
				HashSet<string> hashSet = new HashSet<string>();
				foreach (ModContentPack modContentPack in (from m in LoadedModManager.RunningMods orderby m.OverwritePriority select m).ThenBy((ModContentPack x) => LoadedModManager.RunningModsListForReading.IndexOf(x))){
					
					hashSet.Clear();
					foreach (PawnKindDef t in GenDefDatabase.DefsToGoInDatabase<PawnKindDef>(modContentPack)){

						if (!modContentPack.IsCoreMod) {

							if (hashSet.Contains (t.defName)) {
								
								Log.Error (string.Concat (new object[] {
									"Mod ",
									modContentPack,
									" has multiple ",
									typeof(PawnKindDef),
									"s named ",
									t.defName,
									". Skipping."
								}));

							} else {
								
								if (t.defName == "UnnamedDef") {
									
									string text = "Unnamed" + typeof(PawnKindDef).Name + Rand.Range (1, 100000).ToString () + "A";
									Log.Error (string.Concat (new string[] {
										typeof(PawnKindDef).Name,
										" in ",
										modContentPack.ToString (),
										" with label ",
										t.label,
										" lacks a defName. Giving name ",
										text
									}));
									t.defName = text;
								}

								/*PawnKindDef def;
								if (DefDatabase<PawnKindDef>.defsByName.TryGetValue(t.defName, out def)){
										DefDatabase<PawnKindDef>.Remove(def);
								}*/

								DefDatabase<PawnKindDef>.Add (t);
							}
						}
					}
				}
			} else {
				DefDatabase<PawnKindDef>.Clear ();
				HashSet<string> hashSet = new HashSet<string>();
				foreach (ModContentPack modContentPack in (from m in LoadedModManager.RunningMods orderby m.OverwritePriority select m).ThenBy((ModContentPack x) => LoadedModManager.RunningModsListForReading.IndexOf(x))){

				hashSet.Clear();
				foreach (PawnKindDef t in GenDefDatabase.DefsToGoInDatabase<PawnKindDef>(modContentPack)){

						if (hashSet.Contains (t.defName)) {

							Log.Error (string.Concat (new object[] {
								"Mod ",
								modContentPack,
								" has multiple ",
								typeof(PawnKindDef),
								"s named ",
								t.defName,
								". Skipping."
							}));

						} else {

							if (t.defName == "UnnamedDef") {

								string text = "Unnamed" + typeof(PawnKindDef).Name + Rand.Range (1, 100000).ToString () + "A";
								Log.Error (string.Concat (new string[] {
									typeof(PawnKindDef).Name,
									" in ",
									modContentPack.ToString (),
									" with label ",
									t.label,
									" lacks a defName. Giving name ",
									text
								}));
								t.defName = text;
							}

							/*PawnKindDef def;
							if (DefDatabase<PawnKindDef>.defsByName.TryGetValue(t.defName, out def)){
									DefDatabase<PawnKindDef>.Remove(def);
							}*/

							DefDatabase<PawnKindDef>.Add (t);
						}
					}
				}
			}
		}

	}

}