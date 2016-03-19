using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a werewolf corpse")]
    public class HalloweenWerewolf : BaseCreature
    {
        public bool frenzied = false;

        public DateTime m_NextFrenzyDamage;
        public TimeSpan NextFrenzyDelay = TimeSpan.FromSeconds(1);

        [Constructable]
        public HalloweenWerewolf() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rather hairy man";
            Body = 400;
            Hue = 33774;
            HairItemID = 8252;
            HairHue = 1117;
            BaseSoundID = 0x45A;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1500);

            SetDamage(25, 35);

            AttackSpeed = 30;

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 50;

            Fame = 2500;
            Karma = -25000;

            AddItem(new LongPants() { Movable = false, Hue = 1365 });
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.5;
        }

        public override int Meat { get { return 4; } }
        public override int Hides { get { return 25; } }
        public override bool AlwaysAttackable { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {
                if (!frenzied && Utility.RandomDouble() < 0.33)
                {
                    PlaySound(GetAngerSound());

                    frenzied = true;

                    SetDamage(30, 45);
                    SetSkill(SkillName.Wrestling, 120);
                    AttackSpeed = 60;
                    Body = 23;
                    Hue = 33774;
                    Name = "a werewolf";
                    BaseSoundID = 0xE5;

                    ActiveSpeed = .3;
                    CurrentSpeed = .3;

                    PublicOverheadMessage(MessageType.Regular, 0, true, "*Fur and claws burst forth as the man turns to beast*");

                    Timer.DelayCall(TimeSpan.FromSeconds(30), delegate ()
                    {
                        if (!this.Alive || this.Deleted) return;

                        frenzied = false;

                        SetSkill(SkillName.Wrestling, 90);
                        SetDamage(25, 35);
                        AttackSpeed = 30;
                        Body = 400;
                        Hue = 1522;
                        Name = "a rather hairy man";
                        HairItemID = 8252;
                        HairHue = 1117;
                        BaseSoundID = 0x45A;

                        ActiveSpeed = .4;
                        CurrentSpeed = .4;

                        PublicOverheadMessage(MessageType.Regular, 0, true, "*the beast's frenzy subsides*");
                    });
                }
            }
        }

        public override bool OnBeforeDeath()
        {
            if (Global_AllowAbilities)
            {
                SetSkill(SkillName.Wrestling, 90);
                SetDamage(25, 35);
                AttackSpeed = 30;
                Body = 400;
                Hue = 33774;
                Name = "a rather hairy man";
                HairItemID = 8252;
                HairHue = 1117;

                ActiveSpeed = .4;
                CurrentSpeed = .4;
            }

            return base.OnBeforeDeath();
        }

        public HalloweenWerewolf(Serial serial) : base(serial)
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

            if (Global_AllowAbilities)
            {
                frenzied = false;

                SetSkill(SkillName.Wrestling, 90);
                SetDamage(25, 35);
                AttackSpeed = 30;
                Body = 400;
                Hue = 33774;
                HairItemID = 8252;
                HairHue = 1117;
                Name = "a rather hairy man";

                ActiveSpeed = .4;
                CurrentSpeed = .4;
            }
        }
    }
}