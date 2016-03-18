using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Achievements;
using System.Collections.Generic;

using Server.Targeting;
using Server.Custom.Ubercrafting;

namespace Server.Mobiles
{
    [CorpseName("the corpse of Belial")] // TODO: Corpse name?
    public class Belial : BaseCreature
    {
        [Constructable]
        public Belial()
            : base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.8, 0.9)
        {
            Name = "Belial";
            Body = 784;
            BaseSoundID = 1149;

            SetStr(5500, 6500);
            SetDex(140, 180);
            SetInt(12000, 15000);

            SetHits(12500, 13500);

            SetDamage(38, 42);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 85, 120);
            SetSkill(SkillName.Magery, 120, 120);
            SetSkill(SkillName.MagicResist, 100.5, 150.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 75000;
            Karma = -75000;

            VirtualArmor = 90;
            PackItem(new Bone(6));
        }

        public override bool AlwaysBoss { get { return true; } }

        public class ImpAdds : BaseCreature
        {
            [Constructable]
            public ImpAdds(): base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.8, 0.9)
            {
                Name = "Devilish Imp";
                Body = 74;
                BaseSoundID = 422;
                Hue = 37;

                SetStr(200, 320);
                SetDex(140, 180);
                SetInt(100, 200);

                SetHits(200, 320);

                SetDamage(5, 20);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 65, 75);
                SetResistance(ResistanceType.Fire, 80, 90);
                SetResistance(ResistanceType.Cold, 70, 80);
                SetResistance(ResistanceType.Poison, 60, 70);
                SetResistance(ResistanceType.Energy, 60, 70);

                SetSkill(SkillName.EvalInt, 85, 120);
                SetSkill(SkillName.Magery, 120, 120);
                SetSkill(SkillName.MagicResist, 100.5, 150.0);
                SetSkill(SkillName.Tactics, 97.6, 100.0);
                SetSkill(SkillName.Wrestling, 97.6, 100.0);


                Fame = 1000;
                Karma = -10000;

                VirtualArmor = 20;
                Timer.DelayCall(TimeSpan.FromMinutes(10), new TimerCallback(Delete));
            }
           
            public override bool AlwaysMurderer { get { return true; } }

            public override bool AllowParagon { get { return false; } }
            public override bool BardImmune { get { return true; } }  

            protected override void OnTargetChange()
            {

                if (Combatant != null && Combatant is BaseCreature && ((BaseCreature)Combatant).Controlled && ((BaseCreature)Combatant).ControlMaster != null)
                {
                    Combatant = ((BaseCreature)Combatant).ControlMaster;
                }

                base.OnTargetChange();
            }

            public ImpAdds(Serial serial): base(serial)
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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            if (from == null) return; // from can be null

            double chance = 0;

            if (amount > 0)
            {
                chance = Math.Log(amount) * 2;
            }


            if (Utility.Random(90) <= chance)
            {
                int tiles = Utility.RandomMinMax(1, 3);
                int damage = Utility.RandomMinMax(12, 18);
                bool bigexplosion = Utility.RandomBool();

                List<Mobile> targets = GetTargets(from, tiles, bigexplosion);

                if (targets.Count > 0)
                {
                    foreach (Mobile m in targets)
                    {

                        if (m != null)
                            m.Damage(damage);

                    }
                }

                int duration = (int)Math.Max(1, 3);

                for (int x = from.X - tiles; x <= from.X + tiles; x += tiles)
                {
                    for (int y = from.Y - tiles; y <= from.Y + tiles; y += tiles)
                    {
                        if (from.X == x && from.Y == y)
                            continue;

                        Point3D p3d = new Point3D(x, y, from.Map.GetAverageZ(x, y));

                        if (from.Map.CanFit(p3d, 12, true, false))
                            new ExplosionItem(duration).MoveToWorld(p3d, from.Map);

                    }
                }

                if (bigexplosion)
                {
                    for (int x = from.X - (tiles * 2); x <= from.X + (tiles * 2); x += (tiles * 2))
                    {
                        for (int y = from.Y - (tiles * 2); y <= from.Y + (tiles * 2); y += (tiles * 2))
                        {
                            if (from.X == x && from.Y == y)
                                continue;

                            Point3D p3d = new Point3D(x, y, from.Map.GetAverageZ(x, y));

                            if (from.Map.CanFit(p3d, 12, true, false))
                                new BlackHoleItem(duration).MoveToWorld(p3d, from.Map);

                        }
                    }
                }
                Say("Boom!");
                targets.Clear();
            }
            if (Utility.Random(150) <= chance)
            {
                if (from != null && this != null)
                {
                    Point3D p3d;

                    p3d = new Point3D((from.X - 1), (from.Y - 1), (from.Z));

                    new ImpAdds().MoveToWorld(p3d, this.Map);
                }
            }


        }

        public class ExplosionItem : Item
        {
            public ExplosionItem(int duration)
                : base(0x36B0) //(Utility.RandomBool() ? 0x398C : 0x3996)
            {
                Movable = false;
                Timer.DelayCall(TimeSpan.FromSeconds(duration), new TimerCallback(Delete));
            }

            public ExplosionItem(Serial serial)
                : base(serial)
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

        public class BlackHoleItem : Item
        {
            public BlackHoleItem(int duration)
                : base(0x3789) //(Utility.RandomBool() ? 0x398C : 0x3996)
            {
                Movable = false;
                Timer.DelayCall(TimeSpan.FromSeconds(duration), new TimerCallback(Delete));
            }

            public BlackHoleItem(Serial serial)
                : base(serial)
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

        private List<Mobile> GetTargets(Mobile caster, int range, bool modifier)
        {
            var targets = new List<Mobile>();
            int modifiedrange = range * (modifier ? 2 : 1);

            IPooledEnumerable eable = Map.GetMobilesInRange(caster.Location, modifiedrange);

            foreach (Mobile m in eable)
            {
                if (m == null) continue;

                if (m != this && SpellHelper.ValidIndirectTarget(this, m) && this.CanBeHarmful(m, false))
                    targets.Add(m);
            }

            eable.Free();

            return targets;

        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.Random(6))
            {
                case 0: { c.AddItem(new FjordjinGorget()); } break;
                case 1: { c.AddItem(new YigalrothHelm()); } break;
                case 2: { c.AddItem(new VashreLegs()); } break;
                case 3: { c.AddItem(new ApoxisGloves()); } break;
                case 4: { c.AddItem(new OrghereimChest()); } break;
                case 5: { c.AddItem(new HergamnonArms()); } break;
            }

            c.AddItem(new TreasureMap((Utility.RandomBool() ? 6 : 5), Map.Felucca));

            switch (Utility.Random(250))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new MassDispelScroll())); } break;
                case 1: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Shame, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Helmet)); } break;
                case 2: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Shame, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gorget)); } break;
                case 4: { c.AddItem(new ReactiveArmorScroll()); } break;
            }

            if (Utility.Random(200) == 0)
                c.AddItem(TitleDye.VeryRareTitleDye(Server.Custom.PlayerTitleColors.EVeryRareColorTypes.ClearBlueTitleHue));
        }

        public override int GetIdleSound() { return 0x19D; }
        public override int GetAngerSound() { return 0x175; }
        public override int GetDeathSound() { return 0x108; }
        public override int GetAttackSound() { return 0xE2; }
        public override int GetHurtSound() { return 0x28B; }

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

            if (weapon != null)
            {
                switch (Utility.Random(6))
                {
                    case 0:
                        {
                            weapon.Slayer = SlayerName.BalronDamnation;
                            break;
                        }
                    case 1:
                        {
                            weapon.Slayer = SlayerName.DaemonDismissal;
                            break;
                        }
                    case 2:
                        {
                            weapon.Slayer = SlayerName.Exorcism;
                            break;
                        }
                    case 3:
                        {
                            weapon.Slayer = SlayerName.Repond;
                            break;
                        }
                    case 4:
                        {
                            weapon.Slayer = SlayerName.BloodDrinking;
                            break;
                        }
                    case 5:
                        {
                            weapon.Slayer = SlayerName.DragonSlaying;
                            break;
                        }
                }
                weapon.Hue = 2707;
                weapon.DamageLevel = (WeaponDamageLevel)RandomMinMaxScaled(1, 5);
                weapon.AccuracyLevel = (WeaponAccuracyLevel)RandomMinMaxScaled(1, 5);
                weapon.DurabilityLevel = (WeaponDurabilityLevel)RandomMinMaxScaled(3, 5);
                PackItem(weapon);
            }


            PackItem(new GnarledStaff());
            PackScroll(3, 5);
            PackScroll(4, 5);
            PackReg(12, 22);
            AddLoot(LootPack.UncommonTitleDye);
            AddLoot(LootPack.WeaponDamageEnhancer);
            AddLoot(LootPack.RareTitleDye);


            return base.OnBeforeDeath();
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 1; } }

        public override bool AlwaysMurderer { get { return true; } }       

        public Belial(Serial serial): base(serial)
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
