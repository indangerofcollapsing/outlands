using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
	public class FactionHenchman : BaseFactionGuard
	{
		[Constructable]
		public FactionHenchman() : base( "the henchman" )
		{
            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(200);

            SetDamage(15, 25);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Tactics, 100);
            
            SetSkill(SkillName.MagicResist, 90);

            VirtualArmor = 50;

			GenerateBody( false, true );

			AddItem( new StuddedChest() );
			AddItem( new StuddedLegs() );
			AddItem( new StuddedArms() );
			AddItem( new StuddedGloves() );
			AddItem( new StuddedGorget() );
			AddItem( new Boots() );

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new Scimitar() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 2: { AddItem(new WarFork() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 3: { AddItem(new Club() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
                case 4: { AddItem(new Kryss() { Movable = false, Hue = 0 }); AddItem(new WoodenShield() { Movable = false, Hue = 0 }); break; }
            }

            PackItem(new Crossbow() { Movable = false, Hue = 0 });

			PackItem( new Bandage( Utility.RandomMinMax( 10, 20 ) ) );
			PackWeakPotions( 1, 4 );
		}

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

		public FactionHenchman( Serial serial ) : base( serial )
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