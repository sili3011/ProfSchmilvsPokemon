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
	public class Pokemon_Magnemite : Pokemon_Abstract_Magnets
	{

		public Pokemon_Magnemite(){
			base.storedEnergyMax = this.Def.storedEnergyMaxUtility;
		}

		#region Properties
		//
		public ThingDef_Magnemite Def
		{
			get 
			{
				return this.def as ThingDef_Magnemite;
			}
		}
		//
		#endregion Properties

		public override void evolve(){

			List<Pokemon_Magnemite> magnemitesInRange = new List<Pokemon_Magnemite>();
			magnemitesInRange.Add (this);

			for (int i = 0; i < 25; i++) {

				IntVec3 intVec = this.Position + GenRadial.RadialPattern [i];
				if (intVec.InBounds (base.Map)) {

					Thing thing = intVec.GetThingList (base.Map).Find ((Thing x) => x is Pokemon_Magnemite);
					if (thing != null) {

						if (GenSight.LineOfSight (this.Position, intVec, base.Map, false, null, 0, 0)) {

							Pokemon_Magnemite pDummy = (Pokemon_Magnemite)thing;
							string d = thing.def.defName.ToString ();

							if (d.Equals ("Pokemon_Magnemite") && pDummy.closestCompPower != null && pDummy != this) {

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

				Pokemon_Magnemite mag = this;

				foreach (Pokemon_Magnemite m in magnemitesInRange) {

					mag = m;

					if (m.Faction != null) {

						break;

					}

				}

				Pawn ton = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magneton"));
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

				foreach (Pokemon_Magnemite m in magnemitesInRange) {

					if (m.Spawned && m != this) {
						m.DeSpawn ();
					}

				}

				if (this.Spawned) {

					GenSpawn.Spawn (ton, mag.Position, Map);
					this.DeSpawn ();
				}

			}

		}
			
		public override void repairing(){
		
			repairJob.HitPoints++;
			StoredEnergy--;
			Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);
		
		}
	}
}