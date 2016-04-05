using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;

using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a bloodstalker corpse")]
    public class BloodStalker : BaseCreature
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15);

        [Constructable]
        public BloodStalker(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bloodstalker";
            Body = 734;
            Hue = 2118;
            BaseSoundID = 660;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(350);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);           

            VirtualArmor = 50; 

            Fame = 1000;
            Karma = -1000;
        }

        public override bool AlwaysBossMinion { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.05;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 1;
        }

        public override void OnThink()
        {
            base.OnThink();
           
            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    Point3D originalLocation = Location;

                    if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))
                    {
                        int projectiles = 6;
                        int particleSpeed = 8;
                        double distanceDelayInterval = .12;

                        int minRadius = 1;
                        int maxRadius = 5;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(originalLocation, true, false, originalLocation, Map, projectiles, 20, minRadius, maxRadius, false);

                        if (m_ValidLocations.Count == 0)
                            return;

                        for (int c = 0; c < projectiles; c++)
                        {
                            Point3D bloodLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 2), Map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(bloodLocation.X, bloodLocation.Y, bloodLocation.Z + 50), Map);

                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), particleSpeed, 0, false, false, 0, 0);
                        }
                    }                    

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public BloodStalker(Serial serial): base(serial)
        {
        }

        protected override bool OnMove(Direction d)
        {            
            for (int a = 0; a < 1; a++)
            {
                new Blood().MoveToWorld(new Point3D(this.X + Utility.RandomMinMax(-1, 1), this.Y + Utility.RandomMinMax(-1, 1), this.Z), this.Map);
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
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