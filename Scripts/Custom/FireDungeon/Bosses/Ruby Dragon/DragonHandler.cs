using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    public class DragonHandler : BaseCreature
    {
        public RubyDragon Dragon { get; set; }
        public override bool AlwaysBoss { get { return true; } }

        public override bool ClickTitle { get { return false; } }
        public override bool BardImmune { get { return true; } }

        private InternalTimer m_Timer;

        [Constructable]
        public DragonHandler(double difficultyMultiplier = 1.0)
            : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "the dragon handler";
            Hue = Utility.RandomSkinHue();

            this.Female = true;

            Body = 0x191;
            Name = NameList.RandomName("female");
            AddItem(new Skirt(Utility.RandomNeutralHue()));

            SetStr(86, 100);
            SetDex(81, 95);
            SetInt(61, 75);

            SetHits(3200, 3300);
            SetHits((int)Math.Ceiling(Hits * difficultyMultiplier));
            SetDamage((int)Math.Ceiling(16.0 * difficultyMultiplier), (int)Math.Ceiling(22.0 * difficultyMultiplier));

            SetSkill(SkillName.Macing, 95.0, 107.5);
            SetSkill(SkillName.MagicResist, 105.0, 107.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 15.0, 37.5);

            Fame = 1000;
            Karma = -1000;

            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new FancyShirt());
            AddItem(new QuarterStaff());

            Utility.AssignRandomHair(this);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }

        //public override int GoldWorth { get { return Utility.RandomMinMax(25, 40); } }
        //public override double MaxSkillScrollWorth { get { return 75.0; } }
        public override bool AlwaysMurderer { get { return true; } }

        public DragonHandler(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeHarmful(Mobile target)
        {
            return !(target is RubyDragon) && base.CanBeHarmful(target);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(Dragon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Dragon = reader.ReadMobile() as RubyDragon;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        private bool m_Moving;

        public override bool Move(Direction d)
        {
            if (Dragon != null)
            {
                double dist = GetDistanceToSqrt(Dragon.Location);

                if (m_Moving)
                {
                    if (dist > 1)
                        m_Moving = false;
                    else
                        d = GetDirectionTo(Dragon);
                }
                else if (dist > 5)
                {
                    m_Moving = true;
                    d = GetDirectionTo(Dragon);
                }
            }

            return base.Move(d);
        }

        public void BeginStare()
        {
            if (Dragon == null || Dragon.Deleted)
                return;

            if (AIObject != null)
                AIObject.AcquireFocusMob(true);

            if (AIObject != null && Combatant != null && FocusMob != null && FocusMob != Dragon)
            {
                Frozen = true;
                SpellHelper.Turn(this, FocusMob);
                Emote(String.Format("Glares at {0}", FocusMob.Name));
                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (Dragon == null) return;
                    if (Dragon.Deleted || !Dragon.Alive) return;
                    if (FocusMob == null) return;
                    if (FocusMob.Deleted || !FocusMob.Alive) return;
                    
                    Frozen = false;
                    Dragon.FocusedFlamestrike(FocusMob);
                }
                );
            }
        }

        private class InternalTimer : Timer
        {
            private DateTime m_NextTarget;
            private DragonHandler m_Handler;

            public InternalTimer(DragonHandler handler)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                m_NextTarget = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                m_Handler = handler;
            }

            protected override void OnTick()
            {
                if (m_Handler == null || m_Handler.Deleted || m_Handler.Dragon == null || m_Handler.Dragon.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_NextTarget < DateTime.UtcNow)
                {
                    m_Handler.BeginStare();
                    m_NextTarget = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 30));
                }
            }
        }
    }
}
