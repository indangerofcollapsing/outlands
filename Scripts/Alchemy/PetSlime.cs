using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a slimey corpse")]
    public class PetSlime : BaseCreature
    {
        public override bool InitialInnocent{ get { return true; } }
        [Constructable]
        public PetSlime()
            : base(AIType.AI_Melee, FightMode.None, 20, 1, 0.2, 0.4)
        {
            Name = "a pet slime";
            Body = 51;
            BaseSoundID = 456;
            SetStr(22, 34);
            SetDex(16, 21);
            SetInt(16, 20);
            SetHits(15, 19);
            SetDamage(1, 5);
            SetDamageType(ResistanceType.Physical, 100);
            SetResistance(ResistanceType.Physical, 5, 10);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.Poisoning, 10);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 19.3, 34.0);
            SetSkill(SkillName.Wrestling, 19.3, 34.0);
            Fame = 300;
            Karma = -300;
            VirtualArmor = 8;

            Tamable = false;
            ControlSlots = 1;
            Blessed = true;
        }

        public override Poison PoisonImmune { get { return Poison.Lesser; } }
        public override Poison HitPoison { get { return Poison.Lesser; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish | FoodType.FruitsAndVegies | FoodType.GrainsAndHay | FoodType.Eggs; } }

        public PetSlime(Serial serial)
            : base(serial)
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
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Utility.RandomDouble() < .05)
            {
                base.OnThink();
            }
        }

        public override void OnThink()
        {
            base.OnThink();
            if ( Utility.RandomDouble() < .0025 )
            {
                double x = Utility.RandomDouble();
                switch (Utility.Random(7))
                {
                    case 0:
                        if (x < .10)
                        {
                            this.Say("Hijejij mheuhieuh <3");
                        }
                        this.Say("Hijejij mheuhieuh");
                        break;
                    case 1:
                        if (x < .10)
                        {
                            this.Say("Ieesrer Blaeewwwwwwwwwwwwwph <3");
                        }
                        this.Say("Ieesplbbbh Blaasph");
                        break;
                    case 2:
                        if (Utility.RandomDouble() < .10)
                        {
                            this.Say("3");
                        }
                        this.Say("ijdhhem uiiiooijerj");
                        break;
                    case 3:
                        if (Utility.RandomDouble() < .10)
                        {
                            this.Say("Fijfbfef dewwkm <3");
                        }
                        this.Say("Fijfb iuheokm");
                        break;
                    case 4:
                        if (Utility.RandomDouble() < .10)
                        {
                            this.Say("efhu ehfuh jruuu <3");
                        }
                        this.Say("jioej fiweoIO");
                        break;
                    case 5:
                        if (Utility.RandomDouble() < .10)
                        {
                            this.Say("<3");
                        }
                        this.Say("ofeije ijfieuyh");
                        break;
                    case 6:
                        if (Utility.RandomDouble() < .10)
                        {
                            this.Say("svooosh");
                        }
                        this.Say("okwelk riehuh oweirj");
                        break;
                }
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            Item bottle = m.Backpack.FindItemByType(typeof(Bottle));
            if (this.ControlMaster == m)
            {
                if (m.InRange(this.Location, 2))
                {
                    if (bottle != null)
                    {
                        Item item = new SummonSlimePotion();
                        item.Hue = this.Hue;
                        m.AddToBackpack(item);
                        bottle.Consume(1);
                        this.Delete();
                    }
                    else
                    {
                        m.SendMessage("You need an empty bottle to put it in.");
                    }
                }
                else
                {
                    m.SendMessage("You are not close enough to put that away.");
                }
            }
        }
    }
}