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

				float prob = (0.01f * this.powerBuildingCount) / (Mathf.Pow(spawnedMagnemites, spawnedMagnemites));
				float roll = Rand.Value;

				//Log.Message ("Tick: " + prob + " rolled: " + roll);

				if (roll < prob) {

					Pawn mag = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magnemite"));
					IntVec3 loc;
					RCellFinder.TryFindRandomPawnEntryCell (out loc, this.map, CellFinder.EdgeRoadChance_Animal, null);
					IntVec3 loc2 = CellFinder.RandomClosewalkCellNear(loc, this.map, 8, null);
					GenSpawn.Spawn (mag, loc2, this.map);
					++spawnedMagnemites;

					//Log.Message ("Spawned a Magnemite at: " + loc.ToString());
				
				}

				currentTick = 0;
			
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