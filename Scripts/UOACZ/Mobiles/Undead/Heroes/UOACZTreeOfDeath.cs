using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    [CorpseName("the tree of death's corpse")]
    public class UOACZTreeOfDeath : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Large; } }        
        
        public override int DifficultyValue { get { return 10; } }

        public DateTime m_NextMushroomExplosionAllowed;
        public TimeSpan NextMushroomExplosionDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public int GrassItemId { get { return Utility.RandomList(3220, 3255, 3256, 3223, 3245, 3246, 3248, 3254, 3257, 3258, 3259, 3260, 3261, 3269, 3270, 3267, 3237, 3267, 3239, 3332); } }

		[Constructable]
		public UOACZTreeOfDeath() : base()
		{
            Name = "the tree of death";

            Body = 47;
            Hue = 2076;

            BaseSoundID = 442;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(10000);
            SetMana(5000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;

            Fame = 3500;
            Karma = -3500;   
           
            CanTeleportToBaseNode = false;
            GuardsHome = true;
		}

        public enum ExplosionType
        {
            Branch,
            Mushroom,
            Flower
        }

        public override int MaxDistanceAllowedFromHome { get { return 100; } }

        public override bool AlwaysChamp { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override int AttackRange { get { return 3; } }

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            CombatEpicActionMinDelay = 10;
            CombatEpicActionMaxDelay = 20;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MassiveBoneBreathAttack] = 25;

            MassiveBreathRange = 8;

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.5;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.EntangleSpecialAbility(.33, this, defender, 1.0, 4.0, -1, false, "", "The creature entangles you with its branches!", "-1");
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (willKill)
                UOACZEvents.UndeadChampionDamaged(true);

            else
                UOACZEvents.UndeadChampionDamaged(false);            
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(Undead Champion)", from.NetState);

            base.OnSingleClick(from);
        }

        public override void OnThink()
        {
            base.OnThink();            

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextMushroomExplosionAllowed)
            {
                if (Combatant != null)
                {
                    SpecialAbilities.MushroomExplosionAbility(this, 20, 40, 0, 8, true);

                    m_NextMushroomExplosionAllowed = DateTime.UtcNow + NextMushroomExplosionDelay;

                    return;
                }
            } 
        }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            int items = Utility.RandomMinMax(1, 3);

            for (int a = 0; a < items; a++)
            {
                Point3D moveItemLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref moveItemLocation, Map, 12, false);

                TimedStatic moveItem = new TimedStatic(GrassItemId, 5);
                moveItem.Name = "dead plantlife";
                moveItem.Hue = 2101;
                moveItem.ItemID = GrassItemId;
                moveItem.MoveToWorld(moveItemLocation, Map);
            }

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            int magicItems = 8;

            c.DropItem(new Arrow(100));
            c.DropItem(new Bolt(100));
            c.DropItem(new Bandage(100));

            for (int a = 0; a < magicItems; a++)
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1:
                        BaseWeapon weapon = Loot.RandomWeapon();

                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(1, 4);
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(1, 4);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(1, 4);
                        
                        weapon.Identified = true;

                        c.DropItem(weapon);
                    break;

                    case 2:
                        BaseArmor armor = Loot.RandomArmorOrShield();

                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(1, 4);
                        armor.DurabilityLevel = (ArmorDurabilityLevel)Utility.RandomMinMax(1, 4);

                        armor.Identified = true;

                        c.DropItem(armor);
                    break;
                }
            }
        }

        public UOACZTreeOfDeath(Serial serial): base(serial)
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
