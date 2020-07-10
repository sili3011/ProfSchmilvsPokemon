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
	public abstract class Pokemon_Abstract_Swords : Pokemon_Abstract
	{

		public override void Draw()
		{

		}

		public override void Tick()
		{

			base.Tick ();

		}

		public virtual void evolve(){}

		public override void ExposeData()
		{
			base.ExposeData ();
		}

	}
}