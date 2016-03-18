using System;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("an ent corpse")]
    public class Ent : BaseCreature
    {
        public int BranchItemId { get { return Utility.RandomList(3387, 3388, 3889); } }
        public int MushroomItemId { get { return Utility.RandomList(3340, 3341, 3342, 3343, 3344, 3345, 3346, 3347, 3348, 3349, 3350, 3351, 3352, 3353); } }
        public int GrassItemId { get { return Utility.RandomList(3220, 3255, 3256, 3223, 3245, 3246, 3248, 3254, 3257, 3258, 3259, 3260, 3261, 3269, 3270, 3267, 3237, 3267, 3239, 3332); } }
        public int FlowerItemId{get{return Utility.RandomList(3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, 3212, 3213, 3214, 3262, 3263, 3264, 3265,
                    6809, 6810, 6811);}}

        [Constructable]
        public Ent(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ent";
            Body = 301;
            Hue = Utility.RandomList(2526, 2527, 2528, 2515, 2207);

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(2500);
            SetStam(500);

            SetDamage(30, 40);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;

            Fame = 8000;
            Karma = 0;
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 18;            

            UniqueCreatureDifficultyScalar = 1.25;           
        }

        public override bool AllowParagon { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .25)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*flings them aside*");

                double damage = Utility.RandomMinMax(DamageMin, DamageMax);

                SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 10, -1, "", "The creature flings you aside!");

                Combatant = null;
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (amount > 10 && !willKill)
            {
                if (Utility.RandomDouble() < .25)
                {
                    Effects.PlaySound(Location, Map, 0x59E);

                    Blood branches = new Blood();
                    branches.Name = "branches";
                    branches.ItemID = BranchItemId;
                    branches.MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
                } 
            }                       
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() < .5)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x33C, 0x33B));

                int items = Utility.RandomMinMax(1, 3);

                for (int a = 0; a < items; a++)
                {
                    Point3D moveItemLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                    SpellHelper.AdjustField(ref moveItemLocation, Map, 12, false);

                    Blood moveItem = new Blood();
                    moveItem.Name = "mushroom";
                    moveItem.ItemID = MushroomItemId;
                    moveItem.MoveToWorld(moveItemLocation, Map);
                }
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, 0x4CF);

            for (int a = 0; a < 30; a++)
            {
                Blood branches = new Blood();
                branches.Name = "branches";
                branches.ItemID = BranchItemId;

                if (Utility.RandomDouble() < .8)
                {
                    branches.Name = "mushrooms";
                    branches.ItemID = MushroomItemId;
                }

                Point3D branchesLocation = new Point3D(Location.X + Utility.RandomMinMax(-4, 4), Location.Y + Utility.RandomMinMax(-4, 4), Location.Z);

                branches.MoveToWorld(branchesLocation, Map);
            }

            PackItem(new Engines.Plants.Seed());
            PackItem(new Engines.Plants.Seed());
            PackItem(new Engines.Plants.Seed());
            PackItem(new FertileDirt(Utility.RandomMinMax(3, 6)));

            if (Utility.RandomMinMax(1, 5) == 1)
                PackItem(new EntAppendage());

            if (Utility.RandomMinMax(1, 2) == 1)
                PackItem(new MagicSpringwood());

            return base.OnBeforeDeath();
        }

        public override int GetAttackSound() { return 0x626; }
        public override int GetHurtSound() { return 0x629; }
        public override int GetAngerSound() { return 0x2AB; }
        public override int GetIdleSound() { return 0x066; }
        public override int GetDeathSound() { return 0x0E4; }   
        
        public Ent(Serial serial): base(serial)
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