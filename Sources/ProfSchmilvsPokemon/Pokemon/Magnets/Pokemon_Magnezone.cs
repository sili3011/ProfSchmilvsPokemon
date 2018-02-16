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
	public class Pokemon_Magnezone : Pokemon_Abstract_Magnets
	{

		public Pokemon_Magnezone(){
			base.storedEnergyMax = this.Def.storedEnergyMaxUtility;
		}

		#region Properties
		//
		public ThingDef_Magnezone Def
		{
			get 
			{
				return this.def as ThingDef_Magnezone;
			}
		}
		//
		#endregion Properties

		public override void evolve(){}

		public override void repairing(){
		
			this.repairJob.HitPoints += 5;
			this.StoredEnergy--;
			base.Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);
		
		}
	}
}