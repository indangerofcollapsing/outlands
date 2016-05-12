using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a grave robber's corpse")]
    public class GraveRobber : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextDigAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));
        public TimeSpan NextDigDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Did you hear something?",
                    "I have a bad feeling about this place.",
                    "This better be worth it, this place gives me the creeps."
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
                    "",
                    "",
                    "",
                    "",
                    "", 
                };
            }
        }
        
        [Constructable]
        public GraveRobber(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {            
            Name = "a grave robber";
            Hue = Utility.RandomSkinHue(); 
            
            if (Female = Utility.RandomBool())            
                Body = 0x191;            

            else   
                Body = 0x190;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(225);

            SetDamage(15, 25);

            SetSkill(SkillName.Swords, 85);
            SetSkill(SkillName.Macing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 500;
            Karma = -3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
           
            AddItem(new ShortPants() { Movable = true, Hue = Utility.RandomNeutralHue()});           
            AddItem(new StuddedGloves() { Movable = true, Hue = 0 });  
            AddItem(new Pickaxe() { Movable = true, Hue = 0 });
            AddItem(new Torch());          
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
          
            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextDigAllowed && Combatant == null)
            {
                Effects.PlaySound(this.Location, this.Map, Utility.RandomList(0x125, 0x126));
                Animate(Utility.RandomList(11, 12), 5, 1, true, false, 0);

                AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(2);
                LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(2);

                //Dirt
                for (int a = 0; a < 2; a++)
                {
                    Blood dirt = new Blood();
                    dirt.Name = "dirt";
                    dirt.ItemID = Utility.RandomList(7681, 7682);

                    Point3D dirtLocation = new Point3D(X + Utility.RandomMinMax(-1, 1), Y + Utility.RandomMinMax(-1, 1), Z);

                    dirt.MoveToWorld(dirtLocation, Map);
                }

                m_NextDigAllowed = DateTime.UtcNow + NextDigDelay;
            }            
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public GraveRobber(Serial serial): base(serial)
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