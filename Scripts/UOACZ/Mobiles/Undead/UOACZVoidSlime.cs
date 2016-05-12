using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server
{
    [CorpseName("a void slime corpse")]
    public class UOACZVoidSlime : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public DateTime m_NextVoidAttackAllowed;
        public TimeSpan NextPvMVoidAttackDelay = TimeSpan.FromSeconds(3);
        public TimeSpan NextPvPVoidAttackDelay = TimeSpan.FromSeconds(5);

        public override BonePileType BonePile { get { return BonePileType.Small; } }

        public override int DifficultyValue { get { return 2; } }

		[Constructable]
		public UOACZVoidSlime() : base()
		{
            Name = "a void slime";
            Body = 51;
            Hue = 2200;
            BaseSoundID = 456;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 200);

            VirtualArmor = 25;

            Fame = 300;
            Karma = -300;                  
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 10;
            DictCombatRange[CombatRange.Withdraw] = 0;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;
        }

        public override void OnThink()
        {
            base.OnThink();

            //Prevent Melee Attacks
            LastSwingTime = DateTime.UtcNow + TimeSpan.FromSeconds(30);

            if (DateTime.UtcNow >= m_NextVoidAttackAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = Combatant;

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

                        double effectChance = .10;
                        double damageScalar = 1.0;

                        if (Controlled && ControlMaster != null)
                        {
                            if (ControlMaster is PlayerMobile)
                            {
                                if (combatant is PlayerMobile)
                                    effectChance = .01;

                                else
                                    effectChance = .25;
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

                                    SpecialAbilities.EntangleSpecialAbility(1.0, this, combatant, 1, entangleDuration, 0, false, "", "", "-1");
                                    SpecialAbilities.PierceSpecialAbility(1.0, this, combatant, pierceAmount, 50, 0, false, "", "", "-1");
                                    SpecialAbilities.CrippleSpecialAbility(1.0, this, combatant, crippleAmount, 10, 0, false, "", "", "-1");
                                }

                                weapon.OnHit(this, combatant, damageScalar);
                            });
                        }
                    }
                }
            }
        }

        public UOACZVoidSlime(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
