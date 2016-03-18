using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class SanguinProtector : BaseSanguin
	{
		[Constructable]
		public SanguinProtector(): base()
		{			
			Name = "a sanguine protector";
			Hue = 902;

            SetStr(100);
            SetDex(100);
            SetInt(25);

            SetHits(2000);

            SetDamage(19, 38);

            SetSkill(SkillName.Swords, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 35);

            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 50;

			Fame = 10000;
			Karma = -10000;

            AddItem(new BoneHelm() { Movable = false, Hue = itemHue });
            AddItem(new BoneChest() { Movable = false, Hue = weaponHue });
            AddItem(new BoneArms() { Movable = false, Hue = itemHue });
            AddItem(new BoneGloves() { Movable = false, Hue = itemHue });
            AddItem(new BoneLegs() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = weaponHue });

            AddItem(new VikingSword() { Movable = false, Hue = weaponHue });
            AddItem(new ChaosShield() { Movable = false, Hue = weaponHue });
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 0.66;
            
            CombatEpicActionMinDelay = 10;
            CombatEpicActionMaxDelay = 20;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MeleeBleedAoE] = 25;
        }

        public SanguinProtector(Serial serial): base(serial)
        {
        }

        public override bool OnBeforeDeath()
        {
            if (Global_AllowAbilities)
            {
                Balron rm = new Balron();
                rm.Team = this.Team;
                rm.Combatant = this.Combatant;

                Effects.PlaySound(this, Map, GetDeathSound());
                Effects.SendLocationEffect(Location, Map, 0x3709, 30, 10, 1775, 0);

                rm.MoveToWorld(Location, Map);

                Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);

                var goldItem = new Gold(ModifiedGoldWorth());
                goldItem.MoveToWorld(Location, Map);

                Delete();
            }

            return false;
        }

		public override int GetIdleSound() { return 0x184; }
		public override int GetAngerSound() { return 0x286; }
		public override int GetDeathSound() { return 0x288; }
		public override int GetHurtSound() { return 0x19F; }        

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