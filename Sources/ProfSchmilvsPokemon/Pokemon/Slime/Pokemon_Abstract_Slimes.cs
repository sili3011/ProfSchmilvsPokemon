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
	public class Pokemon_Abstract_Slimes : Pokemon_Abstract
	{

		public override void Tick()
		{
			base.Tick ();
		}

		public void IncrementFilth()
		{
			++this.amountOfFilth;
			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 3600000L * 150L;
		}

		public void DecrementFilth()
		{
			--this.amountOfFilth;
			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 3600000L * 150L;
		}

		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.amountOfFilth, "amountOfFilth", 0f);
			Scribe_Deep.Look<Zone>(ref this.currentDump, "currentDump", null);
			Scribe_Deep.Look<Thing>(ref this.digesting, "digesting", null);
			Scribe_Values.Look<long>(ref this.digestingTicks, "digestingTicks", 0L);
		}

		public float amountOfFilth = 0f;
		protected int currentTick = 0;
		protected Zone currentDump = null;
		protected Thing digesting = null;
		protected long digestingTicks = 0L;

	}

}