using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a net gremlin's corpse")]
    public class DeDOSNetGremlin : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextThrowingNetAllowed;
        public TimeSpan NextThrowingNetDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40));

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    ""
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "",
                };
            }
        }

        [Constructable]
        public DeDOSNetGremlin(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a net gremlin";

            Body = 723;
            BaseSoundID = 422;

            Hue = 23999;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(200);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 0; 
        }

        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1;
            RangePerception = 18;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                else
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (Combatant != null)
            {
                if (DateTime.UtcNow >= m_NextThrowingNetAllowed)
                {
                    m_NextThrowingNetAllowed = DateTime.UtcNow + NextThrowingNetDelay;

                    int effectRange = 10;

                    IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, effectRange);

                    int mobileCount = 0;

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (!Map.InLOS(Location, mobile.Location)) continue;
                        if (mobile.Hidden) continue;

                        mobileCount++;
                    }

                    nearbyMobiles.Free();
                    nearbyMobiles = Map.GetMobilesInRange(Location, effectRange);

                    List<Mobile> m_NearbyMobiles = new List<Mobile>();

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == this) continue;
                        if (!SpecialAbilities.MonsterCanDamage(this, mobile)) continue;
                        if (!Map.InLOS(Location, mobile.Location)) continue;
                        if (mobile.Hidden) continue;
                        if (Combatant != null)
                        {
                            if (mobileCount > 1 && mobile == Combatant)
                                continue;
                        }

                        m_NearbyMobiles.Add(mobile);
                    }

                    nearbyMobiles.Free();

                    if (m_NearbyMobiles.Count == 0)
                        return;

                    Mobile mobileTarget = m_NearbyMobiles[Utility.RandomMinMax(0, m_NearbyMobiles.Count - 1)];

                    Point3D location = Location;
                    Map map = Map;

                    Point3D mobileLocation = mobileTarget.Location;
                    Map mobileMap = mobileTarget.Map;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, 1, true, 0, false, "", "", "-1");

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 7), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobileTarget.X, mobileTarget.Y, mobileTarget.Z), mobileMap);

                    int itemId = 3530;
                    int itemHue = 2587;

                    Effects.SendMovingEffect(startLocation, endLocation, itemId, 5, 0, false, false, 0, 0);                   

                    double distance = Utility.GetDistance(location, mobileLocation);
                    double destinationDelay = (double)distance * .08;

                    double duration = Utility.RandomMinMax(3, 5);

                    if (mobileTarget is BaseCreature)
                        duration *= 1.5;

                    PublicOverheadMessage(MessageType.Regular, 2586, false, "* .net *");

                    SpecialAbilities.EntangleSpecialAbility(1.0, null, mobileTarget, 1, duration, 0, true, "", "You have been ensnared in a net!", "-1");

                    Effects.PlaySound(location, map, Utility.RandomList(0x5D2, 0x5D3));

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (mobileTarget == null) return;
                        if (mobileTarget.Deleted || !mobileTarget.Alive) return;
                        if (Utility.GetDistance(location, mobileTarget.Location) >= 30) return;

                        for (int a = 0; a < 9; a++)
                        {
                            TimedStatic net = new TimedStatic(3538, duration - destinationDelay);
                            Point3D netLocation = mobileTarget.Location;

                            switch (a)
                            {
                                //Row 1
                                case 0:
                                    net.ItemID = 3538;
                                    netLocation = new Point3D(netLocation.X - 1, netLocation.Y - 1, netLocation.Z);
                                    break;

                                case 1:
                                    net.ItemID = 3528;
                                    netLocation = new Point3D(netLocation.X, netLocation.Y - 1, netLocation.Z);
                                    break;

                                case 2:
                                    net.ItemID = 3537;
                                    netLocation = new Point3D(netLocation.X + 1, netLocation.Y - 1, netLocation.Z);
                                    break;

                                //Row 2
                                case 3:
                                    net.ItemID = 3539;
                                    netLocation = new Point3D(netLocation.X - 1, netLocation.Y, netLocation.Z);
                                    break;

                                case 4:
                                    net.ItemID = 3530;
                                    netLocation = new Point3D(netLocation.X, netLocation.Y, netLocation.Z);
                                    break;

                                case 5:
                                    net.ItemID = 3531;
                                    netLocation = new Point3D(netLocation.X + 1, netLocation.Y, netLocation.Z);
                                    break;

                                //Row 3
                                case 6:
                                    net.ItemID = 3540;
                                    netLocation = new Point3D(netLocation.X - 1, netLocation.Y + 1, netLocation.Z);
                                    break;

                                case 7:
                                    net.ItemID = 3529;
                                    netLocation = new Point3D(netLocation.X, netLocation.Y + 1, netLocation.Z);
                                    break;

                                case 8:
                                    net.ItemID = 3541;
                                    netLocation = new Point3D(netLocation.X + 1, netLocation.Y + 1, netLocation.Z);
                                    break;
                            }

                            net.Hue = itemHue;
                            net.Name = "the net";
                            net.MoveToWorld(netLocation, mobileMap);
                        }
                    });
                }
            }
        }
                       
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetAngerSound() { return 0x5FF; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetDeathSound() { return 0x55B; }      

        public DeDOSNetGremlin(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}