using Verse;

namespace ProfSchmilvsPokemon.Pokemon.Slime
{
	[StaticConstructorOnStartup]
	public class PokemonAbstractSlimes : PokemonAbstract {

		public float AmountOfFilth;
		protected int CurrentTick = 0;
		protected Zone CurrentDump;
		protected Thing Digesting;
		protected long DigestingTicks;

        public void IncrementFilth() {
			++AmountOfFilth;
			ageTracker.AgeBiologicalTicks = (long)AmountOfFilth * 3600000L * 150L;
		}

		public void DecrementFilth() {
			--AmountOfFilth;
			ageTracker.AgeBiologicalTicks = (long)AmountOfFilth * 3600000L * 150L;
		}

		public override void ExposeData() {
			base.ExposeData ();
			Scribe_Values.Look(ref AmountOfFilth, "AmountOfFilth", 0f);
			Scribe_Deep.Look(ref CurrentDump, "CurrentDump", null);
			Scribe_Deep.Look(ref Digesting, "Digesting", null);
			Scribe_Values.Look(ref DigestingTicks, "DigestingTicks", 0L);
		}

	}

}