using System.Collections.Generic;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ProfSchmilvsPokemon
{
	[StaticConstructorOnStartup]
	public class Pokemon_Mareep : PokemonAbstract
	{

		#region Properties
		//
		public ThingDef_Mareep Def
		{
			get {
				return def as ThingDef_Mareep;
			}
		}
		//
		#endregion Properties

		public Sustainer WickSustainer;
		private static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		private static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		private static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		private float _storedEnergy;
		private Building _closestDischarger;

		public override void Draw()
		{
			base.Draw();

			if (ageTracker.CurLifeStage.defName.Equals("MareepStage")) {

				if (Faction != null) {
					GenDraw.FillableBarRequest r = default;
					r.center = DrawPos + Vector3.up * 0.1f - new Vector3(0, 0, 0.5f);
					r.size = BarSize;
					r.fillPercent = _storedEnergy / Def.storedEnergyMaxUtility;
					r.filledMat = BatteryBarFilledMat;
					r.unfilledMat = BatteryBarUnfilledMat;
					r.margin = 0.15f;
					GenDraw.DrawFillableBar(r);
				}

				if (_storedEnergy < Def.storedEnergyMaxUtility && Spawned) {
					Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
				}
			}
		}

		public override void Tick() {

			base.Tick ();

			if (ageTracker.CurLifeStage.defName.Equals("MareepStage")) {

				var mareepCount = 1;

				for (int i = 0; i < 10; i++) {

					var intVec = Position + GenRadial.RadialPattern [i];
					if (intVec.InBounds (Map)) {

						Thing thing = intVec.GetThingList(Map).Find((Thing x) => x is Pawn);
						if (thing != null) {

							if (GenSight.LineOfSight(this.Position, intVec, Map, false, null, 0, 0)) {

								var pDummy = (Pawn)thing;
								var d = thing.def.defName.ToString ();

								if (d.Equals("Pokemon_Mareep") && pDummy.ageTracker.CurLifeStage.defName.ToString().Equals("MareepStage")) {

									++mareepCount;

								}
							}
						}
					}
				}

				_storedEnergy += (mareepCount / 40) * Rand.Value;

			}

			if (Faction != null) {

				if (_storedEnergy <= Def.storedEnergyMaxUtility * 0.05f) {

					_closestDischarger = null;
					jobs.ClearQueuedJobs();

				}

				if (_storedEnergy >= Def.storedEnergyMaxUtility) {

					if (_closestDischarger == null) {

						for (int i = 0; i < 1200; i++) {

							var intVec = Position + GenRadial.RadialPattern [i];
							if (intVec.InBounds (Map)) {

								var thing = intVec.GetThingList (Map).Find ((Thing x) => x is Building);
								if (thing != null) {

									if (GenSight.LineOfSight (Position, intVec, Map, false, null, 0, 0)) {

										var closest = (Building)thing;
										var splt = closest.def.defName.Split('_');

										if (splt.Length == 2 && splt [0].Equals ("Mareep")) {
											//Log.Message ("Found discharging station");
											_closestDischarger = closest;
											pather.StartPath (new LocalTargetInfo (_closestDischarger), Verse.AI.PathEndMode.OnCell);
											break;
										}
									}
								}
							}
						}
					} else {
						Log.Message("No discharging station found");
					}
				}

				if (_closestDischarger != null) {

					if (Position.AdjacentTo8WayOrInside (_closestDischarger)) {
						PawnUtility.ForceWait (this, 5);
						List<ThingComp> lt = _closestDischarger.AllComps;
						CompPowerPlantMareep comp = (CompPowerPlantMareep)lt [0];
						comp.AddToStoredPower (1f);
						_storedEnergy -= 1f;
					}
				}

			} else {

				if (_storedEnergy >= Def.storedEnergyMaxUtility * 0.1f) {
					_storedEnergy -= 0.1f * Rand.Value;
				}
			}

			if(_storedEnergy > Def.storedEnergyMaxUtility + Def.storedEnergyMaxUtility*0.2f) {
				
				var randomCell = this.OccupiedRect().RandomCell;
				var radius = Rand.Range (0.5f, 1f) * 3f;
				GenExplosion.DoExplosion (randomCell, Map, radius, DamageDefOf.EMP, null, -1, 0f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
				_storedEnergy -= 800f;

			}
		}

		private void StartWickSustainer()
		{
			var info = SoundInfo.InMap(this, MaintenanceType.PerTick);
			WickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
		}

		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look(ref _storedEnergy, "storedEnergy", 0f);
			Scribe_Deep.Look(ref _closestDischarger, "_closestDischarger", null);
		}
	}
}