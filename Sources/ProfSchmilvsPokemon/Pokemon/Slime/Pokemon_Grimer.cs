using System.Collections.Generic;
using ProfSchmilvsPokemon.Pokemon.Slime;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;
using Verse;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Grimer : PokemonAbstractSlimes {

		#region Properties
		//
		public ThingDef_Grimer Def
		{
			get {
				return def as ThingDef_Grimer;
			}
		}
		//
		#endregion Properties

		public override void Tick() {

			base.Tick ();
			++this.CurrentTick;

			if (this.AmountOfFilth > 20) {

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

				for (int i = (int)this.AmountOfFilth; i >= 0; i--) {
					muk.IncrementFilth ();
				}

				if (this.Spawned) {
					GenSpawn.Spawn (muk, this.Position, base.Map);
					this.DeSpawn ();
				}
			
			} else {

				if (this.CurrentTick == 600) {

					this.CurrentTick = 0;

					float prob = this.AmountOfFilth / 200;
					float roll = Rand.Value;

					if (roll < prob && this.AmountOfFilth > 0) {
						FilthMaker.TryMakeFilth (this.Position, base.Map, ThingDefOf.Filth_Slime);
						this.DecrementFilth ();
					}
				}

				if (Digesting == null) {

					if (CurrentDump == null) {

						if(!base.Map.zoneManager.AllZones.NullOrEmpty()){

							List<Zone> zones = base.Map.zoneManager.AllZones;
							float nextDistance = 999999f;

							foreach (Zone z in zones) {
							
								if (z.label.Split (' ') [0].Equals ("Dumping")) {
								
									if (nextDistance > this.Position.DistanceTo (z.cells [0])) {
									
										this.CurrentDump = z;
									
									}
								}
							}
						}

						if (this.CurrentDump != null) {
							this.jobs.ClearQueuedJobs ();
							this.pather.StartPath (new LocalTargetInfo (this.CurrentDump.cells [0]), Verse.AI.PathEndMode.OnCell);
						}

					} else {
						
						if (this.Position.AdjacentTo8WayOrInside (this.CurrentDump.cells [0])) {

							Thing toBeDigested = null;

							var ts = this.CurrentDump.AllContainedThings;

							foreach(var t in ts){
								
								if (t is Pokemon_Grimer && t.Faction == null && t != this) {
									
									var g = (Pokemon_Grimer)t;
									for (var gi = (int)g.AmountOfFilth; gi >= 0; gi--) {
										this.IncrementFilth ();
									}
									t.DeSpawn ();
									this.IncrementFilth ();

								}else if(t != null && !(t is Pawn)){
									toBeDigested = t;
									break;
								}
							}

							this.DigestingTicks = 30000L;
							toBeDigested.DeSpawn ();
							this.CurrentDump = null;
							this.Digesting = toBeDigested;

						} else if (this.pather.Destination != new LocalTargetInfo (this.CurrentDump.cells [0])) {

							this.jobs.ClearQueuedJobs ();
							this.pather.StartPath (new LocalTargetInfo (this.CurrentDump.cells [0]), Verse.AI.PathEndMode.OnCell);

						}

					}
							
				} else {
				
					--this.DigestingTicks;

					if (this.DigestingTicks <= 0) {
						++this.AmountOfFilth;
						this.Digesting = null;
					}
				}
			}
		}
	}
}