using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon
{
	
	public class ThingDef_Mareep : ThingDef
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

		private float storedEnergyMax = 1000f;

	}
}

