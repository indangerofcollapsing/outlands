using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a swamp crawler corpse")]
    public class SwampCrawler : BaseCreature
    {
        public DateTime m_NextStealthCheckAllowed = DateTime.UtcNow;
        public TimeSpan StealthDelay = TimeSpan.FromSeconds(10);

        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public SwampCrawler(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a swamp crawler";
            Body = 720;
            Hue = 2127;
            BaseSoundID = 0x606;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(250);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;

            Fame = 1000;
            Karma = -1000;
        }

        public override int TamedItemId { get { return 8469; } }
        public override int TamedItemHue { get { return 870; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 250; } }
        public override int TamedBaseMinDamage { get { return 16; } }
        public override int TamedBaseMaxDamage { get { return 18; } }
        public override double TamedBaseWrestling { get { return 80; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

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

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
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

        public SwampCrawler(Serial serial): base(serial)
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