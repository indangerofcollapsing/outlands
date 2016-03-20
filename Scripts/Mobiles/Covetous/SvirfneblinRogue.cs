using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a svirfneblin rogue corpse")]
    public class SvirfneblinRogue : BaseCreature
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);
        
        [Constructable]
        public SvirfneblinRogue(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a svirfneblin rogue";
            Body = 723;
            BaseSoundID = 422;
            Hue = 1102;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);
           
            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Stealth, 95);
            SetSkill(SkillName.Hiding, 95);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.25;
            
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;
        }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))
                        Say("*poof*");

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);          
        }

        public override int GetAttackSound(){return 0x5FD;}
        public override int GetHurtSound(){return 0x5FF;}
        public override int GetAngerSound(){return 0x5FF;}
        public override int GetIdleSound(){return 0x600;}
        public override int GetDeathSound(){return 0x55B;}

        public SvirfneblinRogue(Serial serial): base(serial)
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
