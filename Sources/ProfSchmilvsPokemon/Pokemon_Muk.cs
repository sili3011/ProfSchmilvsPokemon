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
	public class Pokemon_Muk : Pawn
	{

		#region Properties
		//
		public ThingDef_Muk Def
		{
			get 
			{
				return this.def as ThingDef_Muk;
			}
		}
		//
		#endregion Properties

		public override void Draw()
		{
			base.Draw();
		}

		public override void Tick()
		{
			base.Tick ();

		}

		public void IncrementFilth()
		{
			++this.amountOfFilth;
			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 20000L * 365L * 150L;
		}
			
		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.amountOfFilth, "amountOfFilth", 0f);
		}

		public float amountOfFilth = 0f;

	}
}