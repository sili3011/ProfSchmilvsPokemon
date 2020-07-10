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
	public class Pokemon_Magnezone : Pokemon_Abstract_Magnets {

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

		public Pokemon_Magnezone()
		{
			base.storedEnergyMax = this.Def.storedEnergyMaxUtility;
		}

		public override void evolve() {}

		public override void repairing() {
		
			this.repairJob.HitPoints += 5;
			this.StoredEnergy--;
			Map.overlayDrawer.DrawOverlay(this.repairJob, OverlayTypes.BurningWick);
		
		}
	}
}