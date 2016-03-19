using Server.Custom.Battlegrounds.Regions;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Mobiles
{
    [CorpseName("The King's corpse")]
    public class BattlegroundKing : BaseCreature
    {
        protected SiegeBattleground m_Battleground;

        public override bool IsScaryToPets { get { return true; } }

        public override bool DeleteCorpseOnDeath {get {return true;}}

        public override bool AlwaysMurderer { get { return true; } }

        public override bool ImmuneToSpecialAttacks { get { return true; } }
        public override bool ImmuneToChargedSpells { get { return true; } }

        public BattlegroundKing()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "The King";
            SetHits(500);
            VirtualArmor = 100;
            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Wrestling, 100);
            CantWalk = true;
        }


        public override void OnDeath(Container c)
        {
            m_Battleground.OnKingsDeath();
            base.OnDeath(c);
        }

        public BattlegroundKing(Serial serial)
            : base(serial)
        {

        }

        protected override void OnMapChange(Map oldMap)
        {
            base.OnMapChange(oldMap);
            if (Location == Point3D.Zero) return;

            var region = Region.Find(Location, Map);
            if (!(region is BattlegroundRegion))
            {
                Delete();
                return;
            }

            m_Battleground = ((BattlegroundRegion)region).Battleground as SiegeBattleground;

            if (m_Battleground == null)
            {
                Delete();
                return;
            }
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
            OnMapChange(this.Map);
        }
    }
}
