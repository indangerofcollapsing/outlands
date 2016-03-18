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
    [CorpseName("a fountain of evil corpse")]
    public class UOACZFountainOfEvil : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 7; } }

		[Constructable]
		public UOACZFountainOfEvil() : base()
		{
            Name = "a fountain of evil";
            Body = 16;
            Hue = 1761;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(500);
            SetMana(3000);

            SetDamage(11, 22);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 0);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

            Fame = 4500;
            Karma = -4500;

            CanSwim = true;                   
		}

        public override void SetUniqueAI()
        {
            SetSubGroup(AISubgroup.MeleeMage2);
            UpdateAI(false);

            base.SetUniqueAI();

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;
            DictWanderAction[WanderAction.SpellHealOther100] = 0;
            DictWanderAction[WanderAction.SpellCureOther] = 0;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
                        
            if (Utility.RandomDouble() < .2)
            {
                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.MoveToWorld(defender.Location, Map);

                for (int a = 0; a < 4; a++)
                {
                    ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                    ichor.Hue = 2051;
                    ichor.Name = "ichor";
                    ichor.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), Map);
                }

                Effects.PlaySound(defender.Location, defender.Map, 0x580);
                defender.FixedParticles(0x374A, 10, 20, 5021, 1107, 0, EffectLayer.Head);

                defender.SendMessage("You have been covered in an evil ichor!");

                SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 3, -1, false, "", "");
                SpecialAbilities.PierceSpecialAbility(1.0, this, defender, .25, 15, -1, false, "", "");
                SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .2, 15, -1, false, "", "");
                SpecialAbilities.StunSpecialAbility(1.0, this, defender, .10, 15, -1, false, "", "");
            }            
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() <= .33)
            {
                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x101));
            }            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                if (c != null)
                {
                    if (!c.Deleted)
                    {
                        Queue m_Queue = new Queue();

                        foreach (Item item in c.Items)
                        {
                            m_Queue.Enqueue(item);
                        }

                        while (m_Queue.Count > 0)
                        {
                            Item item = (Item)m_Queue.Dequeue();
                            item.MoveToWorld(c.Location, c.Map);
                        }

                        c.Delete();
                    }
                }
            });
        }

        public override int GetAngerSound() { return 0x27F; }
        public override int GetIdleSound() { return 0x280; }
        public override int GetAttackSound() { return 0x281; }
        public override int GetHurtSound() { return 0x282; }
        public override int GetDeathSound() { return 0x283; }

        public UOACZFountainOfEvil(Serial serial): base(serial)
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
