using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a void slime corpse" )]
	public class VoidSlime : BaseCreature
	{
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        public DateTime m_NextVoidAttackAllowed;
        public TimeSpan NextPvMVoidAttackDelay = TimeSpan.FromSeconds(3);
        public TimeSpan NextPvPVoidAttackDelay = TimeSpan.FromSeconds(5);

		[Constructable]
		public VoidSlime() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a void slime";
			Body = 51;
            Hue = 2200;
			BaseSoundID = 456;			

            SetStr(50);
            SetDex(25);
            SetInt(50);

            SetHits(500);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 300);
            
            VirtualArmor = 25;

			Fame = 300;
			Karma = -300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 115.1;
        }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8424; } }
        public override int TamedItemHue { get { return Hue; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 10; } }
        public override int TamedBaseMaxDamage { get { return 12; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 150; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }
        
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
            NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);

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

                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, 1.0, true, 0, false, "", "");

                        double entangleDuration = 3;
                        double pierceAmount = .25;
                        double crippleAmount = .20;

                        TimeSpan attackCooldown = NextPvMVoidAttackDelay;

                        double effectChance = .15;
                        double damageScalar = 1.0;

                        if (Controlled && ControlMaster != null)
                        {
                            if (ControlMaster is PlayerMobile)
                            {
                                if (combatant is PlayerMobile)
                                    effectChance = .01;

                                else
                                    effectChance = .33;
                            }
                        }

                        if (ControlMaster is PlayerMobile && combatant is PlayerMobile)
                        {
                            entangleDuration = 1;
                            attackCooldown = NextPvPVoidAttackDelay;
                        }

                        m_NextVoidAttackAllowed = DateTime.UtcNow + attackCooldown;                        

                        Animate(4, 4, 1, true, false, 0);

                        BaseWeapon weapon = Weapon as BaseWeapon;

                        if (weapon == null)
                            return;

                        bool hitSuccessful = false;

                        if (weapon.CheckHit(this, combatant))
                            hitSuccessful = true;

                        Effects.PlaySound(Location, Map, 0x5D8);

                        MovingEffect(combatant, 0x573E, 8, 1, false, false, 2200, 0);

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

                                TimedStatic voidResidue = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                                voidResidue.Hue = 2051;
                                voidResidue.Name = "void residue";
                                voidResidue.MoveToWorld(new Point3D(combatant.X, combatant.Y, combatant.Z), Map);                               

                                if (Utility.RandomDouble() <= effectChance)
                                {
                                    Effects.PlaySound(combatant.Location, combatant.Map, 0x5DC);

                                    for (int a = 0; a < 3; a++)
                                    {
                                        voidResidue = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                                        voidResidue.Hue = 2051;
                                        voidResidue.Name = "void residue";

                                        Point3D voidResidueLocation = new Point3D(combatant.X + Utility.RandomMinMax(-1, 1), combatant.Y + Utility.RandomMinMax(-1, 1), combatant.Z);
                                        SpellHelper.AdjustField(ref voidResidueLocation, combatant.Map, 12, false);

                                        voidResidue.MoveToWorld(voidResidueLocation, combatant.Map);
                                    }

                                    combatant.SendMessage("You have been covered in void residue!");
                                    combatant.FixedParticles(0x374A, 10, 15, 5021, 2051, 0, EffectLayer.Waist);

                                    SpecialAbilities.EntangleSpecialAbility(1.0, this, combatant, 1, entangleDuration, 0, false, "", "");
                                    SpecialAbilities.PierceSpecialAbility(1.0, this, combatant, pierceAmount, 10, 0, false, "", "");
                                    SpecialAbilities.CrippleSpecialAbility(1.0, this, combatant, crippleAmount, 10, 0, false, "", "");
                                }

                                weapon.OnHit(this, combatant, damageScalar);
                            });
                        }
                    }
                }
            }
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override int GetAttackSound() { return 0x5DA; }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

    		switch( Utility.Random( 1000 ) )
    		{
         		case 1: { c.AddItem( new Diamond( ) ); } break;
         		case 2: { c.AddItem( new Ruby( ) ); } break;
         		case 3: { c.AddItem( new HealPotion( ) ); } break;
         		case 4: { c.AddItem( new HealScroll( ) ); } break;         			
    		}
		}        
        
        public VoidSlime( Serial serial ) : base( serial )
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
