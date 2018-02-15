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
	public class Pokemon_Magnemite : Pawn
	{

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

		public override void Draw()
		{
			base.Draw();
			if (this.Faction != null) {
				GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
				r.center = this.DrawPos + Vector3.up * 0.1f - new Vector3 (0, 0, 0.5f);
				r.size = Pokemon_Magnemite.BarSize;
				r.fillPercent = this.StoredEnergy / this.Def.storedEnergyMaxUtility;
				r.filledMat = Pokemon_Magnemite.BatteryBarFilledMat;
				r.unfilledMat = Pokemon_Magnemite.BatteryBarUnfilledMat;
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

										foreach(Pokemon_Magnemite m in magnemitesInRange){

											mag = m;

											if (m.Faction != null) {

												break;
											
											}
												
										}

										Pawn ton = PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Magneton"));
										if (mag.Faction != null) {
											if (mag.playerSettings.master != null) {
												ton.SetFaction (mag.Faction, (Pawn)mag.playerSettings.master);
												ton.training.Train (TrainableUtility.TrainableDefsInListOrder [0], (Pawn)mag.playerSettings.master);
											} else {
												ton.SetFaction (mag.Faction, (Pawn)mag.Faction.leader); //should not happen, just for debugging purposes
											}
											if (!mag.Name.ToString().Split(' ')[0].Equals ("magnemite")) {
												ton.Name = mag.Name;
											}
										}

										foreach(Pokemon_Magnemite m in magnemitesInRange){

											if (m.Spawned && m != this) {
												m.DeSpawn ();
											}

										}

										if (this.Spawned) {

											GenSpawn.Spawn (ton, mag.Position, base.Map);
											this.DeSpawn ();
										}

									}
								}
							}
						}
					} else {

						if (this.pather.Destination != new LocalTargetInfo (this.closestCompPower.parent.Position)) {

							this.jobs.ClearQueuedJobs();
							this.pather.StartPath (new LocalTargetInfo (this.closestCompPower.parent.Position), Verse.AI.PathEndMode.Touch);

						}

					}

				}

			}

		}

		public virtual void repairing(){
		
			this.repairJob.HitPoints++;
			this.StoredEnergy--;
			base.Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);
		
		}
			
		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.StoredEnergy, "storedEnergy", 250f);
			Scribe_Deep.Look<Thing>(ref this.repairJob, "repairJob", new object[0]);
		}
			
		private static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		private float StoredEnergy = 250f;
		private CompPower closestCompPower = null;
		public Thing repairJob = null;

	}
}