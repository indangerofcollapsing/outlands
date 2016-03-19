using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;

namespace Server
{
    [CorpseName("an enveloping darkness corpse")]
    public class UOACZEnvelopingDarkness : UOACZBaseUndead
    {
        public override string[] idleSpeech
        {
            get
            {
                return new string[] {   "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };
            }
        }

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };
            }
        }

        List<Mobile> m_EngulfedMobiles = new List<Mobile>();

        public override BonePileType BonePile { get { return BonePileType.Medium; } }

        public override int DifficultyValue { get { return 9; } }
        
        [Constructable]
        public UOACZEnvelopingDarkness(): base()
        {
            Name = "enveloping darkness";
            Body = 780;
            Hue = 2250;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(800);

            SetDamage(13, 26);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 4500;
            Karma = -4500;

            CanSwim = true;
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ResolveAcquireTargetDelay = 1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .2)
            {
                if (defender == null) return;
                if (defender.Deleted || !defender.Alive) return;

                Point3D location = defender.Location;
                Map map = defender.Map;

                double belowDuration = 15;

                double damage = 45;                

                PublicOverheadMessage(MessageType.Regular, 0, false, "*engulfs target*");
                m_EngulfedMobiles.Add(defender);

                PlaySound(0x573);

                for (int a = 0; a < 6; a++)
                {
                    TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                    ichor.Hue = 2051;
                    ichor.Name = "ichor";

                    Point3D newPoint = new Point3D(defender.Location.X + Utility.RandomList(-1, 1), defender.Location.Y + Utility.RandomList(-1, 1), defender.Location.Z);
                    SpellHelper.AdjustField(ref newPoint, defender.Map, 12, false);

                    ichor.MoveToWorld(newPoint, defender.Map);
                }

                if (defender is BaseCreature)
                    damage *= 3;

                defender.Location = Location;

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, -1, true, "", "");
                SpecialAbilities.HinderSpecialAbility(1.0, null, defender, 1.0, belowDuration, false, -1, false, "", "You have been 'engulfed' and cannot move or speak!");

                defender.Squelched = true;
                defender.Hidden = true;

                Timer.DelayCall(TimeSpan.FromSeconds(belowDuration), delegate
                {
                    if (defender == null) return;
                    if (defender.Deleted) return;

                    defender.Squelched = false;
                    defender.Hidden = false;

                    if (!UOACZSystem.IsUOACZValidMobile(defender)) return;

                    Effects.PlaySound(defender.Location, defender.Map, Utility.RandomList(0x101));

                    for (int a = 0; a < 6; a++)
                    {
                        TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                        ichor.Hue = 2051;
                        ichor.Name = "ichor";

                        Point3D newPoint = new Point3D(defender.Location.X + Utility.RandomList(-1, 1), defender.Location.Y + Utility.RandomList(-1, 1), defender.Location.Z);
                        SpellHelper.AdjustField(ref newPoint, defender.Map, 12, false);

                        ichor.MoveToWorld(newPoint, defender.Map);
                    }

                    Effects.PlaySound(defender.Location, defender.Map, Utility.RandomList(0x101));

                    if (m_EngulfedMobiles.Contains(defender))
                        m_EngulfedMobiles.Remove(defender);
                });
            }
        }

        protected override bool OnMove(Direction d)
        {
            foreach (Mobile mobile in m_EngulfedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;
                if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;
                if (Utility.GetDistance(mobile.Location, Location) >= 12) continue;

                PlayerMobile player = mobile as PlayerMobile;

                if (player != null)
                {
                    if (!player.IsUOACZHuman)
                        continue;
                }                

                mobile.Location = Location;
            }

            if (Utility.RandomDouble() <= .33)
            {
                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x101));
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            foreach (Mobile mobile in m_EngulfedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                mobile.Squelched = false;
                mobile.Hidden = false;

                if (!UOACZSystem.IsUOACZValidMobile(mobile))
                    continue;   

                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.MoveToWorld(mobile.Location, mobile.Map);

                Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x101));
            }

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x300; }
        public override int GetIdleSound() { return 0x301; }
        public override int GetAttackSound() { return 0x302; }
        public override int GetHurtSound() { return 0x303; }
        public override int GetDeathSound() { return 0x304; }

        public UOACZEnvelopingDarkness(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_EngulfedMobiles.Count);
            
            for (int a = 0; a < m_EngulfedMobiles.Count; a++)
            {
                writer.Write(m_EngulfedMobiles[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int engulfedMobiles = reader.ReadInt();

            for (int a = 0; a < engulfedMobiles; a++)
            {
                m_EngulfedMobiles.Add((Mobile)reader.ReadMobile());
            }
        }
    }
}
