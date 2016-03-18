using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class OrderDragon : BaseCreature
    {
        private DateTime m_LastSpecialAttack;        

        public Rectangle2D containRect;
        public bool contain;

        [Constructable]
        public OrderDragon(): base(AIType.AI_Mage, FightMode.Closest, 30, 1, 0.2, 0.4)
        {
            Name = "an Order dragon";
            Body = 0xC;
            BaseSoundID = 362;
            Hue = 2119;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(7500);
            SetMana(3000);

            SetDamage(30, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;           

            Fame = 15000;
            Karma = -15000;

            Tamable = false;
            m_LastSpecialAttack = DateTime.UtcNow + TimeSpan.FromSeconds(20);
        }

        public override int Meat { get { return 5; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Barbed; } }

        public override bool AlwaysMurderer { get { return true; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public OrderDragon(Serial serial): base(serial)
        {
        }

        protected override bool OnMove(Direction d)
        {
            if (contain)
            {
                int oX = 0;
                int oY = 0;
                switch (d & Direction.Mask)
                {
                    case (Direction.North): oY = -1; break;
                    case (Direction.Right): oY = -1; oX = 1; break;
                    case (Direction.East): oX = 1; break;
                    case (Direction.Down): oY = 1; oX = 1; break;
                    case (Direction.South): oY = 1; break;
                    case (Direction.Left): oX = -1; oY = 1; break;
                    case (Direction.West): oX = -1; break;
                    case (Direction.Up): oY = -1; oX = -1; break;
                }

                return (containRect.Contains(new Point2D(Location.X + oX, Location.Y + oY)));
            }
            return base.OnMove(d);
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m.Player)
                return true;
            else
                return false;
        }

        public override bool IsFriend(Mobile m)
        {
            if (m.Player)
                return false;
            else
                return true;
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            damage = 0;
            PrivateOverheadMessage(Network.MessageType.Regular, EmoteHue, true, "Your melee attack glances off the dragon's thick hide",from.NetState);
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