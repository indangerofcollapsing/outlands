using System;
using Server;
using Server.Items;
using Server.Targeting;

using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a mysterious sphinx corpse")]
    public class MysterySphinx : BaseCreature
    {
        public bool m_MasterSphinx = false;
        public List<MysterySphinx> m_Duplicates = new List<MysterySphinx>();

        public int m_PossibleHues = Utility.RandomList(2587, 2510, 2614, 2602, 2500, 2967);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public int m_Transformations = 0;
        public static int m_MinimumTransformations = 3;

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "Perhaps you believe you have found my true form, yes?",
                    "You are so very, very close.",
                    "Getting warmer.",
                    "Maybe this one, perhaps?",
                    "You would like a hint, maybe?",
                    "Mayhaps a simpler riddle for you, instead.", 
                };
            }
        }

        [Constructable]
        public MysterySphinx(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mysterious sphinx";

            Body = 293;
            Hue = m_PossibleHues;
            BaseSoundID = 0x289;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(1000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 100;

            Fame = 1000;
            Karma = -1000;

            //--------------

            m_AncientMysteryCreature = true;
        }

        public override bool BardImmune { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override TimeSpan AutoDispelCooldown { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(4, 6)); } }
        public override int PoisonResistance { get { return 5; } }
        public override bool AlwaysMurderer { get { return true; } }
                
        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 9;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 18;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .10;

            SpecialAbilities.CrippleSpecialAbility(effectChance, this, defender, .25, 10, -1, true, "", "The creature has transfixed you with it's gaze, slowing your actions!");            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (m_Transformations < m_MinimumTransformations)
            {
                if (amount > 10 && Utility.RandomDouble() <= .25)
                {
                    m_Transformations++;
                    TransformAndRestore();

                    return;
                }
            }

            else
            {
                if (!m_MasterSphinx)
                {
                    if (willKill)
                    {
                    }

                    else if (amount > 10 && Utility.RandomDouble() <= .25)
                        TransformAndRestore();

                    return;
                }

                else
                {
                    if (amount > 10 && Utility.RandomDouble() <= .05)
                        Say("*looks nervous*");
                }
            }                       

            base.OnDamage(amount, from, willKill);
        }

        public void TransformAndRestore()
        {
            if (m_Duplicates.Count == 0)            
                return;            

            List<Point3D> m_Locations = new List<Point3D>();

            for (int a = 0; a < m_Duplicates.Count; a++)
            {
                MysterySphinx mysterySphinx = m_Duplicates[a];

                mysterySphinx.Restore();
                m_Locations.Add(mysterySphinx.Location);
            }

            int locationCount = m_Locations.Count;

            if (m_Locations.Count > 0)
            {
                for (int a = 0; a < m_Duplicates.Count; a++)
                {
                    int index = Utility.RandomMinMax(0, m_Locations.Count - 1);

                    m_Duplicates[a].Location = m_Locations[index];
                    m_Locations.RemoveAt(index);
                }
            }

            for (int a = 0; a < m_Duplicates.Count; a++)
            {
                CustomizationAbilities.Vanish(m_Duplicates[a]);

                m_Duplicates[a].m_MasterSphinx = false;
                m_Duplicates[a].Say("Sorry, try again.");
                m_Duplicates[a].m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            MysterySphinx newMasterSphinx = m_Duplicates[Utility.RandomMinMax(0, m_Duplicates.Count - 1)];
            newMasterSphinx.m_MasterSphinx = true;      
        }

        public void Restore()
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(100), delegate
            {
                if (this == null) return;
                if (Deleted) return;

                Hits += HitsMax - Hits;
                Stam += StamMax - Stam;
                Mana += ManaMax - Mana;

                Paralyzed = false;
                CantWalk = false;
                Poison = null;

                Hue = Utility.RandomList(2587, 2515, 2614, 2592, 2500, 2210);
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant != null)
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }
        }

        public override bool OnBeforeDeath()
        {
            if (m_MasterSphinx)            
                Boss = true;  

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_MasterSphinx)
            {
                c.AddItem(new SphinxStatue());

                int treasurePileLevel = 1;

                if (Utility.RandomMinMax(1, 3) == 1)
                    treasurePileLevel = 2;

                if (Utility.RandomMinMax(1, 6) == 1)
                    treasurePileLevel = 3;

                if (Utility.RandomMinMax(1, 10) == 1)
                    treasurePileLevel = 4;

                switch (treasurePileLevel)
                {
                    case 1: c.AddItem(new TreasurePileSmallAddonDeed()); break;
                    case 2: c.AddItem(new TreasurePileMediumAddonDeed()); break;
                    case 3: c.AddItem(new TreasurePileLargeAddonDeed()); break;
                    case 4: c.AddItem(new TreasurePileHugeAddonDeed()); break;
                }

                List<MysterySphinx> m_OtherMysterySphinx = new List<MysterySphinx>();

                foreach (MysterySphinx MysterySphinx in m_Duplicates)
                {
                    if (MysterySphinx != this)
                        m_OtherMysterySphinx.Add(MysterySphinx);
                }

                int MysterySphinxCount = m_OtherMysterySphinx.Count;
                for (int a = 0; a < MysterySphinxCount; a++)
                {
                    m_OtherMysterySphinx[a].Delete();
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            List<MysterySphinx> m_OtherMysterySphinx = new List<MysterySphinx>();

            foreach (MysterySphinx MysterySphinx in m_Duplicates)
            {
                if (MysterySphinx != this)
                    m_OtherMysterySphinx.Add(MysterySphinx);
            }

            int MysterySphinxCount = m_OtherMysterySphinx.Count;
            for (int a = 0; a < MysterySphinxCount; a++)
            {
                m_OtherMysterySphinx[a].Delete();
            } 
        }

        public MysterySphinx(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_MasterSphinx);

            writer.Write(m_Duplicates.Count);
            foreach (BaseCreature creature in m_Duplicates)
            {
                writer.Write(creature);
            }

            writer.Write(m_Transformations);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_MasterSphinx = reader.ReadBool();

            m_Duplicates = new List<MysterySphinx>();

            //Version 0
            if (version >= 0)
            {
                int duplicateCount = reader.ReadInt();
                for (int a = 0; a < duplicateCount; a++)
                {
                    MysterySphinx MysterySphinx = reader.ReadMobile() as MysterySphinx;

                    if (MysterySphinx != null)
                        m_Duplicates.Add(MysterySphinx);
                }

                m_Transformations = reader.ReadInt();
            }
        }
    }
}