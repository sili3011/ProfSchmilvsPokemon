﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	[StaticConstructorOnStartup]
	public class Pokemon_Mareep : Pokemon_Abstract
	{

		#region Properties
		//
		public ThingDef_Mareep Def
		{
			get 
			{
				return this.def as ThingDef_Mareep;
			}
		}
		//
		#endregion Properties

		public override void Draw()
		{
			base.Draw();
			if(this.ageTracker.CurLifeStage.defName.ToString().Equals("MareepStage")){
				if (this.Faction != null) {
					GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
					r.center = this.DrawPos + Vector3.up * 0.1f - new Vector3 (0, 0, 0.5f);
					r.size = Pokemon_Mareep.BarSize;
					r.fillPercent = this.StoredEnergy / this.Def.storedEnergyMaxUtility;
					r.filledMat = Pokemon_Mareep.BatteryBarFilledMat;
					r.unfilledMat = Pokemon_Mareep.BatteryBarUnfilledMat;
					r.margin = 0.15f;
					GenDraw.DrawFillableBar (r);
				}
				if (this.StoredEnergy < this.Def.storedEnergyMaxUtility && base.Spawned)
				{
					base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
				}
			}
		}

		public override void Tick()
		{

			base.Tick ();

			if (this.ageTracker.CurLifeStage.defName.ToString ().Equals ("MareepStage")) {

				float mareepCount = 1;

				for (int i = 0; i < 10; i++) {

					IntVec3 intVec = this.Position + GenRadial.RadialPattern [i];
					if (intVec.InBounds (base.Map)) {

						Thing thing = intVec.GetThingList (base.Map).Find ((Thing x) => x is Pawn);
						if (thing != null) {

							if (GenSight.LineOfSight (this.Position, intVec, base.Map, false, null, 0, 0)) {

								Pawn pDummy = (Pawn)thing;
								string d = thing.def.defName.ToString ();

								if (d.Equals ("Pokemon_Mareep") && pDummy.ageTracker.CurLifeStage.defName.ToString().Equals("MareepStage")) {

									++mareepCount;

								}

							}

						}

					}

				}

				this.StoredEnergy += (mareepCount/40) * Rand.Value;

			}

			if (this.Faction != null) {

				if (this.StoredEnergy <= this.Def.storedEnergyMaxUtility * 0.05f) {

					this.closestDischarger = null;
					this.jobs.ClearQueuedJobs();

				}

				if (this.StoredEnergy >= this.Def.storedEnergyMaxUtility) {

					if (this.closestDischarger == null) {

						for (int i = 0; i < 1200; i++) {

							IntVec3 intVec = this.Position + GenRadial.RadialPattern [i];
							if (intVec.InBounds (base.Map)) {

								Thing thing = intVec.GetThingList (base.Map).Find ((Thing x) => x is Building);
								if (thing != null) {

									if (GenSight.LineOfSight (this.Position, intVec, base.Map, false, null, 0, 0)) {

										Building closest = (Building)thing;
										string d = closest.def.defName.ToString ();
										string[] splt = d.Split ('_');

										//Log.Message (d);

										if (splt.Length == 2 && splt [0].Equals ("Mareep")) {

											//Log.Message ("Found discharging station");
											this.closestDischarger = closest;
											this.pather.StartPath (new LocalTargetInfo (this.closestDischarger), Verse.AI.PathEndMode.OnCell);
											break;

										}
								
									}

								}

							}

						}

					} else {
					
						Log.Message ("No discharging station found");
					
					}

				}

				if (this.closestDischarger != null) {

					if (this.Position.AdjacentTo8WayOrInside (this.closestDischarger)) {

						PawnUtility.ForceWait (this, 5);
						List<ThingComp> lt = this.closestDischarger.AllComps;
						CompPowerPlantMareep comp = (CompPowerPlantMareep)lt [0];
						comp.AddToStoredPower (1f);
						this.StoredEnergy -= 1f;

					}

				}

			} else {
			
				if (this.StoredEnergy >= this.Def.storedEnergyMaxUtility * 0.1f) {

					this.StoredEnergy -= 0.1f * Rand.Value;

				}
			
			}

			if(this.StoredEnergy > this.Def.storedEnergyMaxUtility + this.Def.storedEnergyMaxUtility*0.2f){
				
				IntVec3 randomCell = this.OccupiedRect ().RandomCell;
				float radius = Rand.Range (0.5f, 1f) * 3f;
				GenExplosion.DoExplosion (randomCell, base.Map, radius, DamageDefOf.EMP, null, -1, 0f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
				this.StoredEnergy -= 800f;

			}

		}

		private void StartWickSustainer()
		{
			SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
			this.wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
		}

		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.StoredEnergy, "storedEnergy", 0f);
			Scribe_Deep.Look<Building>(ref this.closestDischarger, "closestDischarger", null);
		}
			
		public Sustainer wickSustainer;
		private static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		private float StoredEnergy = 0f;
		private Building closestDischarger = null;

	}
}