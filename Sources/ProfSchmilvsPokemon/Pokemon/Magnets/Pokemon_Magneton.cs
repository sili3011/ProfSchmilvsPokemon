using ProfSchmilvsPokemon.Pokemon.Magnets;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;
using UnityEngine;
using Verse;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Magneton : PokemonAbstractMagnets {

		#region Properties
		//
		public ThingDef_Magneton Def
		{
			get {
				return def as ThingDef_Magneton;
			}
		}
		//
		#endregion Properties

		public Pokemon_Magneton() {
			StoredEnergyMax = this.Def.storedEnergyMaxUtility;
		}

		public override void Draw() {

			base.Draw();
			if (this.Faction != null) {
				GenDraw.FillableBarRequest r = default;
				r.center = this.DrawPos + Vector3.up * 0.1f - new Vector3 (0, 0, 0.5f);
				r.size = BarSize;
				r.fillPercent = this.StoredEnergy / this.Def.storedEnergyMaxUtility;
				r.filledMat = BatteryBarFilledMat;
				r.unfilledMat = BatteryBarUnfilledMat;
				r.margin = 0.15f;
				GenDraw.DrawFillableBar (r);
			}
			/*if (false)
			{
				base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
			}*/
		}

		public override void Evolve(){

			float bonusMagnetism = 0f;

			for (int i = 0; i < 400; i++) {

				var intVec = this.Position + GenRadial.RadialPattern [i];
				if (intVec.InBounds (Map)) {

					Thing thing = intVec.GetThingList (Map).Find((Thing x) => x is Pawn);
					if (thing != null) {

						if (thing is Pokemon_Magnemite) {

							bonusMagnetism += 25f;

						}else if (thing is Pokemon_Magneton) {

							bonusMagnetism += 75f;

						}else if (thing is Pokemon_Magnezone) {

							bonusMagnetism += 125f;

						}
					}
				}
			}

			if ((Spawner.spawnerPokemon.powerBuildingCount + bonusMagnetism)/1000 > Rand.Value) {

				var zone = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magnezone"));
				if (this.Faction != null) {
					if (this.playerSettings.Master != null) {
						zone.SetFaction (this.Faction, this.playerSettings.Master);
						zone.training.Train (TrainableUtility.TrainableDefsInListOrder [0], this.playerSettings.Master);
					} else {
						zone.SetFaction (this.Faction, this.Faction.leader); //should not happen, just for debugging purposes
					}
					if (!this.Name.ToString().Split(' ')[0].Equals ("magneton")) {
						zone.Name = this.Name;
					}
				}

				GenSpawn.Spawn (zone, this.Position, Map);
				this.DeSpawn ();

			}
		}

		public override void Repairing(){
			this.RepairJob.HitPoints += 3;
			this.StoredEnergy--;
			Map.overlayDrawer.DrawOverlay(this.RepairJob, OverlayTypes.BurningWick);
		}
	}
}