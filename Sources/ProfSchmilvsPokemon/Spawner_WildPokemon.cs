using Harmony;
using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace ProfSchmilvsPokemon
{
	public class Spawner_WildPokemon : CompSpawner
	{

		public Spawner_WildPokemon(Map map){
		
			this.map = map;

		}
			
		public void WildPokemonSpawnerTick()
		{

			++currentTick;

			if (currentTick == 250) {

				//MAGNEMITES

				float probMags = (0.01f * this.powerBuildingCount) / (Mathf.Pow(spawnedMagnemites, spawnedMagnemites));
				float rollMags = Rand.Value;

				//Log.Message ("Probability: " + prob + " rolled: " + roll);

				if (rollMags < probMags) {

					Pawn mag = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magnemite"));
					IntVec3 loc;
					RCellFinder.TryFindRandomPawnEntryCell (out loc, this.map, CellFinder.EdgeRoadChance_Animal, null);
					IntVec3 loc2 = CellFinder.RandomClosewalkCellNear(loc, this.map, 8, null);
					GenSpawn.Spawn (mag, loc2, this.map);
					++spawnedMagnemites;

					//Log.Message ("Spawned a Magnemite at: " + loc2.ToString());
				
				}

				currentTick = 0;

				//MAGNEMITES_END

				//GRIMERS

				if (!map.listerFilthInHomeArea.FilthInHomeArea.NullOrEmpty ()) {

					float probGrimer = ((float)map.listerFilthInHomeArea.FilthInHomeArea.Count/40f);
					float rollGrimer = Rand.Value;

					if (rollGrimer < probGrimer) {

						Pokemon_Grimer grimer = (Pokemon_Grimer )PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Grimer"));
						IntVec3 loc2 = CellFinder.RandomClosewalkCellNear (map.areaManager.Home.ActiveCells.RandomElement (), this.map, 8, null);

						List<Thing> filth = map.listerFilthInHomeArea.FilthInHomeArea;

						for (int k = 0; k < filth.Count; k++) {
							grimer.IncrementFilth ();
							filth [k].DeSpawn ();
						}

						map.listerFilthInHomeArea.FilthInHomeArea.Clear ();

						GenSpawn.Spawn (grimer, loc2, this.map);
						Log.Message ("Spawned a Grimer at: " + loc2.ToString ());

					}

				}
					
				//GRIMERS_END
			
			}

		}

		public void InkrementPower(CompPower cp){
		
			string compType = cp.parent.def.defName.ToString ();

			this.PowerComps.Add (cp);

			float amount = 0f;

			switch (compType) {
				case "Battery":
					amount = 1.5f;
					break;
				case "PowerConduit":
					amount = 0.25f;
					break;
				case "PowerSwitch":
					amount = 0.1f;
					break;
				case "WoodFiredGenerator":
					amount = 1.0f;
					break;
				case "ChemfuelPoweredGenerator":
					amount = 5.0f;
					break;
				case "SolarGenerator":
					amount = 1.5f;
					break;
				case "GeothermalGenerator":
					amount = 7.5f;
					break;
			}

			this.powerBuildingCount += amount;
		
		}

		public void ExposeData()
		{
			Scribe_Values.Look<float>(ref this.powerBuildingCount, "powerBuildings", 0f);
			Scribe_Values.Look<int>(ref this.spawnedMagnemites, "spawnedMagnemites", 0);
			Scribe_Deep.Look<List<CompPower>>(ref this.PowerComps, "powerComps", new object[0]);
		}

		private Map map;
		public float powerBuildingCount = 0;
		private int spawnedMagnemites = 0;
		private int currentTick = 0;
		public List<CompPower> PowerComps = new List<CompPower>();

	}

}