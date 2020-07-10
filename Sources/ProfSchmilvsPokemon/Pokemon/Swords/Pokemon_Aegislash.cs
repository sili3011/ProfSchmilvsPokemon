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
	public class Pokemon_Aegislash : Pokemon_Abstract_Swords
	{

		#region Properties
		//
		public ThingDef_Aegislash Def
		{
			get 
			{
				return this.def as ThingDef_Aegislash;
			}
		}
		//
		#endregion Properties

		public override void evolve(){}

	}
}