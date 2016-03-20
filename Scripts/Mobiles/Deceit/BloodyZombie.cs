using Server.Achievements;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a bloody zombie corpse")]
    public class BloodyZombie : BaseCreature
    {
        [Constructable]
        public BloodyZombie(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 1.0)
        {
            Name = "a bloody zombie";
            Body = 3;
            Hue = 1779;
            BaseSoundID = 471;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 3500;
            Karma = -3500;            
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 2; } }

        public override void SetUniqueAI()
        {   
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            
            int bloodItem = Utility.RandomMinMax(1, 2);

            for (int a = 0; a < bloodItem; a++)
            {
                new Blood().MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(0.1, this, defender, DamageMax, 8.0, Utility.RandomList(0x5D9, 0x5DB), true, "", "Their attack causes you to bleed!");            
        }

        protected override bool OnMove(Direction d)
        {           
            for (int a = 0; a < 1; a++)
            {
                new Blood().MoveToWorld(new Point3D(this.X + Utility.RandomMinMax(-1, 1), this.Y + Utility.RandomMinMax(-1, 1), this.Z), this.Map);
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {           
            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

            int bloodItem = Utility.RandomMinMax(2, 3);

            for (int a = 0; a < bloodItem; a++)
            {
                new Blood().MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            
            
            base.OnDeath(c);
        }    

        public BloodyZombie(Serial serial): base(serial)
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
