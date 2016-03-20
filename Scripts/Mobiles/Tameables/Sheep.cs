using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Achievements;
namespace Server.Mobiles
{
    [CorpseName("a sheep corpse")]
    public class Sheep : BaseCreature, ICarvable
    {        
        private DateTime m_NextWoolTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextWoolTime
        {
            get { return m_NextWoolTime; }
            set { m_NextWoolTime = value; Body = (DateTime.UtcNow >= m_NextWoolTime) ? 0xCF : 0xDF; }
        }

        public void Carve(Mobile from, Item item)
        {
            if (from != null && !InRange(from.Location, 2))
            {
                from.SendLocalizedMessage(500295); //You are too far away to do that.
                return;
            }

            if (DateTime.UtcNow < m_NextWoolTime)
            {
                // This sheep is not yet ready to be shorn.
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 500449, from.NetState);
                return;
            }

            // IPY ACHIEVEMENT
            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_ShearSheep_TIMED);
            // IPY ACHIEVEMENT

            from.SendLocalizedMessage(500452); // You place the gathered wool into your backpack.
            from.AddToBackpack(new Wool(Map == Map.Felucca ? 2 : 1));

            NextWoolTime = DateTime.UtcNow + TimeSpan.FromHours(3.0); // TODO: Proper time delay
        }

        public override void OnThink()
        {
            base.OnThink();
            Body = (DateTime.UtcNow >= m_NextWoolTime) ? 0xCF : 0xDF;
        }

        [Constructable]
        public Sheep(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a sheep";
            Body = 0xCF;
            BaseSoundID = 0xD6;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

            Fame = 300;
            Karma = 0;


            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        public override int TamedItemId { get { return 8427; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }
        
        public override int Wool { get { return (Body == 0xCF ? 1 : 0); } }

        public Sheep(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.WriteDeltaTime(m_NextWoolTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                {
                    NextWoolTime = reader.ReadDeltaTime();
                    break;
                }
            }
        }
    }
}