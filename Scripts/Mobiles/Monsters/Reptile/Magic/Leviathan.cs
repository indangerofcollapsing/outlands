using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a leviathan corpse" )]
	public class Leviathan : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Leviathan() : this( null )
		{
		}

		[Constructable]
		public Leviathan( Mobile fisher ) : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;
			
			Name = "a leviathan";
			Body = 77;
			BaseSoundID = 353;

			Hue = 2123;

            SetStr(100);
            SetDex(25);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;            

			Fame = 24000;
			Karma = -24000;

			CanSwim = true;
			CantWalk = true;
		}

        public override int OceanDoubloonValue { get { return 150; } }
        public override bool IsOceanCreature { get { return true; } }
            
        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public Leviathan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}		
	}
}
