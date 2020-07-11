using ProfSchmilvsPokemon.Pokemon.Swords;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Doublade : PokemonAbstractSwords
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

		public override void Evolve(){

		}
	}
}