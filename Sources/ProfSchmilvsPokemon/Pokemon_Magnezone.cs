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
	[StaticConstructorOnStartup]
	public class Pokemon_Magnezone : Pawn
	{

		#region Properties
		//
		public ThingDef_Magnezone Def
		{
			get 
			{
				return this.def as ThingDef_Magnezone;
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
				r.size = Pokemon_Magnezone.BarSize;
				r.fillPercent = this.StoredEnergy / this.Def.storedEnergyMaxUtility;
				r.filledMat = Pokemon_Magnezone.BatteryBarFilledMat;
				r.unfilledMat = Pokemon_Magnezone.BatteryBarUnfilledMat;
				r.margin = 0.15f;
				GenDraw.DrawFillableBar (r);
			}
			/*if (false)
			{
				base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
			}*/
		}

		public override void Tick()
		{

			base.Tick ();

			if (this.StoredEnergy > 0) {
			
				this.StoredEnergy -= 0.01f;
			
			} else {

				if (!this.Downed) {

					Hediff hediff = HediffMaker.MakeHediff(ProfSchmilvsPokemon.DefOfs.HediffDefOf.OutOfPower, this);
					this.health.AddHediff (hediff);

				}
			
			}

			if (closestCompPower == null && this.StoredEnergy >= this.Def.storedEnergyMaxUtility*0.21f && this.Faction != null){

				if (this.repairJob == null) {

					List<Thing> toRepair = base.Map.listerBuildingsRepairable.RepairableBuildings (this.Faction);

					if (!toRepair.NullOrEmpty ()) { 

						this.repairJob = toRepair[0];
						this.jobs.ClearQueuedJobs();
						this.pather.StartPath (new LocalTargetInfo (this.repairJob.Position), Verse.AI.PathEndMode.Touch);
	
					}

				} else {

					if (this.repairJob.MaxHitPoints == this.repairJob.HitPoints) {

						base.Map.listerBuildingsRepairable.Notify_BuildingRepaired ((Building)this.repairJob);
						this.repairJob = null;
						this.jobs.ClearQueuedJobs();

					} else {
				
						if (this.Position.AdjacentTo8WayOrInside (this.repairJob.Position)) {

							this.repairing ();

						} else if (this.pather.Destination != new LocalTargetInfo (this.repairJob.Position)) {
							
							this.pather.StartPath (new LocalTargetInfo (this.repairJob.Position), Verse.AI.PathEndMode.Touch);

						}

					}
				
				}

			}
				
			if (closestCompPower != null && this.StoredEnergy >= this.Def.storedEnergyMaxUtility) {

				closestCompPower = null;
				this.jobs.ClearQueuedJobs();
			
			}

			if (this.Downed) {

				if (this.StoredEnergy < this.Def.storedEnergyMaxUtility * 0.2f) {

					List<CompPower> dummylist = Spawner.spawnerPokemon.PowerComps;

					if (!dummylist.NullOrEmpty ()) {

						float distance = 999999f;

						foreach (CompPower cp in dummylist) {

							float compareDistance = this.Position.DistanceTo (cp.parent.Position);

							if (compareDistance < distance) {

								distance = compareDistance;
								closestCompPower = cp;

							}

						}		

					}

				}

				if (this.Position.AdjacentTo8WayOrInside (this.closestCompPower.parent.Position)) {
					PowerNet pn = this.closestCompPower.PowerNet;
					if (pn != null) {
						float currentEnergy = pn.CurrentStoredEnergy ();
						if (currentEnergy >= 0.5f) {
							List<CompPowerBattery> cpb = pn.batteryComps;
							CompPowerBattery bat = null;
							foreach (CompPowerBattery c in cpb) {
								if (c.StoredEnergy >= 0.5f) {
									bat = c;
									break;
								}
							}
							if (bat != null) {
								this.StoredEnergy += 0.5f;
								bat.DrawPower (0.5f);
							}
						}
					}
				}

				if (this.StoredEnergy > this.Def.storedEnergyMaxUtility * 0.2f) {

					List<Hediff> heds = this.health.hediffSet.hediffs;
					if (!heds.NullOrEmpty ()) {
						Hediff oop = null;
						foreach (Hediff h in heds) {
							if (h.def.Equals (ProfSchmilvsPokemon.DefOfs.HediffDefOf.OutOfPower)) {
								oop = h;
							}
						}
						if (oop != null) {
							this.health.RemoveHediff (oop);
						}
					}
				}

			} else {
				
				if (closestCompPower == null) {

					if (this.StoredEnergy > 0 && this.StoredEnergy < this.Def.storedEnergyMaxUtility * 0.2f) {

						List<CompPower> dummylist = Spawner.spawnerPokemon.PowerComps;

						if (!dummylist.NullOrEmpty ()) {

							float distance = 999999f;

							foreach (CompPower cp in dummylist) {

								float compareDistance = this.Position.DistanceTo (cp.parent.Position);

								if (compareDistance < distance) {

									distance = compareDistance;
									closestCompPower = cp;

								}

							}		

							this.pather.StartPath (new LocalTargetInfo (this.closestCompPower.parent.Position), Verse.AI.PathEndMode.OnCell);

						}

					}
					
				} else {

					if (this.Position.AdjacentTo8WayOrInside (this.closestCompPower.parent.Position)) {
						PowerNet pn = this.closestCompPower.PowerNet;
						if (pn != null) {
							float currentEnergy = pn.CurrentStoredEnergy ();
							if (currentEnergy >= 2.5f) {
								List<CompPowerBattery> cpb = pn.batteryComps;
								CompPowerBattery bat = null;
								foreach (CompPowerBattery c in cpb) {
									if (c.StoredEnergy >= 2.5f) {
										bat = c;
										break;
									}
								}
								if (bat != null) {
									this.StoredEnergy += 2.5f;
									bat.DrawPower (2.5f);
								}
							}
						}
					} else {

						if (this.pather.Destination != new LocalTargetInfo (this.closestCompPower.parent.Position)) {

							this.pather.StartPath (new LocalTargetInfo (this.closestCompPower.parent.Position), Verse.AI.PathEndMode.Touch);

						}

					}

				}

			}

		}

		public virtual void repairing(){
		
			this.repairJob.HitPoints += 5;
			this.StoredEnergy--;
			base.Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);
		
		}
			
		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.StoredEnergy, "storedEnergy", 1250f);
			Scribe_Deep.Look<Thing>(ref this.repairJob, "repairJob", new object[0]);
		}
			
		private static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		private float StoredEnergy = 1250f;
		private CompPower closestCompPower = null;
		public Thing repairJob = null;

	}
}