using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace ProfSchmilvsPokemon
{
	public class Projectile_Pokeball : Projectile_Explosive
	{

		#region Properties
		//
		public ThingDef_Pokeball Def
		{
			get 
			{
				return this.def as ThingDef_Pokeball;
			}
		}
		//
		#endregion Properties

		#region Overrides
		//
		public override void Tick()
		{
			base.Tick();
			if (this.ticksToDetonation > 0)
			{
				this.ticksToDetonation--;
				if (this.ticksToDetonation <= 0)
				{
					this.Explode();
				}
			}
		}

		protected override void Impact(Thing hitThing)
		{

			Pawn l = (Pawn)this.launcher;

			if (l.equipment != null)
			{
				List<ThingWithComps> allEquipmentListForReading = l.equipment.AllEquipmentListForReading;
				weaponDummy = allEquipmentListForReading [0];

				bool isEmpty = true;

				string[] splitteddummy = weaponDummy.def.ToString ().Split ('_');

				if (splitteddummy.Length > 1) {
					isEmpty = false;
					l.jobs.StopAll (true);
				}

				if (isEmpty) {
					allEquipmentListForReading [0].Destroy ();
				}
			}

			if (this.def.projectile.explosionDelay == 0)
			{
				this.Explode();
				return;
			}
			this.landed = true;
			this.ticksToDetonation = this.def.projectile.explosionDelay;
		}

		protected override void Explode()
		{
			Map map = base.Map;
			this.Destroy (DestroyMode.Vanish);
			if (this.def.projectile.explosionEffect != null) {
				Effecter effecter = this.def.projectile.explosionEffect.Spawn ();
				effecter.Trigger (new TargetInfo (base.Position, map, false), new TargetInfo (base.Position, map, false));
				effecter.Cleanup ();
			}
			IntVec3 position = base.Position;
			Map map2 = map;
			float explosionRadius = this.def.projectile.explosionRadius;
			DamageDef damageDef = this.def.projectile.damageDef;
			Thing launcher = this.launcher;
			int damageAmountBase = this.def.projectile.damageAmountBase;
			SoundDef soundExplode = this.def.projectile.soundExplode;
			ThingDef equipmentDef = this.equipmentDef;
			ThingDef def = this.def;
			ThingDef postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
			float postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
			int postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
			ThingDef preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
			GenExplosion.DoExplosion (position, map2, explosionRadius, damageDef, launcher, damageAmountBase, soundExplode, equipmentDef, def, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, this.def.projectile.applyDamageToExplosionCellsNeighbors, preExplosionSpawnThingDef, this.def.projectile.preExplosionSpawnChance, this.def.projectile.preExplosionSpawnThingCount, this.def.projectile.explosionChanceToStartFire, this.def.projectile.explosionDealMoreDamageAtCenter);

			bool used = false;

			//is it a Pokeball with an already captured Pokemon?
			bool isEmpty = true;
			string[] splitteddummy = weaponDummy.def.ToString ().Split ('_');
			if (splitteddummy.Length > 1) {
			
				isEmpty = false;
			
			}

			//if it is not, detect if a something is in capture range
			if(isEmpty){

				for (int i = 0; i < 24; i++) {
					
					IntVec3 intVec = position + GenRadial.RadialPattern [i];
					if (intVec.InBounds (map)) {
						
						Thing thing = intVec.GetThingList (map).Find ((Thing x) => x is Pawn);
						if (thing != null) {

							//if it is, check if it is a Pokemon
							if (GenSight.LineOfSight (position, intVec, map, false, null, 0, 0)) {
								
								Pawn closest = (Pawn)thing;
								string d = closest.def.ToString ();
								string[] splt = d.Split ('_');

								//if it is, check if it is an already captured Pokemon
								if (splt [0].Equals ("Pokemon")) {

									Log.Message ("Pokemon detected!");

									//if it is not, capture it
									if (closest.Faction == null) {

										string name = "Pokeball_Full";
										float pStrength = 1f;

										if (weaponDummy.def.ToString ().Equals ("Greatball")) {
											name = "Greatball_Full";
											pStrength = 2f;
										}
										if (weaponDummy.def.ToString ().Equals ("Ultraball")) {
											name = "Ultraball_Full";
											pStrength = 4f;
										}

										int downed = 1;

										if (closest.health.Downed) {

											downed = 2;
										
										}

										float rec = (float)closest.kindDef.baseRecruitDifficulty;
										float prob = (((3 * (float)closest.MaxHitPoints - 2 * ((float)closest.MaxHitPoints)*closest.HealthScale) * rec * pStrength)/100f)*downed;
										var rand = Rand.Value;

										Log.Message ("Catch Probability: " + prob.ToString() + "\n Recruitment difficulty: " + rec + "\n Dice roll: " + rand.ToString() + "\n MaxHitpoints" + closest.MaxHitPoints + "\n Current Hitpoints" + (closest.MaxHitPoints*closest.HealthScale));

										if (rand <= prob) {			

											used = true;
												
											Weapon_Pokeball_Full p = (Weapon_Pokeball_Full)ThingMaker.MakeThing (ThingDef.Named (name));
											p.def.description = "A " + name.Split ('_') [0] + " with a Pokémon in it. Throw to release caught Pokémon. \n" +
											closest.Label.CapitalizeFirst ();
											closest.SetFaction (launcher.Faction, (Pawn)launcher);
											List<TrainableDef> trainableDefsInListOrder = TrainableUtility.TrainableDefsInListOrder;
											closest.training.Train (trainableDefsInListOrder [0], (Pawn)launcher);
											p.Pokemon = closest;
											closest.DeSpawn ();
											GenSpawn.Spawn (p, position, map);

										}

									}

									break;
								
								//if there is no Pokemon in range, break
								} else {
								
									Log.Message ("This is no Pokemon!");
									break;
								
								}
						
							}
					
						}
				
					}
			
				}

				//check if a Pokemon was captured in this attempt, if not random a number to determine if the thrown ball gets destroyed
				if(!used){

					float r = 0.5f;

					if (weaponDummy.def.ToString ().Equals ("Greatball")) {
						r = 0.33f;
					}
					if (weaponDummy.def.ToString ().Equals ("Ultraball")) {
						r = 0.125f;
					}

					var rand = Rand.Value;
					if (rand <= r) {			
						used = true;			
					}

					//if the Pokeball does not get destroyed with no capturable Pokemon in range, spawn a new one
					if (!used) {
						GenSpawn.Spawn (weaponDummy.def, position, map);
					}

				}
			
			//if the thrown Pokeball is already home to a Pokemon
			}else{

				Weapon_Pokeball_Full p = (Weapon_Pokeball_Full)weaponDummy;
				Pawn pok = p.Pokemon;

				//check if it is currently empty, if so, check if the Pokemon that belongs to it is in capture range
				if (p.empty) {

					for (int i = 0; i < 24; i++) {

						IntVec3 intVec = position + GenRadial.RadialPattern [i];
						if (intVec.InBounds (map)) {

							Thing thing = intVec.GetThingList (map).Find ((Thing x) => x is Pawn);
							if (thing != null) {

								if (GenSight.LineOfSight (position, intVec, map, false, null, 0, 0)) {

									Pawn closest = (Pawn)thing;
									string d = closest.def.ToString ();
									string[] splt = d.Split ('_');

									//if it is, get it back into the ball
									if (splt [0].Equals ("Pokemon")) {

										if (closest.Equals (pok)) {

											pok.DeSpawn ();
											p.empty = false;
											break;

										}

									//if it is not, break
									} else {

										Log.Message ("Captured Pokemon not in range!");
										break;

									}

								}

							}

						}

					}

				//if captured Pokemon is currently in the ball, release it at Pokeball position
				} else {

					GenSpawn.Spawn (pok, position, map);
					p.empty = true;

				}

				weaponDummy = null;

			}

		}
		//
		#endregion Overrides

		private int ticksToDetonation;
		private Thing weaponDummy;

	}
}