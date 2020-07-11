using ProfSchmilvsPokemon.Pokemon.Slime;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;
using Verse;
using Verse.AI;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Muk : PokemonAbstractSlimes {

		#region Properties
		//
		public ThingDef_Muk Def
		{
			get {
				return def as ThingDef_Muk;
			}
		}
		//
		#endregion Properties

		public override void Tick() {

			base.Tick ();

			++CurrentTick;

			if (CurrentTick == 60) {

				CurrentTick = 0;

				var prob = AmountOfFilth/150;
				var roll = Rand.Value;

				if(roll < prob && AmountOfFilth > 0){

					FilthMaker.TryMakeFilth(Position, base.Map, ThingDefOf.Filth_Slime);
					this.DecrementFilth ();

				}
			}

			if (Digesting == null) {

				if (CurrentDump == null) {

					if (!Map.zoneManager.AllZones.NullOrEmpty ()) {

						var zones = Map.zoneManager.AllZones;
						var nextDistance = 999999f;

						foreach (var z in zones) {

							if (z.label.Split (' ') [0].Equals ("Dumping")) {

								if (nextDistance > Position.DistanceTo (z.cells [0])) {

									CurrentDump = z;

								}
							}
						}
					}

					if (CurrentDump != null) {
						jobs.ClearQueuedJobs ();
						pather.StartPath (new LocalTargetInfo (CurrentDump.cells [0]), PathEndMode.OnCell);
					}

				} else {
					
					if (Position.AdjacentTo8WayOrInside (CurrentDump.cells [0])) {

						Thing toBeDigested = null;

						var ts = CurrentDump.AllContainedThings;

						foreach(var t in ts){

							if (t is PokemonAbstractSlimes && t.Faction == null) {

								PokemonAbstractSlimes g = (PokemonAbstractSlimes)t;
								for (var gi = (int)g.AmountOfFilth; gi >= 0; gi--) {
									IncrementFilth ();
								}
								t.DeSpawn ();
								IncrementFilth ();

							}else if(t != null && !(t is Pawn)){
								toBeDigested = t;
								break;
							}
						}

						DigestingTicks = 15000L;
						toBeDigested.DeSpawn ();
						CurrentDump = null;
						Digesting = toBeDigested;

					} else if (pather.Destination != new LocalTargetInfo (CurrentDump.cells [0])) {

						jobs.ClearQueuedJobs ();
						pather.StartPath (new LocalTargetInfo (CurrentDump.cells [0]), Verse.AI.PathEndMode.OnCell);

					}
				}

			} else {

				--DigestingTicks;

				if (DigestingTicks <= 0) {
					++AmountOfFilth;
					Digesting = null;
				}
			}
		}
	}
}