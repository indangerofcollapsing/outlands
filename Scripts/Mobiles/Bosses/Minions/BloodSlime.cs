using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a blood slime corpse" )]
	public class BloodSlime : BaseCreature
	{
        public DateTime m_NextVoidAttackAllowed;

        public TimeSpan NextPvMVoidAttackDelay = TimeSpan.FromSeconds(3);
        public TimeSpan NextPvPVoidAttackDelay = TimeSpan.FromSeconds(5);

		[Constructable]
		public BloodSlime() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a blood slime";
			Body = 51;
            Hue = 38;
			BaseSoundID = 456;			

            SetStr(50);
            SetDex(25);
            SetInt(50);

            SetHits(250);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 300);
            
            VirtualArmor = 25;

			Fame = 300;
			Karma = -300;
        }

        public override int PoisonResistance { get { return 5; } }

        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        
        public override void SetUniqueAI()
        {  
            UniqueCreatureDifficultyScalar = 1.5;

            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 10;
            DictCombatRange[CombatRange.Withdraw] = 0;
        }

        public override void OnThink()
        {
            base.OnThink();

            //Prevent Melee Attacks
            LastSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);

            Mobile combatant = Combatant;

            if (ControlMaster is PlayerMobile)
            {
                int minSeconds = 0;
                int maxSeconds = (int)(Math.Round(NextPvMVoidAttackDelay.TotalSeconds));

                if (combatant == null)
                    m_NextVoidAttackAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(minSeconds, maxSeconds));
            }           

            if (DateTime.UtcNow >= m_NextVoidAttackAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                if (combatant != null && !CantWalk && !Frozen && Alive && !IsDeadPet && !IsDeadBondedPet)
                {
                    if (combatant.Alive && InLOS(combatant) && GetDistanceToSqrt(combatant) <= 8)
                    {
                        RevealingAction();

                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 1.0, true, 0, false, "", "", "-1");

                        double entangleDuration = 3;
                        double pierceAmount = .25;
                        double crippleAmount = .20;

                        TimeSpan attackCooldown = NextPvMVoidAttackDelay;

                        double effectChance = .2;
                        double damageScalar = 1.0;
                        
                        m_NextVoidAttackAllowed = DateTime.UtcNow + attackCooldown;                        

                        Animate(4, 4, 1, true, false, 0);

                        BaseWeapon weapon = Weapon as BaseWeapon;

                        if (weapon == null)
                            return;

                        bool hitSuccessful = false;

                        if (weapon.CheckHit(this, combatant))
                            hitSuccessful = true;

                        Effects.PlaySound(Location, Map, 0x5D8);

                        int itemID = 0x573E; // Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F);
                        int itemHue = 38;

                        MovingEffect(combatant, itemID, 8, 1, false, false, itemHue, 0);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 10), Map);

                        double distance = Utility.GetDistanceToSqrt(Location, combatant.Location);
                        double destinationDelay = (double)distance * .08;

                        if (hitSuccessful)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;
                                if (Deleted || !Alive || IsDeadPet || IsDeadBondedPet) return;
                                if (!SpecialAbilities.IsDamagable(combatant)) return;
                                if (Utility.GetDistance(Location, combatant.Location) >= 20) return;

                                new Blood(TimeSpan.FromSeconds(10)).MoveToWorld(new Point3D(combatant.X, combatant.Y, combatant.Z), Map);

                                if (Utility.RandomDouble() <= effectChance)
                                {
                                    Effects.PlaySound(combatant.Location, combatant.Map, 0x5DC);

                                    for (int a = 0; a < 3; a++)
                                    {
                                        Point3D voidResidueLocation = new Point3D(combatant.X + Utility.RandomMinMax(-1, 1), combatant.Y + Utility.RandomMinMax(-1, 1), combatant.Z);
                                        SpellHelper.AdjustField(ref voidResidueLocation, combatant.Map, 12, false);

                                        new Blood(TimeSpan.FromSeconds(10)).MoveToWorld(voidResidueLocation, combatant.Map);
                                    }

                                    combatant.SendMessage("You have been covered in sticky blood!");
                                    combatant.FixedParticles(0x374A, 10, 15, 5021, 38, 0, EffectLayer.Waist);

                                    SpecialAbilities.EntangleSpecialAbility(1.0, this, combatant, 2, entangleDuration, 0, false, "", "", "-1");
                                }

                                weapon.OnHit(this, combatant, damageScalar);
                            });
                        }
                    }
                }
            }
        }

        public override int GetAttackSound() { return 0x5DA; }       
        
        public BloodSlime( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
