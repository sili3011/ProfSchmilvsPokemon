using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI;
using RimWorld;
using ProfSchmilvsPokemon.ThingDefs;

namespace ProfSchmilvsPokemon
{
	public class Pokemon_Grimer : Pawn
	{

		#region Properties
		//
		public ThingDef_Grimer Def
		{
			get 
			{
				return this.def as ThingDef_Grimer;
			}
		}
		//
		#endregion Properties

		public override void Draw()
		{
			base.Draw();
		}

		public override void Tick()
		{
			base.Tick ();

			if (this.amountOfFilth > 20) {

				Pokemon_Muk muk = (Pokemon_Muk)PawnGenerator.GeneratePawn (PawnKindDef.Named ("Pokemon_Muk"));
				if (this.Faction != null) {
					if (this.playerSettings.master != null) {
						muk.SetFaction (this.Faction, (Pawn)this.playerSettings.master);
						muk.training.Train (TrainableUtility.TrainableDefsInListOrder [0], (Pawn)this.playerSettings.master);
					} else {
						muk.SetFaction (this.Faction, (Pawn)this.Faction.leader); //should not happen, just for debugging purposes
					}
					if (!this.Name.ToString().Split(' ')[0].Equals ("grimer")) {
						muk.Name = this.Name;
					}

				}

				for (int i = (int)this.amountOfFilth; i >= 0; i--) {

					muk.IncrementFilth();

				}

				if (this.Spawned) {
					
					GenSpawn.Spawn (muk, this.Position, base.Map);
					this.DeSpawn ();
				}
			
			}

		}

		public void IncrementFilth()
		{
			++this.amountOfFilth;

			this.ageTracker.AgeBiologicalTicks = (long)this.amountOfFilth * 3600000L * 150L;
		}
			
		public override void ExposeData()
		{
			base.ExposeData ();
			Scribe_Values.Look<float>(ref this.amountOfFilth, "amountOfFilth", 0f);
		}

		public float amountOfFilth = 0f;

	}
}