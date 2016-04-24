using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server
{
    [CorpseName("a pit tentacle corpse")]
    public class UOACZPitTentacle : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "",
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

        public override int DifficultyValue { get { return 3; } }

		[Constructable]
		public UOACZPitTentacle() : base()
		{
            Name = "a pit tentacle";

            Body = 8;
            Hue = 2052;
            BaseSoundID = 684;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 0;                     
		}

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);            

            if (Utility.RandomDouble() <= .15)
            {
                if (defender == null) return;
                if (defender.Deleted || !defender.Alive) return;

                Point3D location = defender.Location;
                Map map = defender.Map;

                double belowDuration = 15;

                double damage = 30;

                if (defender is BaseCreature)
                    damage *= 2;

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, -1, true, "", "", "-1");
                SpecialAbilities.HinderSpecialAbility(1.0, null, defender, 1.0, belowDuration, false, -1, false, "", "You have been 'taken below' and cannot move or speak!", "-1");

                Squelched = true;

                defender.Squelched = true;
                defender.Hidden = true;

                Timer.DelayCall(TimeSpan.FromSeconds(belowDuration), delegate
                {
                    if (defender == null) return;
                    if (defender.Deleted) return;

                    defender.Squelched = false;
                    defender.Hidden = false;
                });

                Blessed = true;

                Effects.PlaySound(location, defender.Map, 0x246); //0x0FB

                PublicOverheadMessage(MessageType.Regular, 0, false, "*takes them down below...*");
                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, belowDuration, false, -1, false, "", "", "-1");

                Effects.SendLocationParticles(EffectItem.Create(location, defender.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2051, 0, 5029, 0);

                TimedStatic floorHole = new TimedStatic(7025, belowDuration + 1);
                floorHole.Name = "pit to below";
                floorHole.MoveToWorld(location, defender.Map);

                for (int a = 0; a < 6; a++)
                {
                    TimedStatic pitPlasm = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), belowDuration - 1);
                    pitPlasm.Name = "pit plasm";
                    pitPlasm.Hue = 2052;

                    Point3D pitPlasmLocation = new Point3D(location.X + Utility.RandomList(-2, 2), location.Y + Utility.RandomList(-2, 2), location.Z);
                    SpellHelper.AdjustField(ref pitPlasmLocation, defender.Map, 12, false);

                    pitPlasm.MoveToWorld(pitPlasmLocation, defender.Map);
                }

                IEntity pitLocationEntity = new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z), defender.Map);
                Effects.SendLocationParticles(pitLocationEntity, 0x3709, 10, 60, 2053, 0, 5052, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (this == null) return;
                    if (Deleted || !Alive) return;

                    MoveToWorld(location, map);

                    for (int a = 0; a < 20; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                        {
                            if (this == null) return;
                            if (Deleted || !Alive) return;

                            Z--;
                        });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(2.1), delegate
                    {
                        if (this == null) return;
                        if (Deleted) return;

                        Blessed = false;
                        Delete();
                    });
                });
            }
        }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public UOACZPitTentacle(Serial serial): base(serial)
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
