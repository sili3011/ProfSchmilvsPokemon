using UnityEngine;
using Verse;
using RimWorld;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Doublade : Pokemon_Abstract_Swords
	{

		#region Properties
		//
		public ThingDef_Doublade Def
		{
			get 
			{
				return this.def as ThingDef_Doublade;
			}
		}
		//
		#endregion Properties

		public override void Draw()
		{
			base.Draw();
		}

		public override void evolve(){

		}
	}
}