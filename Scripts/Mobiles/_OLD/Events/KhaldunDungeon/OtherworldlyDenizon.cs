using System;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a denizon's corpse")]
    public class OtherworldlyDenizon : BaseCreature
    {
        public bool isExploding = false;

        [Constructable]
        public OtherworldlyDenizon() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an otherworldy denizon";

            Body = 776;
            Hue = 25000;

            BaseSoundID = 684;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 0;                        
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1;
            RangePerception = 18;

            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .20 && !isExploding)
            {
                isExploding = true;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*loses grip on reality*");

                Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3)), delegate
                {
                    if (Deleted || !Alive) return;

                    SpecialAbilities.AnimalExplosion(null, Location, Map, null, Utility.RandomMinMax(1, 3), 5, 15, 5, 2588, true, true);

                    Kill();
                });
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill && amount >= 10)
            {
                if (Utility.RandomDouble() <= .20 && !isExploding)
                {
                    isExploding = true;

                    PublicOverheadMessage(MessageType.Regular, 0, false, "*loses grip on reality*");

                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3)), delegate
                    {
                        if (Deleted || !Alive) return;

                        SpecialAbilities.AnimalExplosion(null, Location, Map, null, Utility.RandomMinMax(1, 3), 5, 10, 5, 2588, true, true);

                        Kill();
                    });
                }
            }
        }  

        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x254; }
        public override int GetIdleSound() { return 0x253; }
        public override int GetAttackSound() { return 0x615; }
        public override int GetHurtSound() { return 0x613; }
        public override int GetDeathSound() { return 0x256; }

        public OtherworldlyDenizon(Serial serial) : base(serial) { }        

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