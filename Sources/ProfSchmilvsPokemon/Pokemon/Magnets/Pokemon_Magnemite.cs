using System.Collections.Generic;
using ProfSchmilvsPokemon.Pokemon.Magnets;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;
using Verse;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Magnemite : PokemonAbstractMagnets
	{
        #region Properties
		//
		public ThingDef_Magnemite Def
		{
			get {
				return def as ThingDef_Magnemite;
			}
		}
		//
		#endregion Properties

        public Pokemon_Magnemite() {
            base.StoredEnergyMax = Def.storedEnergyMaxUtility;
        }

		public override void Evolve(){

			var magnemitesInRange = new List<Pokemon_Magnemite>();
			magnemitesInRange.Add (this);

			for (var i = 0; i < 25; i++) {

				var intVec = Position + GenRadial.RadialPattern [i];

				if (intVec.InBounds (Map)) {

					Thing thing = intVec.GetThingList (Map).Find ((Thing x) => x is Pokemon_Magnemite);
					if (thing != null) {

						if (GenSight.LineOfSight (Position, intVec, Map, false, null, 0, 0)) {

							var pDummy = (Pokemon_Magnemite)thing;
							var d = thing.def.defName;

							if (d.Equals ("Pokemon_Magnemite") && pDummy.ClosestCompPower != null && pDummy != this) {

								magnemitesInRange.Add (pDummy);

							}

							if (magnemitesInRange.Count == 3) {

								break;

							}
						}
					}
				}
			}

			if (magnemitesInRange.Count == 3) {

				var mag = this;

				foreach (var m in magnemitesInRange) {

					mag = m;

					if (m.Faction != null) {

						break;

					}
				}

				var ton = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magneton"));

				if (mag.Faction != null) {

					if (mag.playerSettings.Master != null) {
						ton.SetFaction (mag.Faction, mag.playerSettings.Master);
						ton.training.Train (TrainableUtility.TrainableDefsInListOrder [0], mag.playerSettings.Master);
					} else {
						ton.SetFaction (mag.Faction, mag.Faction.leader); //should not happen, just for debugging purposes
					}

					if (!mag.Name.ToString ().Split (' ') [0].Equals ("magnemite")) {
						ton.Name = mag.Name;
					}
				}

				foreach (var m in magnemitesInRange) {

					if (m.Spawned && m != this) {
						m.DeSpawn ();
					}
				}

				if (Spawned) {

					GenSpawn.Spawn (ton, mag.Position, Map);
					DeSpawn ();
				}

			}

		}
			
		public override void Repairing() {

			RepairJob.HitPoints++;
			StoredEnergy--;
			Map.overlayDrawer.DrawOverlay(RepairJob, OverlayTypes.BurningWick);

		}
	}
}