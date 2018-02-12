using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI;
using RimWorld;

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

				if (!this.Downed && this.Faction != null) {

					Hediff hediff = HediffMaker.MakeHediff(ProfSchmilvsPokemon.HediffDefOf.OutOfPower, this);
					this.health.AddHediff (hediff);

				}
			
			}

			if (closestCompPower != null && this.StoredEnergy >= this.Def.storedEnergyMaxUtility) {

				closestCompPower = null;
				this.jobs.ClearQueuedJobs();
			
			}

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

				if (this.Position.IsInside (this.closestCompPower.parent) || this.Position.AdjacentTo8Way (this.closestCompPower.parent.Position)) {

					PowerNet pn = this.closestCompPower.PowerNet;
					if(pn != null){
						float currentEnergy = pn.CurrentStoredEnergy ();
						if (currentEnergy >= 0.5f) {	
							PawnUtility.ForceWait (this, 5);
							this.StoredEnergy += 0.5f;
							pn.batteryComps [0].DrawPower (0.5f);
						}
					}

				} else {

					if (this.pather.Destination != new LocalTargetInfo (this.closestCompPower.parent.Position)) {

						new LocalTargetInfo (this.closestCompPower.parent.Position);

					}

				}

			}

		}

		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.StoredEnergy, "storedEnergy", 0f);
		}
			
		private static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		private float StoredEnergy = 250f;
		private CompPower closestCompPower = null;
	}
}