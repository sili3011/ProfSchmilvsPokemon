using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon
{
	
	[StaticConstructorOnStartup]
	public class CompPowerPlantMareep : CompPowerPlant
	{
		
		protected override float DesiredPowerOutput
		{
			get
			{
				return this.storedMareepPower;
			}
		}
			
		public override void PostDraw()
		{
			base.PostDraw();
			GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
			r.center = this.parent.DrawPos + Vector3.up * 0.1f;
			r.size = CompPowerPlantMareep.BarSize;
			r.fillPercent = this.storedMareepPower / this.MaxStoredMareepPower;
			r.filledMat = CompPowerPlantMareep.PowerPlantSolarBarFilledMat;
			r.unfilledMat = CompPowerPlantMareep.PowerPlantSolarBarUnfilledMat;
			r.margin = 0.15f;
			Rot4 rotation = this.parent.Rotation;
			rotation.Rotate(RotationDirection.Clockwise);
			r.rotation = rotation;
			GenDraw.DrawFillableBar(r);
		}
			
		public void addToStoredPower(float discharge){

			this.storedMareepPower += discharge;

		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.storedMareepPower, "storedMareepPower", 0f, false);
			if (this.storedMareepPower > this.MaxStoredMareepPower)
			{
				this.storedMareepPower = this.MaxStoredMareepPower;
			}
		}

		public float storedMareepPower;
		public float MaxStoredMareepPower = 50000f;
		private static readonly Vector2 BarSize = new Vector2(2.3f, 0.14f);
		private static readonly Material PowerPlantSolarBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f), false);
		private static readonly Material PowerPlantSolarBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f), false);
	}
}
