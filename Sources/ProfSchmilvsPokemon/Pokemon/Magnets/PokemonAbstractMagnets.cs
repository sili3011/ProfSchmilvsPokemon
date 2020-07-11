using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ProfSchmilvsPokemon.Pokemon.Magnets
{
	[StaticConstructorOnStartup]
	public abstract class PokemonAbstractMagnets : PokemonAbstract
	{

		protected static readonly Vector2 BarSize = new Vector2(0.6f, 0.05f);
		protected static readonly Material BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f), false);
		protected static readonly Material BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
		protected float StoredEnergy = 250f;
		protected CompPower ClosestCompPower;
		protected Thing RepairJob;
		protected float StoredEnergyMax;

		public override void Draw()
		{
			base.Draw();

			if (Faction != null) {

				GenDraw.FillableBarRequest r = default;
				r.center = DrawPos + Vector3.up * 0.1f - new Vector3 (0, 0, 0.5f);
				r.size = BarSize;
				r.fillPercent = StoredEnergy / StoredEnergyMax;
				r.filledMat = BatteryBarFilledMat;
				r.unfilledMat = BatteryBarUnfilledMat;
				r.margin = 0.15f;
				GenDraw.DrawFillableBar (r);

			}
		}

		public override void Tick()
		{

			base.Tick ();

			if (StoredEnergy > 0) {

				StoredEnergy -= 0.01f;

			} else {

				if (!Downed) {

					Hediff hediff = HediffMaker.MakeHediff(DefOfs.HediffDefOf.OutOfPower, this);
					health.AddHediff (hediff);

				}

			}

			if (ClosestCompPower == null && StoredEnergy >= StoredEnergyMax*0.21f && Faction != null){

				if (RepairJob == null) {

					var toRepair = Map.listerBuildingsRepairable.RepairableBuildings(Faction);

					if (!toRepair.NullOrEmpty ()) {

						RepairJob = toRepair[0];
						jobs.ClearQueuedJobs();
						pather.StartPath (new LocalTargetInfo (RepairJob.Position), Verse.AI.PathEndMode.Touch);

					}

				} else {

					if (RepairJob.MaxHitPoints == RepairJob.HitPoints) {

						Map.listerBuildingsRepairable.Notify_BuildingRepaired((Building)RepairJob);
						RepairJob = null;
						jobs.ClearQueuedJobs();

					} else {

						if (Position.AdjacentTo8WayOrInside(RepairJob.Position)) {

							Repairing ();

						} else if (pather.Destination != new LocalTargetInfo(RepairJob.Position)) {

							pather.StartPath (new LocalTargetInfo(RepairJob.Position), PathEndMode.Touch);

						}
					}
				}
			}

			if (ClosestCompPower != null && StoredEnergy >= StoredEnergyMax) {
				ClosestCompPower = null;
				jobs.ClearQueuedJobs();
			}

			if (Downed) {

				if (StoredEnergy < StoredEnergyMax * 0.2f) {

					var dummylist = Spawner.spawnerPokemon.PowerComps;

					if (!dummylist.NullOrEmpty ()) {

						var distance = 999999f;

						foreach (CompPower cp in dummylist) {

							var compareDistance = Position.DistanceTo (cp.parent.Position);

							if (compareDistance < distance) {

								distance = compareDistance;
								ClosestCompPower = cp;

							}
						}
					}
				}

				if (Position.AdjacentTo8WayOrInside (ClosestCompPower.parent.Position)) {

					var pn = ClosestCompPower.PowerNet;

					if (pn != null) {

						var currentEnergy = pn.CurrentStoredEnergy ();

						if (currentEnergy >= 0.5f) {

							var cpb = pn.batteryComps;
                            CompPowerBattery bat = null;

							foreach (CompPowerBattery c in cpb) {

								if (c.StoredEnergy >= 0.5f) {
									bat = c;
									break;
								}
							}

							if (bat != null) {
								StoredEnergy += 0.5f;
								bat.DrawPower (0.5f);
							}
						}
					}
				}

				if (StoredEnergy > StoredEnergyMax * 0.2f) {

					var heds = health.hediffSet.hediffs;

					if (!heds.NullOrEmpty ()) {

						Hediff oop = null;

						foreach (var h in heds) {

							if (h.def.Equals (DefOfs.HediffDefOf.OutOfPower)) {
								oop = h;
							}
						}

						if (oop != null) {
							health.RemoveHediff (oop);
						}
					}
				}

			} else {

				if (ClosestCompPower == null) {

					if (StoredEnergy > 0 && StoredEnergy < StoredEnergyMax * 0.2f) {

						var dummylist = Spawner.spawnerPokemon.PowerComps;

						if (!dummylist.NullOrEmpty ()) {

							float distance = 999999f;

							foreach (var cp in dummylist) {

								var compareDistance = Position.DistanceTo(cp.parent.Position);

								if (compareDistance < distance) {
									distance = compareDistance;
									ClosestCompPower = cp;
								}
							}		
							pather.StartPath (new LocalTargetInfo (ClosestCompPower.parent.Position), PathEndMode.OnCell);
						}
					}

				} else {

					if (Position.AdjacentTo8WayOrInside (ClosestCompPower.parent.Position)) {

						PowerNet pn = ClosestCompPower.PowerNet;

						if (pn != null) {

							var currentEnergy = pn.CurrentStoredEnergy ();

							if (currentEnergy >= 0.5f) {

								var cpb = pn.batteryComps;
								CompPowerBattery bat = null;

								foreach (CompPowerBattery c in cpb) {

									if (c.StoredEnergy >= 0.5f) {
										bat = c;
										break;
									}
								}

								if (bat != null) {
									StoredEnergy += 0.5f;
									bat.DrawPower (0.5f);
									Evolve ();
                                }
							}
						}

					} else {

						if (pather.Destination != new LocalTargetInfo (this.ClosestCompPower.parent.Position)) {
                            jobs.ClearQueuedJobs();
							pather.StartPath (new LocalTargetInfo (this.ClosestCompPower.parent.Position), Verse.AI.PathEndMode.Touch);
                        }
					}
				}
			}
		}

		public virtual void Evolve() {}

		public virtual void Repairing() {}

		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look(ref this.StoredEnergy, "storedEnergy", this.StoredEnergyMax);
			Scribe_Deep.Look(ref this.RepairJob, "RepairJob", new object[0]);
			Scribe_Values.Look(ref this.StoredEnergyMax, "StoredEnergyMax", 0f);
		}

	}
}