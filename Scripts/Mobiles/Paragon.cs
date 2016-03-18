using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Regions;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Paragon
    {
        public static double ChestChance = .10;         // Chance that a paragon will carry a paragon chest

        private static TimeSpan FastRegenRate = TimeSpan.FromSeconds(.5);
        private static TimeSpan CPUSaverRate = TimeSpan.FromSeconds(2);

        private class ParagonStamRegen : Timer
        {
            private BaseCreature m_Owner;

            public ParagonStamRegen(Mobile m): base(FastRegenRate, FastRegenRate)
            {
                this.Priority = TimerPriority.FiftyMS;

                m_Owner = m as BaseCreature;
            }

            protected override void OnTick()
            {
                if (!m_Owner.Deleted && m_Owner.IsParagon && m_Owner.Map != Map.Internal)
                {
                    m_Owner.Stam++;

                    Delay = Interval = (m_Owner.Stam < (m_Owner.StamMax * .75)) ? FastRegenRate : CPUSaverRate;
                }
                else
                {
                    Stop();
                }
            }
        }

        public static Type[] Artifacts = new Type[]
		{
			typeof( DaemonBlood ), typeof( ArtifactVase ), 
			typeof( Basket6Artifact ), typeof( NoxCrystal ),
			typeof( EggCaseArtifact ), typeof( GraveDust ), typeof( NoxCrystal ), 
			typeof( BatWing )
		};

        public static int Hue = 0x501;

        public static double MaxHitsScalar = 3.0;
        public static double DamageScalar = 1.33;
        public static double CombatSkillsScalar = 1.25;
        public static double EvalIntScalar = 1.5;
        public static double MagicResistScalar = 1.5;
        public static double MeditationScalar = 3.0;

        public static void Convert(BaseCreature bc)
        {
            if (bc.IsParagon || bc.IPYLootTag == BaseCreature.EIPYLootTag.SEQ_SPAWN_MINIBOSS)
                return;

            bc.Hue = Hue;

            //Hits
            bc.SetHitsMax((int)((double)bc.HitsMax * MaxHitsScalar));
            bc.Hits = bc.HitsMax;

            //Damage
            bc.DamageMin = (int)((double)bc.DamageMin * DamageScalar);
            bc.DamageMax = (int)((double)bc.DamageMax * DamageScalar);

            //Skills           
            bc.Skills.Archery.Base *= CombatSkillsScalar;
            bc.Skills.Fencing.Base *= CombatSkillsScalar;
            bc.Skills.Macing.Base *= CombatSkillsScalar;
            bc.Skills.Swords.Base *= CombatSkillsScalar;
            bc.Skills.Wrestling.Base *= CombatSkillsScalar;
            bc.Skills.Parry.Base *= CombatSkillsScalar;

            bc.Skills.EvalInt.Base *= EvalIntScalar;
            bc.Skills.MagicResist.Base *= MagicResistScalar;
            bc.Skills.Meditation.Base *= MeditationScalar;

            if (bc.AIObject != null)
            {
                if (bc.AIObject.m_Timer != null)                
                    bc.AIObject.m_Timer.Priority = TimerPriority.FiftyMS;                
            }

            new ParagonStamRegen(bc).Start();
        }

        public static void UnConvert(BaseCreature bc)
        {
            if (!bc.IsParagon)
                return;

            bc.Hue = 0;

            //Hits
            bc.SetHitsMax((int)((double)bc.HitsMax / MaxHitsScalar));

            //Damage
            bc.DamageMin = (int)((double)bc.DamageMin / DamageScalar);
            bc.DamageMax = (int)((double)bc.DamageMax / DamageScalar);

            //Skills           
            bc.Skills.Archery.Base /= CombatSkillsScalar;
            bc.Skills.Fencing.Base /= CombatSkillsScalar;
            bc.Skills.Macing.Base /= CombatSkillsScalar;
            bc.Skills.Swords.Base /= CombatSkillsScalar;
            bc.Skills.Wrestling.Base /= CombatSkillsScalar;
            bc.Skills.Parry.Base /= CombatSkillsScalar;

            bc.Skills.EvalInt.Base /= EvalIntScalar;
            bc.Skills.MagicResist.Base /= MagicResistScalar;
            bc.Skills.Meditation.Base /= MeditationScalar;
        }

        public static bool CheckConvert(BaseCreature bc)
        {
            return CheckConvert(bc, bc.Location, bc.Map);
        }

        public static bool CheckConvert(BaseCreature bc, Point3D location, Map m)
        {
            if (!bc.AllowParagon || bc.Difficulty >= 50 || bc.Difficulty <= 0.5 || bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone || bc.IsParagon || bc.IPYLootTag == BaseCreature.EIPYLootTag.SEQ_SPAWN_MINIBOSS)
                return false;

            double regionMod = Server.Commands.RegionParagonMod.GetModifier(Region.Find(location, m));

            if (regionMod != 0.0)
            {
                return regionMod > Utility.RandomDouble();
            }

            double chance = 0.0075;

            if (Region.Find(location, m) is DungeonRegion)
                chance = 0.015;

            if (SpellHelper.IsHythlothDungeon(m, location) || SpellHelper.IsDeceitDungeon(m, location))
                chance = 0.03;

            return (chance > Utility.RandomDouble());
        }

        public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
        {
            //if ( !Core.AOS )
            //	return false;

            double fame = (double)bc.Fame;

            if (fame > 32000)
                fame = 32000;

            double chance = 1 / (Math.Max(10, 100 * (0.83 - Math.Round(Math.Log(Math.Round(fame / 6000, 3) + 0.001, 10), 3))) * (100 - Math.Sqrt(m.Luck)) / 100.0);

            return chance > Utility.RandomDouble();
        }

        public static void GiveArtifactTo(Mobile m)
        {
            Item item = (Item)Activator.CreateInstance(Artifacts[Utility.Random(Artifacts.Length)]);

            if (m.AddToBackpack(item))
                m.SendMessage("As a reward for slaying the mighty paragon, an artifact has been placed in your backpack.");
            else
                m.SendMessage("As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet.");
        }
    }
}
