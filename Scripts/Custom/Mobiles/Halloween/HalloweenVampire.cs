using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a vampire corpse")]
    public class HalloweenVampire : BaseCreature
    {
        [Constructable]
        public HalloweenVampire() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a vampire";
            Body = 400;
            HairItemID = 8252;
            HairHue = 1109;
            Hue = 0;
            BaseSoundID = 0x1C3;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(1000);

            SetDamage(26, 36);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 75;

            Fame = 3500;
            Karma = -25000;

            PackItem(new Bone(15));

            AddItem(new FancyShirt() { Movable = false, Hue = 1150 });
            AddItem(new LongPants() { Movable = false, Hue = 2052 });
            AddItem(new Cloak() { Movable = false, Hue = 1775 });
            AddItem(new GoldNecklace() { Movable = false });
            AddItem(new GoldRing() { Movable = false });
            AddItem(new BodySash() { Movable = false, Hue = 1779 });
            AddItem(new Boots() { Movable = false, Hue = 2052 });

        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysAttackable { get { return true; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.43;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {
                double effectChance = .25;

                if (Controlled && ControlMaster != null)
                {
                    if (ControlMaster is PlayerMobile)
                    {
                        if (defender is PlayerMobile)
                            effectChance = .02;
                        else
                            effectChance = .25;
                    }
                }

                if (Utility.RandomDouble() <= effectChance)
                {
                    int healingAmount = (int)((double)HitsMax * .10);
                    int stamrestoreAmount = (int)((double)StamMax * .20);

                    Hits += healingAmount;
                    Stam += stamrestoreAmount;

                    this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

                    Blood blood = new Blood();
                    blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);

                    SpecialAbilities.StunSpecialAbility(1.0, this, defender, .15, 10, -1, false, "", "");
                    SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, 0x44D, true, "", "The vampire sinks its fangs into you, siphoning your life to replenish itself!");
                }
            }
        }

        public HalloweenVampire(Serial serial) : base(serial)
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