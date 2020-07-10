using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI;
using RimWorld;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Grimer : Pokemon_Abstract_Slimes {

		#region Properties
		//
		public ThingDef_Grimer Def
		{
			get 
			{
				return this.def as ThingDef_Grimer;
			}
		}
		//
		#endregion Properties

		public override void Tick() {

			base.Tick ();
			++this.currentTick;

			if (this.amountOfFilth > 20) {

				Pokemon_Muk muk = (Pokemon_Muk)PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Muk"));

				if (this.Faction != null) {

					if (this.playerSettings.Master != null) {
						muk.SetFaction (this.Faction, this.playerSettings.Master);
						muk.training.Train (TrainableUtility.TrainableDefsInListOrder [0], this.playerSettings.Master);
					} else {
						muk.SetFaction (this.Faction, this.Faction.leader); //should not happen, just for debugging purposes
					}

					if (!this.Name.ToString ().Split (' ') [0].Equals ("grimer")) {
						muk.Name = this.Name;
					}
				}

				for (int i = (int)this.amountOfFilth; i >= 0; i--) {
					muk.IncrementFilth ();
				}

				if (this.Spawned) {
					GenSpawn.Spawn (muk, this.Position, base.Map);
					this.DeSpawn ();
				}
			
			} else {

				if (this.currentTick == 600) {

					this.currentTick = 0;

					float prob = this.amountOfFilth / 200;
					float roll = Rand.Value;

					if (roll < prob && this.amountOfFilth > 0) {
						FilthMaker.TryMakeFilth (this.Position, base.Map, ThingDefOf.Filth_Slime);
						this.DecrementFilth ();
					}
				}

				if (digesting == null) {

					if (currentDump == null) {

						if(!base.Map.zoneManager.AllZones.NullOrEmpty()){

							List<Zone> zones = base.Map.zoneManager.AllZones;
							float nextDistance = 999999f;

							foreach (Zone z in zones) {
							
								if (z.label.Split (' ') [0].Equals ("Dumping")) {
								
									if (nextDistance > this.Position.DistanceTo (z.cells [0])) {
									
										this.currentDump = z;
									
									}
								}
							}
						}

						if (this.currentDump != null) {
							this.jobs.ClearQueuedJobs ();
							this.pather.StartPath (new LocalTargetInfo (this.currentDump.cells [0]), Verse.AI.PathEndMode.OnCell);
						}

					} else {
						
						if (this.Position.AdjacentTo8WayOrInside (this.currentDump.cells [0])) {

							Thing toBeDigested = null;

							var ts = this.currentDump.AllContainedThings;

							foreach(var t in ts){
								
								if (t is Pokemon_Grimer && t.Faction == null && t != this) {
									
									var g = (Pokemon_Grimer)t;
									for (var gi = (int)g.amountOfFilth; gi >= 0; gi--) {
										this.IncrementFilth ();
									}
									t.DeSpawn ();
									this.IncrementFilth ();

								}else if(t != null && !(t is Pawn)){
									toBeDigested = t;
									break;
								}
							}

							this.digestingTicks = 30000L;
							toBeDigested.DeSpawn ();
							this.currentDump = null;
							this.digesting = toBeDigested;

						} else if (this.pather.Destination != new LocalTargetInfo (this.currentDump.cells [0])) {

							this.jobs.ClearQueuedJobs ();
							this.pather.StartPath (new LocalTargetInfo (this.currentDump.cells [0]), Verse.AI.PathEndMode.OnCell);

						}

					}
							
				} else {
				
					--this.digestingTicks;

					if (this.digestingTicks <= 0) {
						++this.amountOfFilth;
						this.digesting = null;
					}
				}
			}
		}
	}
}