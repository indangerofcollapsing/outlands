using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
	public class FactionMercenary : BaseFactionGuard
	{
		[Constructable]
		public FactionMercenary() : base( "the mercenary" )
		{
            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(260);

            SetDamage(20, 25);

            SetSkill(SkillName.Archery, 95);
            SetSkill(SkillName.Fencing, 95);
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Tactics, 105);

            SetSkill(SkillName.MagicResist, 95);

            VirtualArmor = 50;

      			GenerateBody( false, true );
                  
      			AddItem( new ChainChest() );
      			AddItem( new ChainLegs() );
      			AddItem( new RingmailArms() );
      			AddItem( new RingmailGloves() );
      			AddItem( new ChainCoif() );
      			AddItem( new Boots() );

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new Broadsword() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 2: { AddItem(new Longsword() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 3: { AddItem(new HammerPick() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 4: { AddItem(new Maul() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
            }
            
            PackItem(new Bow() { Movable = false, Hue = 0 });

      			PackItem( new Bandage( Utility.RandomMinMax( 20, 30 ) ) );
      			PackStrongPotions( 3, 8 );
		}

        public override bool CanSwitchWeapons { get { return true; } }

    public override void SetUniqueAI() 
    {
        DictCombatAction[CombatAction.CombatHealSelf] = 2;
        DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
        DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
        DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 1;
        DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;
        DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 1;
        DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] = 1;
        DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] = 1;
        DictCombatHealSelf[CombatHealSelf.BandageCureSelf] = 1;
    }

		public FactionMercenary( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}