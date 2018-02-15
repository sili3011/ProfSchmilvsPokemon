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
	public class Pokemon_Grimer : Pawn
	{

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

		public override void Draw()
		{
			base.Draw();
		}

		public override void Tick()
		{
			base.Tick ();
			++this.currentTick;

			if (this.amountOfFilth > 20) {

				Pokemon_Muk muk = (Pokemon_Muk)PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Muk"));
				if (this.Faction != null) {
					if (this.playerSettings.master != null) {
						muk.SetFaction (this.Faction, (Pawn)this.playerSettings.master);
						muk.training.Train (TrainableUtility.TrainableDefsInListOrder [0], (Pawn)this.playerSettings.master);
					} else {
						muk.SetFaction (this.Faction, (Pawn)this.Faction.leader); //should not happen, just for debugging purposes
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

				if (this.currentTick == 60) {

					this.currentTick = 0;

					float prob = this.amountOfFilth / 100;
					float roll = Rand.Value;

					if (roll < prob && this.amountOfFilth > 0) {

						FilthMaker.MakeFilth (this.Position, base.Map, ThingDefOf.FilthSlime);
						this.DecrementFilth ();

					}
				
				}

				if (digesting == null) {

					if (currentDump == null) {

						List<Zone> zones = base.Map.zoneManager.AllZones;
						float nextDistance = 999999f;

						foreach (Zone z in zones) {
						
							if (z.label.Split (' ') [0].Equals ("Dumping")) {
							
								if (nextDistance > this.Position.DistanceTo (z.cells [0])) {
								
									this.currentDump = z;
								
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

							IEnumerable<Thing> ts = this.currentDump.AllContainedThings;

							foreach(Thing t in ts){
								toBeDigested = t;
								if(toBeDigested != null && toBeDigested != this){
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

		public void IncrementFilth()
		{
			++this.amountOfFilth;
			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 3600000L * 150L;
		}

		public void DecrementFilth()
		{
			--this.amountOfFilth;
			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 3600000L * 150L;
		}
			
		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.amountOfFilth, "amountOfFilth", 0f);
			Scribe_Deep.Look<Zone>(ref this.currentDump, "currentDump", new object[0]);
			Scribe_Deep.Look<Thing>(ref this.digesting, "digesting", new object[0]);
			Scribe_Values.Look<long>(ref this.digestingTicks, "digestingTicks", 0L);
		}

		public float amountOfFilth = 0f;
		private int currentTick = 0;
		private Zone currentDump = null;
		private Thing digesting = null;
		private long digestingTicks = 0L;

	}
}