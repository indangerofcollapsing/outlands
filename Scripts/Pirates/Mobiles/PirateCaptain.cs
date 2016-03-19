using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;

namespace Server.Custom.Pirates
{
    public class PirateCaptain : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public string[] idleSpeech
        {
            get
            {
                return new string[] {       "Let none alive take the wind from our sails",
                                            "Ho, ho! The sea's our mistress and she be a good lass to us",
                                            "A pirate's takes what he wants and gives nothing back!",
                                            "Booty and plunder for us, misery and death to the rest!" 
                                            };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[] {     "Give em' what-for, men!",
                                          "Leave none alive!",
                                          "Gutless cowards, all of em!",
                                          "Har, har!" 
                                          };
            }
        }

        [Constructable]
        public PirateCaptain(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 0x22;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                Title = "the pirate captain";
            }

            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                Title = "the pirate captain";
            }

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(600);

            SetDamage(20, 30);

            SetSkill(SkillName.Archery, 100);
            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Macing, 100);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

            Fame = 3000;
            Karma = -3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new LongPants(Utility.RandomDyedHue())); break;
                case 2: AddItem(new ShortPants(Utility.RandomDyedHue())); break;
            }

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new StuddedChest()); break;
                case 2: AddItem(new RingmailChest()); break;
            }

            AddItem(new Cloak(Utility.RandomDyedHue()));
            AddItem(new TricorneHat(Utility.RandomDyedHue()));
            AddItem(new StuddedGorget());
            AddItem(new StuddedArms());
            AddItem(new BoneGloves());
            AddItem(new ThighBoots());

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { AddItem(new Cutlass()); AddItem(new MetalShield()); break; }
                case 2: { AddItem(new Scimitar()); AddItem(new MetalShield()); break; }
                case 3: { AddItem(new WarFork()); AddItem(new MetalShield()); break; }
                case 4: { AddItem(new HammerPick()); AddItem(new MetalShield()); break; }
                case 5: { AddItem(new ExecutionersAxe()); break; }
            }

            PackItem(new Crossbow() { Movable = true, Hue = 0 });
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 0.95;
        }

        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public PirateCaptain(Serial serial): base(serial)
        {
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
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}