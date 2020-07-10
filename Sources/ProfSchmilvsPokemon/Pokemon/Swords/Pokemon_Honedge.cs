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
	public class Pokemon_Honedge : Pokemon_Abstract_Swords
	{

		#region Properties
		//
		public ThingDef_Honedge Def
		{
			get 
			{
				return this.def as ThingDef_Honedge;
			}
		}
		//
		#endregion Properties

		public override void evolve(){

		}
	}
}