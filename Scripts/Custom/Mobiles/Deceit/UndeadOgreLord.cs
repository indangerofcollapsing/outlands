using Server.Achievements;
using Server.Items;
using Server.Mobiles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an undead ogre lord's corpse")]
    public class UndeadOgreLord : BaseCreature
    {
        [Constructable]
        public UndeadOgreLord(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an undead ogre lord";
            Body = 83;
            BaseSoundID = 427;
            Hue = 1893;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(750);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;            
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.2;
        }

        private bool spawnedZombies = false;

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 2; } }    

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (Global_AllowAbilities)
            {
                double hitsPercent = (double)Hits / (double)HitsMax;

                if (hitsPercent <= .5 && !spawnedZombies)
                    SpawnZombies();
            }
        }

        private void SpawnZombies()
        {
            PublicOverheadMessage(Server.Network.MessageType.Emote, Hue, true, "*flaming zombies crawl out of the monster's stomach*");

            int creatures = Utility.RandomMinMax(3, 3);

            for (int a = 0; a < creatures; a++)
            {
                List<Point3D> m_Locations = SpecialAbilities.GetSpawnableTiles(Location, false, true, Location, Map, 1, 10, 1, 2, true);

                Point3D newLocation = new Point3D();

                if (m_Locations.Count > 0)
                    newLocation = m_Locations[0];
                else
                    newLocation = Location;

                FlamingZombie flamingZombie = new FlamingZombie();
                flamingZombie.MoveToWorld(newLocation, Map);

                new Blood().MoveToWorld(flamingZombie.Location, Map);                
            }

            spawnedZombies = true;           
        }

        public override bool OnBeforeDeath()
        {
            AwardDailyAchievementForKiller(PvECategory.KillUndeadOgreLords);

            return base.OnBeforeDeath();
        }        

        public UndeadOgreLord(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(spawnedZombies);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            spawnedZombies = reader.ReadBool();
        }
    }
}
