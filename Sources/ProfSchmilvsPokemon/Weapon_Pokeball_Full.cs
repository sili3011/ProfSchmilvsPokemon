using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace ProfSchmilvsPokemon
{

	public class Weapon_Pokeball_Full : ThingWithComps
	{

		#region Properties
		//
		public ThingDef_Pokeball_Full Def
		{
			get 
			{
				return this.def as ThingDef_Pokeball_Full;
			}
		}

		public Pawn Pokemon
		{
			get 
			{
				return Poke as Pawn;
			}
			set
			{
				this.Poke = value;
			}
		}

		public bool empty
		{
			get 
			{
				return isEmpty;
			}
			set
			{
				this.isEmpty = value;
			}
		}
		//
		#endregion Properties

		private Pawn Poke;
		private bool isEmpty = false;

	}

}