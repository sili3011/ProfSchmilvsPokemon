using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
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
			var harmony = new Harmony("rimworld.ProfSchmilvsPokemon");

			//methods to patch
			var targetMethod1 = AccessTools.Method(typeof(ITab_Pawn_Character),"FillTab");
			var targetMethod2 = AccessTools.Method(typeof(Map),"ConstructComponents");
			var targetMethod3 = AccessTools.Method(typeof(Map),"MapPostTick");
			var targetMethod4 = AccessTools.Method(typeof(CompPower),"PostSpawnSetup");
			//var targetMethod5 = AccessTools.Method(typeof(StorytellerUI),"DrawStorytellerSelectionInterface");
			var targetMethod6 = AccessTools.Method(typeof(Page_SelectStoryteller),"CanDoNext");

			//patch methods
			var prefixMethod1 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("FillTab_Prefix"));
			var prefixMethod2 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("ConstructComponents_Prefix"));
			var prefixMethod3 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("MapPostTick_Prefix"));
			var prefixMethod4 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("PostSpawnSetup_Prefix"));
			var prefixMethod5 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("CanDoNext_Prefix"));

			var postfixMethod1 = new HarmonyMethod(typeof(HarmonyPatches).GetMethod("DrawStorytellerSelectionInterface_Postfix"));

			//patches
			harmony.Patch( targetMethod1, prefixMethod1, null ) ;
			harmony.Patch( targetMethod2, prefixMethod2, null ) ;
			harmony.Patch( targetMethod3, prefixMethod3, null ) ;
			harmony.Patch( targetMethod4, prefixMethod4, null ) ;
			harmony.Patch( targetMethod6, prefixMethod5, null ) ;

			//harmony.Patch( targetMethod5, null, postfixMethod1 ) ;
		}
			
		public static void FillTab_Prefix() {
			CharacterCardUtility.BasePawnCardSize.y = DefDatabase<SkillDef>.AllDefsListForReading.Count * 47.5f;
		}

		public static void ConstructComponents_Prefix(Map __instance) {
			Spawner.spawnerPokemon = new Spawner_WildPokemon (__instance);
		}

		public static void MapPostTick_Prefix() {
			try
			{
				Spawner.spawnerPokemon.WildPokemonSpawnerTick();
			}catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}

		public static void PostSpawnSetup_Prefix(CompPower __instance) {
			Spawner.spawnerPokemon.InkrementPower (__instance);
		}

		public static void DrawStorytellerSelectionInterface_Postfix(Rect rect, ref StorytellerDef chosenStoryteller, ref DifficultyDef difficulty, Listing_Standard selectedStorytellerInfoListing) {
			var rect2 = new Rect(rect.width-410f, 10f, 400f, 50f);
			Widgets.CheckboxLabeled (rect2, "Completely replace vanilla animals with RimMon", ref PokemonConfig.startWith);
		}

		public static void CanDoNext_Prefix() {
			if (PokemonConfig.startWith) {
				DefDatabase<PawnKindDef>.Clear ();
				var hashSet = new HashSet<string>();
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
									
									var text = "Unnamed" + nameof(PawnKindDef) + Rand.Range (1, 100000).ToString () + "A";
									Log.Error (string.Concat(new [] {
										nameof(PawnKindDef),
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
				var hashSet = new HashSet<string>();
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

								var text = "Unnamed" + typeof(PawnKindDef).Name + Rand.Range (1, 100000).ToString () + "A";
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