using ProfSchmilvsPokemon.Pokemon.Swords;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Honedge : PokemonAbstractSwords
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

		public override void Evolve(){

		}
	}
}