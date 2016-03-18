using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Spells.Spellweaving;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a golden balron corpse")]
    public class GoldenBalron : BaseCreature
    {
        [Constructable]
        public GoldenBalron(): base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.8, 0.9)
        {
            Name = "the golden balron";
            Body = 40;
            BaseSoundID = 357;
            Hue = 0x8A5;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(15000);
            SetStam(5000);
            SetMana(10000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 115);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 75000;
            Karma = -75000;
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.33;
        }
             
        public override bool AlwaysMurderer { get { return true; } }        
        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 50; } }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (Global_AllowAbilities)
            {
                if (from == null) return;

                if (amount > 10 && (from is BaseCreature) && (((BaseCreature)from).ControlMaster != null))
                {
                    bool changeTarget = Utility.RandomBool();
                    if (changeTarget)
                    {
                        Combatant = ((BaseCreature)from).ControlMaster;
                        Say("I see you there.");
                    }
                }

                int harmCaster = Utility.Random(5);
                if (harmCaster == 0)
                {
                    int tiles = Utility.RandomMinMax(1, 3);
                    int damage = Utility.RandomMinMax(12, 18);

                    List<Mobile> targets = GetTargets(from, tiles);

                    foreach (Mobile m in targets)
                    {
                        if (m == null || m.Deleted)
                            continue;

                        m.Damage(damage);
                    }

                    int duration = 5;

                    if (from.Map != null)
                    {
                        for (int x = from.X - tiles; x <= from.X + tiles; x += tiles)
                        {
                            for (int y = from.Y - tiles; y <= from.Y + tiles; y += tiles)
                            {
                                if (from.X == x && from.Y == y)
                                    continue;

                                Point3D p3d = new Point3D(x, y, from.Map.GetAverageZ(x, y));

                                if (from.Map.CanFit(p3d, 12, true, false))
                                    new FireItem(duration).MoveToWorld(p3d, from.Map);

                            }
                        }
                    }

                    Say("You will all burn!");
                }
            }
        }

        public class FireItem : Item
        {
            public FireItem(int duration): base(Utility.RandomBool() ? 0x398C : 0x3996)
            {
                Movable = false;
                Timer.DelayCall(TimeSpan.FromSeconds(duration), new TimerCallback(Delete));
            }

            public FireItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }

        private List<Mobile> GetTargets(Mobile caster, int range)
        {
            var targets = new List<Mobile>();

            IPooledEnumerable eable = Map.GetMobilesInRange(caster.Location, range);

            foreach (Mobile m in eable)
            {
                if (m == null || m.Deleted) continue;

                if (m != this && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                    targets.Add(m);
            }

            eable.Free();

            return targets;
        }

        public override bool OnBeforeDeath()
        {

            BaseWeapon weapon = null;
            switch (Utility.Random(6))
            {
                case 0: weapon = new Halberd(); break;
                case 1: weapon = new Katana(); break;
                case 2: weapon = new QuarterStaff(); break;
                case 3: weapon = new Bow(); break;
                case 4: weapon = new Kryss(); break;
                case 5: weapon = new Bow(); break;
            }

            if (Utility.RandomBool())
            {
                weapon.Hue = 2103;
                weapon.Slayer = SlayerName.Silver;
            }

            else
            {
                weapon.Hue = 0x8A5;
                if (Utility.RandomBool())
                    weapon.Slayer = SlayerName.DaemonDismissal;
                else
                    weapon.Slayer = SlayerName.BalronDamnation;
            }

            weapon.DamageLevel = (WeaponDamageLevel)RandomMinMaxScaled(1, 5);
            weapon.AccuracyLevel = (WeaponAccuracyLevel)RandomMinMaxScaled(1, 5);
            weapon.DurabilityLevel = (WeaponDurabilityLevel)RandomMinMaxScaled(0, 5);

            PackItem(weapon);

            int randval = Utility.Random(8);
            if (randval == 0)
            {
                PackItem(new TribalMask(2213));
            }
            else if (randval == 1)
            {
                PackItem(new IronMaidenDeed() { LootType = LootType.Regular }); // 
            }
            else if (randval == 2)
            {
                PackItem(new Item(Utility.RandomBool() ? 0x1B1D : 0x1B1E));
            }
            else if (randval == 4)
            {
                Item i = new Item(0x20D3);
                i.Hue = 0x8A5;
                i.Name = "golden balron statue";
                AddItem(i);
            }
            else
            {
                PackItem(new TreasureMap(6));
            }

            return base.OnBeforeDeath();
        }               

        public GoldenBalron(Serial serial): base(serial)
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