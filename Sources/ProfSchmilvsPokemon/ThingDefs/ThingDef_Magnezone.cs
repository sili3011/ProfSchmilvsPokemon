﻿using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon.ThingDefs
{
	
	public class ThingDef_Magnezone : ThingDef
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

		private float storedEnergyMax = 1250f;

	}
}
