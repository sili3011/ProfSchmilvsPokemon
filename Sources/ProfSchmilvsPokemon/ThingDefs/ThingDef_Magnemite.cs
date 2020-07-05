using Verse;

namespace ProfSchmilvsPokemon.ThingDefs
{
	
	public class ThingDef_Magnemite : ThingDef
	{
		public float storedEnergyMaxUtility
		{
			get 
			{
				return _storedEnergyMax;
			}
			set
			{
				this._storedEnergyMax = value;
			}
		}

		private float _storedEnergyMax = 250f;

	}
}

