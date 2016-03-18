using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a deepstalker corpse")]
    public class Deepstalker : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        public DateTime m_NextStealthCheckAllowed = DateTime.UtcNow;
        public TimeSpan StealthDelay = TimeSpan.FromSeconds(10);

        [Constructable]
        public Deepstalker(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a deepstalker";
            Body = 730;
            Hue = 2075;         

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = -2000;            

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 100.1;
        }

        public override int Meat { get { return 1; } }

        public override int Hides { get { return 12; } }
        public override HideType HideType { get { return HideType.Spined; } }
        
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8510; } }
        public override int TamedItemHue { get { return 2075; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 75; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 25; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 25; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                }
            }

            SpecialAbilities.FrenzySpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "", "*becomes frenzied*");

            m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.05;

            DictWanderAction[WanderAction.None] = 5;
            DictWanderAction[WanderAction.Stealth] = 1;
        }

        public override void SetTamedAI()
        {
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 2;

            BackstabDamageScalar = BaseCreature.TamedCreatureBackstabScalar;

            SetSkill(SkillName.Hiding, 100);
            SetSkill(SkillName.Stealth, 100);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill)
                m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;

            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Alive && Controlled && ControlMaster is PlayerMobile && ControlOrder != OrderType.Stop)
            {
                if (Hidden || Combatant != null)
                    m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;

                else if (DateTime.UtcNow > m_NextStealthCheckAllowed)
                {
                    bool stealthValid = true;

                    if (Combatant != null)
                        stealthValid = false;

                    if (stealthValid)
                    {
                        IPooledEnumerable eable = Map.GetMobilesInRange(Location, RangePerception);

                        foreach (Mobile mobile in eable)
                        {
                            if (mobile.InLOS(this) && mobile.CanSee(this))
                            {
                                if (mobile.Combatant == this)
                                {
                                    stealthValid = false;
                                    break;
                                }

                                bool aggressive = false;

                                foreach (AggressorInfo aggressorInfo in mobile.Aggressors)
                                {
                                    if (aggressorInfo.Attacker == this || aggressorInfo.Defender == this)
                                    {
                                        aggressive = true;
                                        break;
                                    }
                                }

                                if (aggressive)
                                {
                                    stealthValid = false;
                                    break;
                                }
                            }
                        }

                        eable.Free();
                    }

                    if (stealthValid)
                    {
                        AIMiscAction.DoStealth(this);
                        m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
                    }
                }
            }            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            AwardDailyAchievementForKiller(PvECategory.KillIceSkitters);
        }

        public override int GetAngerSound() { return 0x2A9; }
        public override int GetIdleSound() { return 0x2A8; }
        public override int GetAttackSound() { return 0x622; }
        public override int GetHurtSound() { return 0x623; }
        public override int GetDeathSound() { return 0x5D5; }

        public Deepstalker(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
