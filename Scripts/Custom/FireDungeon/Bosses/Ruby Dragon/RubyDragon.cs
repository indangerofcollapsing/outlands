using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Fourth;
namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class RubyDragon : BaseCreature
    {
        RubyDragonAttackTimer timer;
        public override bool AlwaysBoss { get { return true; } }

        [Constructable]
        public RubyDragon(double difficultyMultiplier)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a ruby dragon";
            Body = 0x3B;
            BaseSoundID = 362;
            Hue = 1194;

            SetStr(896, 1225);
            SetDex(136, 205);
            SetInt(436, 475);

            SetHits(4600, 4800);
            SetHits((int)Math.Ceiling(Hits * difficultyMultiplier));

            SetDamage((int)Math.Ceiling(16.0 * difficultyMultiplier), (int)Math.Ceiling(22.0 * difficultyMultiplier));

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 99.0);
            SetSkill(SkillName.MagicResist, 99.1, 100.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 92.5);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 80;

            timer = new RubyDragonAttackTimer(this);
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;

            UniqueCreatureDifficultyScalar = 1.5;
        }

        public override void OnActionCombat()
        {
            if (!timer.Running)
                timer.Start();

            base.OnActionCombat();
        }

        public override void OnActionWander()
        {
            if (timer.Running)
                timer.Stop();

            base.OnActionWander();
        }

        public override void OnDelete()
        {
            if (timer.Running)
                timer.Stop();

            base.OnAfterDelete();
        }

        public override bool CanBeHarmful(Mobile target, bool message)
        {
            return !(target is DragonHandler) && base.CanBeHarmful(target, message);
        }

        public void FocusedFlamestrike(Mobile target)
        {
            if (target != null)
            {
                List<Mobile> targets = this.AcquireSpellTargets(3, 1);
                
                if (targets == null)
                    return;

                if (targets.Count < 1)
                {
                    if (Utility.RandomBool())
                        UltraFirefield();
                    else
                        MassFireball();
                }

                SpellHelper.Turn(this, target);

                for (int i = 0; i < targets.Count; i++)
                {
                    Mobile mob = targets[i];
                    if (mob == null) continue;
                    
                    this.PublicOverheadMessage(Server.Network.MessageType.Emote, this.Hue, true, "Inhales Deeply");

                    SpellHelper.CheckReflect(8, this, ref mob);

                    double damage, burns;

                    damage = Utility.Random(55, 11);
                    burns = Utility.Random(2, 3);

                    mob.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                    mob.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.RightFoot);
                    mob.PlaySound(0x208);

                    SpellHelper.Damage(TimeSpan.Zero, mob, damage);
                    
                    for (int x = 0; x < burns; x++)
                    {
                        SpellHelper.Damage(TimeSpan.FromSeconds(1 + x), mob, Utility.Random(5, 4));
                    }
                }
            }
        }

        public void UltraFirefield()
        {
            List<Mobile> targets = this.AcquireSpellTargets(8, 8);

            this.PublicOverheadMessage(Server.Network.MessageType.Emote, this.Hue, true, "Stomps");
            foreach (Mobile t in targets)
            {
                new FireFieldSpell.FireFieldItem(0x398C, t.Location, this, t.Map, TimeSpan.FromSeconds(Utility.Random(20, 20)), Utility.Random(20, 10), Utility.Random(20, 20));
            }
        }

        public void MassFireball()
        {
            List<Mobile> targets = this.AcquireSpellTargets(6, 6);

            this.PublicOverheadMessage(Server.Network.MessageType.Emote, this.Hue, true, "Roars");

            foreach (Mobile t in targets)
            {
                this.MovingParticles(t, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                this.PlaySound(Core.AOS ? 0x15E : 0x44B);
                SpellHelper.Damage(TimeSpan.FromSeconds(1), t, Utility.Random(40, 15));
                var innerMobiles = t.GetMobilesInRange(2);
                foreach (Mobile y in innerMobiles)
                {
                    t.MovingParticles(y, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                    t.PlaySound(Core.AOS ? 0x15E : 0x44B);
                    SpellHelper.Damage(TimeSpan.FromSeconds(3), y, Utility.Random(20, 15));
                }
                innerMobiles.Free();
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.RandomMinMax(0, 3))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new ChainLightningScroll())); } break;
                case 1: { c.AddItem(new HedgeSmallTrimmed()); } break;
                case 2: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Legs)); } break;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 1);
            AddLoot(LootPack.Gems, 30);
        }
      
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }

        public RubyDragon(Serial serial)
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
            timer = new RubyDragonAttackTimer(this);
        }
    }

    public class RubyDragonAttackTimer : Timer
    {
        private RubyDragon m_Spawn;

        public RubyDragonAttackTimer(RubyDragon spawn)
            : base(TimeSpan.Zero, TimeSpan.FromSeconds(15))
        {
            m_Spawn = spawn;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if (m_Spawn == null || m_Spawn.Deleted)
                this.Stop();
            else
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        m_Spawn.MassFireball();
                        break;
                    case 1:
                        m_Spawn.UltraFirefield();
                        break;
                }
            }
        }
    }
}
