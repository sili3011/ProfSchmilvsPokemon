using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon
{
	
	[StaticConstructorOnStartup]
	public class CompPowerPlantMareep : CompPower
	{

		public float StoredMareepPower;
		public float MaxStoredMareepPower = 20000f;
		private readonly Vector2 _barSize = new Vector2(2.3f, 0.14f);
		private readonly Material _powerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f), false);
		private readonly Material _powerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f), false);

		public override void PostDraw()
		{
			base.PostDraw();
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = this.parent.DrawPos + Vector3.up * 0.1f;
			r.size = this._barSize;
			r.fillPercent = this.StoredMareepPower / this.MaxStoredMareepPower;
			r.filledMat = this._powerPlantSolarBarFilledMat;
			r.unfilledMat = this._powerPlantSolarBarUnfilledMat;
			r.margin = 0.15f;
			var rotation = this.parent.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
		}

		public override void CompTick(){

			base.CompTick ();

			if (!this.PowerNet.batteryComps.NullOrEmpty()) {

				var bat = this.PowerNet.batteryComps.RandomElement ();

				if (this.StoredMareepPower > 0.5f && bat.AmountCanAccept >= 0.5f) {

					bat.AddEnergy (0.5f);
					this.StoredMareepPower -= 0.5f;

				}

			}

		}
			
		public void AddToStoredPower(float discharge){

			this.StoredMareepPower += discharge;

		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.StoredMareepPower, "StoredMareepPower", 0f, false);
			if (this.StoredMareepPower > this.MaxStoredMareepPower)
			{
				this.StoredMareepPower = this.MaxStoredMareepPower;
			}
		}

	}
}
