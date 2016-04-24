using System;
using Server.Mobiles;
using Server.Items;


namespace Server.Mobiles
{
    [CorpseName("an entrail corpse")]
    public class Entrail : BaseCreature
    {
        [Constructable]
        public Entrail(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an entrail";
            Body = 732;
            Hue = Utility.RandomPinkHue();

            BaseSoundID = 0xDB;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(5, 10);

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 24;

            SetSkill(SkillName.Wrestling, 60);            
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
           
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override bool AlwaysBossMinion { get { return true; } }

        public override int PoisonResistance { get { return 5; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override bool AllowParagon { get { return false; } }
        public override bool BardImmune { get { return true; } }  

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
                        
            defender.SendMessage("The creature burrows inside of you, causing you immense pain and discomfort!");

            double damage = 120;

            if (defender is BaseCreature)
                damage *= 1.5;

            SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, -1, true, "", "", "-1");

            Effects.PlaySound(Location, Map, 0x4F1);

            new Blood().MoveToWorld(new Point3D(defender.X, defender.Y, defender.Z), defender.Map);

            for (int a = 0; a < 4; a++)
            {
                new Blood().MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), defender.Map);
            }

            Kill();            
        }

        protected override bool OnMove(Direction d)
        {           
            Blood blood = new Blood();
            blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);

            blood.MoveToWorld(Location, Map);            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
            {
                if (c != null)
                    c.Delete();
            });
        }

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x57F; }
        
        public Entrail(Serial serial): base(serial)
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