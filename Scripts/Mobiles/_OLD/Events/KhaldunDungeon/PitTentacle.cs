using System;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a pit tentacle corpse")]
    public class PitTentacle : BaseCreature
    {
        [Constructable]
        public PitTentacle() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a pit tentacle";

            Body = 8;
            Hue = 2052;
            BaseSoundID = 684;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2000;
            Karma = 0;                        
        }

        public override bool AlwaysEventMinion { get { return true; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.5;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (defender == null) return;
            if (defender.Deleted || !defender.Alive) return;            

            Point3D location = defender.Location;
            Map map = defender.Map;

            double belowDuration = 15;

            SpecialAbilities.HinderSpecialAbility(1.0, null, defender, 1.0, belowDuration, false, -1, false, "", "You have been 'taken below' and cannot move or speak!");

            defender.Squelched = true;
            defender.Hidden = true;

            Timer.DelayCall(TimeSpan.FromSeconds(15), delegate
            {
                if (defender == null) return;
                if (defender.Deleted) return;

                defender.Squelched = false;
                defender.Hidden = false;
            });

            Squelched = true;
            Blessed = true;

            Effects.PlaySound(location, defender.Map, 0x246); //0x0FB

            PublicOverheadMessage(MessageType.Regular, 0, false, "*takes them down below...*");
            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, belowDuration, false, -1, false, "", "");

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

                    Delete();
                });
            });            
        }

        public override bool AllowParagon { get { return false; } }
        public override int PoisonResistance { get { return 5; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public PitTentacle(Serial serial) : base(serial) { }        

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