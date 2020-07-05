using Verse;

namespace ProfSchmilvsPokemon.ThingDefs
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

