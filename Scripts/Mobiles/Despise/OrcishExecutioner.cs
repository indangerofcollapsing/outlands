
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish executioner corpse")]
    public class OrcishExecutioner : BaseOrc
    {
        [Constructable]
        public OrcishExecutioner(): base()
        {            
            Name = "an orcish executioner";

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(600);

            AttackSpeed = 30;

            SetDamage(18, 28);

            SetSkill(SkillName.Archery, 95);
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 5000;
            Karma = -5000;            

            AddItem(new OrcMask() { Movable = false, Hue = 1776 });
            AddItem(new BoneArms() { Movable = false, Hue = 0 });
            AddItem(new BoneGloves() { Movable = false, Hue = 1775 });
            AddItem(new Kilt() { Movable = false, Hue = 2051 });
            AddItem(new Boots() { Movable = false, Hue = 2051 });

            AddItem(new ExecutionersAxe());
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }
        
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {            
            if (!willKill)
            {               
            }            

            base.OnDamage(amount, from, willKill);
        }
        
        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public OrcishExecutioner(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    } 
}
