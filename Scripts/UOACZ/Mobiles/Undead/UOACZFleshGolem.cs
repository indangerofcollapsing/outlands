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
    [CorpseName("a flesh golem corpse")]
    public class UOACZFleshGolem : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Medium; } }

        public override int DifficultyValue { get { return 8; } }

		[Constructable]
		public UOACZFleshGolem() : base()
		{
            Name = "a flesh golem";
            Body = 304;
            BaseSoundID = 684;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(500);

            SetDamage(14, 28);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = -1800;                   
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 5;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            
            if (Utility.RandomDouble() <= .1)
                MassiveCorpseExplosion(this, Location, Location, false, true, Map, 0, 5, 4, 6);            

            SpecialAbilities.EnrageSpecialAbility(.2, attacker, this, .2, 20, -1, true, "Your attack enrages the target.", "", "*becomes enraged*");
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            

            base.OnDamage(amount, from, willKill);
        }

        public void MassiveCorpseExplosion(BaseCreature creature, Point3D startLocation, Point3D endLocation, bool needLOS, bool allowSameTile, Map map, int minRadius, int maxRadius, int minParts, int maxParts)
        {
            PlaySound(GetAngerSound());
            SpecialAbilities.CorpseExplosionAbility(creature, startLocation, endLocation, needLOS, allowSameTile, map, minRadius, maxRadius, minParts, maxParts);
        }  

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() < .33)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                TimedStatic fleshyBits = new TimedStatic(Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600), 5);
                fleshyBits.Name = "fleshy bits";

                fleshyBits.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            MassiveCorpseExplosion(this, Location, Location, false, true, Map, 0, 8, 10, 10); 

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x36B; }
        public override int GetIdleSound() { return 0x2FA; }
        public override int GetAttackSound() { return 0x2F8; }
        public override int GetHurtSound() { return 0x2F9; }
        public override int GetDeathSound() { return 0x2F7; }

        public UOACZFleshGolem(Serial serial): base(serial)
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
