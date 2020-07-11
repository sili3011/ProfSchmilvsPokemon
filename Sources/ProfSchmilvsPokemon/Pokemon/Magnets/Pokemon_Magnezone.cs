using ProfSchmilvsPokemon.Pokemon.Magnets;
using ProfSchmilvsPokemon.ThingDefs;
using RimWorld;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Magnezone : PokemonAbstractMagnets {

		#region Properties
		//
		public ThingDef_Magnezone Def
		{
			get {
				return def as ThingDef_Magnezone;
			}
		}
		//
		#endregion Properties

		public Pokemon_Magnezone()
		{
			StoredEnergyMax = Def.storedEnergyMaxUtility;
		}

		public override void Evolve() {}

		public override void Repairing() {
		
			RepairJob.HitPoints += 5;
			StoredEnergy--;
			Map.overlayDrawer.DrawOverlay(RepairJob, OverlayTypes.BurningWick);
		
		}
	}
}