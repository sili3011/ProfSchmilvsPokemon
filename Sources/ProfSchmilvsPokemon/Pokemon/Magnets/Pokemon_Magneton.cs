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
	public class Pokemon_Magneton : Pokemon_Abstract_Magnets
	{

		public Pokemon_Magneton(){
			base.storedEnergyMax = this.Def.storedEnergyMaxUtility;
		}

		#region Properties
		//
		public ThingDef_Magneton Def
		{
			get 
			{
				return this.def as ThingDef_Magneton;
			}
		}
		//
		#endregion Properties

		public override void Draw()
		{
			base.Draw();
			if (this.Faction != null) {
				GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
				r.center = this.DrawPos + Vector3.up * 0.1f - new Vector3 (0, 0, 0.5f);
				r.size = Pokemon_Magneton.BarSize;
				r.fillPercent = this.StoredEnergy / this.Def.storedEnergyMaxUtility;
				r.filledMat = Pokemon_Magneton.BatteryBarFilledMat;
				r.unfilledMat = Pokemon_Magneton.BatteryBarUnfilledMat;
				r.margin = 0.15f;
				GenDraw.DrawFillableBar (r);
			}
			/*if (false)
			{
				base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
			}*/
		}

		public override void evolve(){

			float bonusMagnetism = 0f;

			for (int i = 0; i < 400; i++) {

				IntVec3 intVec = this.Position + GenRadial.RadialPattern [i];
				if (intVec.InBounds (base.Map)) {

					Thing thing = intVec.GetThingList (base.Map).Find ((Thing x) => x is Pawn);
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

				Pawn zone = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magnezone"));
				if (this.Faction != null) {
					if (this.playerSettings.master != null) {
						zone.SetFaction (this.Faction, (Pawn)this.playerSettings.master);
						zone.training.Train (TrainableUtility.TrainableDefsInListOrder [0], (Pawn)this.playerSettings.master);
					} else {
						zone.SetFaction (this.Faction, (Pawn)this.Faction.leader); //should not happen, just for debugging purposes
					}
					if (!this.Name.ToString().Split(' ')[0].Equals ("magneton")) {
						zone.Name = this.Name;
					}
				}

				GenSpawn.Spawn (zone, this.Position, base.Map);
				this.DeSpawn ();

			}

		}


		public override void repairing(){

			this.repairJob.HitPoints += 3;
			this.StoredEnergy--;
			base.Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);

		}
	}
}