using Server.Achievements;
using Server.Items;
using Server.Spells.Fourth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("a flaming zombie corpse")]
    public class FlamingZombie : BaseCreature
    {        
        [Constructable]
        public FlamingZombie(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 1.0)
        {
            Name = "a flaming zombie";
            Body = 3;
            Hue = 1359;
            BaseSoundID = 471;

            SetStr(75);
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
            UniqueCreatureDifficultyScalar = 1.2;
        }

        protected override bool OnMove(Direction d)
        {            
            if (Utility.RandomDouble() < .25)
            {
                Effects.PlaySound(Location, Map, 0x208);

                SingleFireField singleFireField = new SingleFireField(this, 0, 1, 20, 3, 5, false, false, true, -1, true);
                singleFireField.Hue = Hue;
                singleFireField.MoveToWorld(Location, Map);
            }            

            return base.OnMove(d);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
          
            if (!willKill && amount > 10 && Utility.RandomDouble() < .2)
            {
                Effects.PlaySound(Location, Map, 0x208);

                SingleFireField singleFireField = new SingleFireField(this, 0, 1, 20, 3, 5, false, false, true, -1, true);
                singleFireField.Hue = Hue;
                singleFireField.MoveToWorld(Location, Map);
            }            
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, 0x208);
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);            

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {   
            base.OnDeath(c);
        }    

        public FlamingZombie(Serial serial): base(serial)
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
