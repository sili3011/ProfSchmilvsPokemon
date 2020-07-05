using System.Collections.Generic;
using Verse;
using RimWorld;
using ProfSchmilvsPokemon.ThingDefs;

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

        private int _ticksToDetonation;
        private Thing _weaponDummy;

		#region Overrides
		//
		public override void Tick()
		{
			base.Tick();
			if (_ticksToDetonation > 0)
			{
				_ticksToDetonation--;
				if (_ticksToDetonation <= 0)
				{
					Explode();
				}
			}
		}

		protected override void Impact(Thing hitThing)
		{

			var l = (Pawn)launcher;

			if (l.equipment != null)
			{
				List<ThingWithComps> allEquipmentListForReading = l.equipment.AllEquipmentListForReading;
				_weaponDummy = allEquipmentListForReading [0];

				var isEmpty = true;

				var splitdummy = _weaponDummy.def.ToString ().Split ('_');

				if (splitdummy.Length > 1) {
					isEmpty = false;
					l.jobs.StopAll (true);
				}

				if (isEmpty) {
					allEquipmentListForReading [0].Destroy ();
				}
			}

			if (def.projectile.explosionDelay == 0)
			{
				Explode();
				return;
			}
			landed = true;
			_ticksToDetonation = def.projectile.explosionDelay;
		}

		protected override void Explode()
		{
			Map map = Map;
			Destroy (DestroyMode.Vanish);
			if (this.def.projectile.explosionEffect != null) {
				var effecter = this.def.projectile.explosionEffect.Spawn();
				effecter.Trigger (new TargetInfo (Position, map, false), new TargetInfo (Position, map, false));
				effecter.Cleanup ();
			}
			var position = Position;
			var map2 = map;
            var explosionRadius = this.def.projectile.explosionRadius;
            var damageDef = this.def.projectile.damageDef;
            var launcher = this.launcher;
            var damageAmountBase = this.def.projectile.GetDamageAmount(0f, null);
            var soundExplode = this.def.projectile.soundExplode;
            var equipmentDef = this.equipmentDef;
            var def = this.def;
            var postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            var postExplosionSpawnChance = this.def.projectile.postExplosionSpawnChance;
            var postExplosionSpawnThingCount = this.def.projectile.postExplosionSpawnThingCount;
            var preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
			GenExplosion.DoExplosion (
                position,
                map2,
                explosionRadius,
                damageDef,
                launcher,
                damageAmountBase,
                0f,
                soundExplode,
                equipmentDef,
                def, 
                null,
                postExplosionSpawnThingDef,
                postExplosionSpawnChance,
                postExplosionSpawnThingCount,
                this.def.projectile.applyDamageToExplosionCellsNeighbors,
                preExplosionSpawnThingDef,
                this.def.projectile.preExplosionSpawnChance,
                0,
                this.def.projectile.explosionChanceToStartFire,
                this.def.projectile.explosionDamageFalloff,
				null
                );

            var used = false;

			//is it a Pokeball with an already captured Pokemon?
            var isEmpty = true;
			var splitdummy = _weaponDummy.def.ToString ().Split ('_');
			if (splitdummy.Length > 1) {
			
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
								var d = closest.def.ToString ();
								var splt = d.Split ('_');

								//if it is, check if it is an already captured Pokemon
								if (splt [0].Equals ("Pokemon")) {

									Log.Message ("Pokemon detected!");

									//if it is not, capture it
									if (closest.Faction == null) {

										var name = "Pokeball_Full";
										var pStrength = 1f;

										if (_weaponDummy.def.ToString ().Equals ("Greatball")) {
											name = "Greatball_Full";
											pStrength = 2f;
										}
										if (_weaponDummy.def.ToString ().Equals ("Ultraball")) {
											name = "Ultraball_Full";
											pStrength = 4f;
										}

										var downed = 1;

										if (closest.health.Downed) {

											downed = 2;
										
										}

										Pawn l = (Pawn)launcher;

										var rec = (float)closest.kindDef.baseRecruitDifficulty;
										var prob = ((((3 * (float)closest.MaxHitPoints - 2 * ((float)closest.MaxHitPoints)*closest.HealthScale) * rec * pStrength)/100f)*downed) + ((float)l.skills.GetSkill(ProfSchmilvsPokemon.DefOfs.SkillDefOf.Pokemon).Level)/20f;
										var rand = Rand.Value;

										Log.Message ("Catch Probability: " + prob.ToString() + "\n Recruitment difficulty: " + rec + "\n Dice roll: " + rand.ToString() + "\n MaxHitpoints: " + closest.MaxHitPoints + "\n Current Hitpoints: " + (closest.MaxHitPoints*closest.HealthScale) + "\n Current Pokemon skill level: " + (((float)l.skills.GetSkill(ProfSchmilvsPokemon.DefOfs.SkillDefOf.Pokemon).Level)/20f));

										if (rand <= prob) {			

											used = true;
												
											var p = (Weapon_Pokeball_Full)ThingMaker.MakeThing (ThingDef.Named (name));
											p.def.description = "A " + name.Split ('_') [0] + " with a Pokémon in it. Throw to release caught Pokémon. \n" +
											closest.Label.CapitalizeFirst ();
											closest.SetFaction (launcher.Faction, (Pawn)launcher);
											closest.training.Train (TrainableUtility.TrainableDefsInListOrder [0], (Pawn)launcher);
											p.Pokemon = closest;
											closest.DeSpawn ();
											GenSpawn.Spawn (p, position, map);

											l.skills.Learn(ProfSchmilvsPokemon.DefOfs.SkillDefOf.Pokemon, (1f-rec)*1000f);

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

					if (_weaponDummy.def.ToString ().Equals ("Greatball")) {
						r = 0.33f;
					}
					if (_weaponDummy.def.ToString ().Equals ("Ultraball")) {
						r = 0.125f;
					}

					var rand = Rand.Value;
					if (rand <= r) {			
						used = true;			
					}

					//if the Pokeball does not get destroyed with no capturable Pokemon in range, spawn a new one
					if (!used) {
						GenSpawn.Spawn (_weaponDummy.def, position, map);
					}

				}
			
			//if the thrown Pokeball is already home to a Pokemon
			} else {

				Weapon_Pokeball_Full p = (Weapon_Pokeball_Full)_weaponDummy;
				Pawn pok = p.Pokemon;

				//check if it is currently empty, if so, check if the Pokemon that belongs to it is in capture range
				if (p.empty) {

					for (int i = 0; i < 24; i++) {

						var intVec = position + GenRadial.RadialPattern [i];
						if (intVec.InBounds (map)) {

							var thing = intVec.GetThingList (map).Find ((Thing x) => x is Pawn);
							if (thing != null) {

								if (GenSight.LineOfSight (position, intVec, map, false, null, 0, 0)) {

									var closest = (Pawn)thing;
									var d = closest.def.ToString ();
									var splt = d.Split ('_');

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

				_weaponDummy = null;

			}

		}
		//
		#endregion Overrides
    }
}