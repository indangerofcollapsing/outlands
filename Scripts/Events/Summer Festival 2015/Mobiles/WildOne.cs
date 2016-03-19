using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wild one corpse")]
    public class WildOne : BaseCreature
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(30);

        public int GrassItemId { get { return Utility.RandomList(3220, 3255, 3256, 3223, 3245, 3246, 3248, 3254, 3257, 3258, 3259, 3260, 3261, 3269, 3270, 3267, 3237, 3267, 3239, 3332); } }

        [Constructable]
        public WildOne() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wild one";

            Body = 777;
            Hue = 2542;

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(500);
            SetStam(300);
            SetMana(2000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Stealth, 95);
            SetSkill(SkillName.Hiding, 95);

            VirtualArmor = 25;

            SpellHue = 2210;

            Fame = 6000;
            Karma = 0;            
        }

        public override bool AllowParagon { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    Point3D location = Location;
                    Map map = Map;

                    if (SpecialAbilities.VanishAbility(this, 3.0, true, -1, 3, 6, true, null))
                    {
                        Blood grasses = new Blood();
                        grasses.Name = "tall grass";
                        grasses.ItemID = 3160;
                        grasses.Hue = 2210;
                        grasses.MoveToWorld(location, Map);

                        for (int a = 0; a < 10; a++)
                        {
                            Blood extraGrass = new Blood();
                            extraGrass.Name = "grass";
                            extraGrass.ItemID = GrassItemId;

                            Point3D extraGrassLocation = new Point3D(location.X + Utility.RandomList(-2, -1, 1, 2), location.Y + Utility.RandomList(-2, -1, 1, 2), location.Z);
                            
                            extraGrass.MoveToWorld(extraGrassLocation, Map);
                        }

                        Say("*disappears into the brush*");
                    }

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomMinMax(1, 100) == 1)
                PackItem(new OldBranch());

            if (Utility.RandomMinMax(1, 15) == 1)
                PackItem(new MagicSpringwood());

            PackItem(new Engines.Plants.Seed());
            PackItem(new Engines.Plants.Seed());
            PackItem(new FertileDirt(Utility.RandomMinMax(2, 6)));

            return base.OnBeforeDeath();
        }
        
        public override int GetAttackSound() { return 0x5F0; }
        public override int GetHurtSound() { return 0x5F2; }
        public override int GetAngerSound() { return 0x5F3; }
        public override int GetIdleSound() { return 0x5F1; }
        public override int GetDeathSound() { return 0x508; }  
        
        public WildOne(Serial serial) : base(serial) { }

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