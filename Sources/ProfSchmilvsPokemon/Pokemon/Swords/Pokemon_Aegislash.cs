using ProfSchmilvsPokemon.Pokemon.Swords;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Aegislash : PokemonAbstractSwords
	{

		#region Properties
		//
		public ThingDef_Aegislash Def
		{
			get 
			{
				return this.def as ThingDef_Aegislash;
			}
		}
		//
		#endregion Properties

		public override void Evolve(){}

	}
}