using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    [CorpseName("a decayed zombie corpse")]
    public class UOACZDecayedZombie : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "brainnnnnnsss.....",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override BonePileType BonePile { get { return BonePileType.Small; } }

        public override int DifficultyValue { get { return 2; } }

		[Constructable]
		public UOACZDecayedZombie() : base()
		{
            Name = "a decayed zombie";
            Body = 3;
            BaseSoundID = 471;
            Hue = 2076; //1175;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 720;
            Karma = -600;

            Tameable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 5;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;

            ResolveAcquireTargetDelay = 3;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
            
            if (Utility.RandomDouble() < .33)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                TimedStatic corpsePart = new TimedStatic(Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600), 5);
                
                corpsePart.MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() < .33)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                TimedStatic corpsePart = new TimedStatic(Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600), 5);

                corpsePart.MoveToWorld(Location, Map);
            }            

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {            
            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

            int corpseItems = Utility.RandomMinMax(2, 3);

            for (int a = 0; a < corpseItems; a++)
            {
                Point3D point = new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z);

                TimedStatic corpsePart = new TimedStatic(Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600), 5);

                corpsePart.MoveToWorld(point, Map);

                new Blood().MoveToWorld(point, Map);
            }            

            return base.OnBeforeDeath();
        }

        public UOACZDecayedZombie(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
