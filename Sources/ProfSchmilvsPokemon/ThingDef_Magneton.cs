using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon
{
	
	public class ThingDef_Magneton : ThingDef
	{
		public float storedEnergyMaxUtility
		{
			get 
			{
				return storedEnergyMax;
			}
			set
			{
				this.storedEnergyMax = value;
			}
		}

		private float storedEnergyMax = 750f;

	}
}

