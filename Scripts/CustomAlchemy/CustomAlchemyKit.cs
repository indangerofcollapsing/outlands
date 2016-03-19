using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class CustomAlchemyKit : Item
    {
        #region Properties

        //Stored Crafting Components
        private int m_BluecapMushroom = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BluecapMushroom
        {
            get { return m_BluecapMushroom; }
            set { m_BluecapMushroom = value; }
        }

        private int m_CockatriceEgg = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CockatriceEgg
        {
            get { return m_CockatriceEgg; }
            set { m_CockatriceEgg = value; }
        }

        private int m_Creepervine = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Creepervine
        {
            get { return m_Creepervine; }
            set { m_Creepervine = value; }
        }        

        private int m_FireEssence = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int FireEssence
        {
            get { return m_FireEssence; }
            set { m_FireEssence = value; }
        }

        private int m_Ghostweed = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Ghostweed
        {
            get { return m_Ghostweed; }
            set { m_Ghostweed = value; }
        }

        private int m_GhoulHide = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GhoulHide
        {
            get { return m_GhoulHide; }
            set { m_GhoulHide = value; }
        }

        private int m_LuniteHeart = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LuniteHeart
        {
            get { return m_LuniteHeart; }
            set { m_LuniteHeart = value; }
        }

        private int m_ObsidianShard = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ObsidianShard
        {
            get { return m_ObsidianShard; }
            set { m_ObsidianShard = value; }
        }

        private int m_Quartzstone = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Quartzstone
        {
            get { return m_Quartzstone; }
            set { m_Quartzstone = value; }
        }

        private int m_ShatteredCrystal = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ShatteredCrystal
        {
            get { return m_ShatteredCrystal; }
            set { m_ShatteredCrystal = value; }
        }

        private int m_Snakeskin = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Snakeskin
        {
            get { return m_Snakeskin; }
            set { m_Snakeskin = value; }
        }

        private int m_TrollFat = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TrollFat
        {
            get { return m_TrollFat; }
            set { m_TrollFat = value; }
        }

        //Loaded Crafting Components
        private CraftingComponent.CraftingComponentType m_FirstIngredientType = CraftingComponent.CraftingComponentType.BluecapMushroom;
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftingComponent.CraftingComponentType FirstIngredientType
        {
            get { return m_FirstIngredientType; }
            set { m_FirstIngredientType = value; }
        }

        private int m_FirstIngredientAmount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int FirstIngredientAmount
        {
            get { return m_FirstIngredientAmount; }
            set { m_FirstIngredientAmount = value; }
        }

        private CraftingComponent.CraftingComponentType m_SecondIngredientType = CraftingComponent.CraftingComponentType.BluecapMushroom;
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftingComponent.CraftingComponentType SecondIngredientType
        {
            get { return m_SecondIngredientType; }
            set { m_SecondIngredientType = value; }
        }

        private int m_SecondIngredientAmount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SecondIngredientAmount
        {
            get { return m_SecondIngredientAmount; }
            set { m_SecondIngredientAmount = value; }
        }

        private CraftingComponent.CraftingComponentType m_ThirdIngredientType = CraftingComponent.CraftingComponentType.BluecapMushroom;
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftingComponent.CraftingComponentType ThirdIngredientType
        {
            get { return m_ThirdIngredientType; }
            set { m_ThirdIngredientType = value; }
        }

        private int m_ThirdIngredientAmount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ThirdIngredientAmount
        {
            get { return m_ThirdIngredientAmount; }
            set { m_ThirdIngredientAmount = value; }
        }        

        #endregion                  

        [Constructable]
        public CustomAlchemyKit(): base(6237)
        {
            Name = "alchemy kit";

            Hue = 2635;
            Weight = 5;
        }

        public CustomAlchemyKit(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);

            int totalDistillations = 0;

            totalDistillations += m_BluecapMushroom;
            totalDistillations += m_CockatriceEgg;
            totalDistillations += m_Creepervine;
            totalDistillations += m_FireEssence;
            totalDistillations += m_Ghostweed;
            totalDistillations += m_GhoulHide;
            totalDistillations += m_LuniteHeart;
            totalDistillations += m_ObsidianShard;
            totalDistillations += m_Quartzstone;
            totalDistillations += m_ShatteredCrystal;
            totalDistillations += m_Snakeskin;
            totalDistillations += m_TrollFat;

            LabelTo(from, "(" + totalDistillations.ToString() + " distillation held)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            m_FirstIngredientAmount = 0;
            m_SecondIngredientAmount = 0;
            m_ThirdIngredientAmount = 0;

            player.SendSound(0x242);

            player.CloseGump(typeof(CustomAlchemyKitGump));
            player.SendGump(new CustomAlchemyKitGump(player, this));
        }
        
        public double DeterminePotionSuccessChance(Mobile from)
        {
            double alchemySkill = from.Skills.Alchemy.Value;
            
            double bonusSkill = alchemySkill - CustomAlchemy.minimumSkill;

            int totalIngredients = FirstIngredientAmount + SecondIngredientAmount + ThirdIngredientAmount;
            int extraIngredients = totalIngredients - CustomAlchemy.minIngredients;
            
            double chance = CustomAlchemy.baseChance;

            chance += (bonusSkill * CustomAlchemy.bonusChancePerSkill);

            chance *= (1 + (CustomAlchemy.extraIngredientModifier * (double)extraIngredients));

            if (totalIngredients < CustomAlchemy.minIngredients)
                chance = 0;

            if (FirstIngredientAmount == 0 || SecondIngredientAmount == 0 || ThirdIngredientAmount == 0)
                chance = 0;

            if (alchemySkill < CustomAlchemy.minimumSkill)
                chance = 0;

            if (chance < 0)
                chance = 0;

            if (chance > 1)
                chance = 1;

            return chance;
        }

        public void CreatePotion(Mobile from)
        {
            double successChance = DeterminePotionSuccessChance(from);

            bool validFirstIngredient = true;
            bool validSecondIngredient = true;
            bool validThirdIngredient = true;

            #region FirstIngredient            

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.BluecapMushroom && (FirstIngredientAmount > m_BluecapMushroom || m_BluecapMushroom == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.CockatriceEgg && (FirstIngredientAmount > m_CockatriceEgg || m_CockatriceEgg == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.Creepervine && (FirstIngredientAmount > m_Creepervine || m_Creepervine == 0))
                validFirstIngredient = false;            

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.FireEssence && (FirstIngredientAmount > m_FireEssence || m_FireEssence == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.Ghostweed && (FirstIngredientAmount > m_Ghostweed || m_Ghostweed == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.GhoulHide && (FirstIngredientAmount > m_GhoulHide || m_GhoulHide == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.LuniteHeart && (FirstIngredientAmount > m_LuniteHeart || m_LuniteHeart == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.ObsidianShard && (FirstIngredientAmount > m_ObsidianShard || m_ObsidianShard == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.Quartzstone && (FirstIngredientAmount > m_Quartzstone || m_Quartzstone == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.ShatteredCrystal && (FirstIngredientAmount > m_ShatteredCrystal || m_ShatteredCrystal == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.Snakeskin && (FirstIngredientAmount > m_Snakeskin || m_Snakeskin == 0))
                validFirstIngredient = false;

            if (m_FirstIngredientType == CraftingComponent.CraftingComponentType.TrollFat && (FirstIngredientAmount > m_TrollFat || m_TrollFat == 0))
                validFirstIngredient = false;

            #endregion

            #region SecondIngredient

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.BluecapMushroom && (SecondIngredientAmount > m_BluecapMushroom || m_BluecapMushroom == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.CockatriceEgg && (SecondIngredientAmount > m_CockatriceEgg || m_CockatriceEgg == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.Creepervine && (SecondIngredientAmount > m_Creepervine || m_Creepervine == 0))
                validSecondIngredient = false;            

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.FireEssence && (SecondIngredientAmount > m_FireEssence || m_FireEssence == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.Ghostweed && (SecondIngredientAmount > m_Ghostweed || m_Ghostweed == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.GhoulHide && (SecondIngredientAmount > m_GhoulHide || m_GhoulHide == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.LuniteHeart && (SecondIngredientAmount > m_LuniteHeart || m_LuniteHeart == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.ObsidianShard && (SecondIngredientAmount > m_ObsidianShard || m_ObsidianShard == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.Quartzstone && (SecondIngredientAmount > m_Quartzstone || m_Quartzstone == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.ShatteredCrystal && (SecondIngredientAmount > m_ShatteredCrystal || m_ShatteredCrystal == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.Snakeskin && (SecondIngredientAmount > m_Snakeskin || m_Snakeskin == 0))
                validSecondIngredient = false;

            if (m_SecondIngredientType == CraftingComponent.CraftingComponentType.TrollFat && (SecondIngredientAmount > m_TrollFat || m_TrollFat == 0))
                validSecondIngredient = false;

            #endregion

            #region ThirdIngredient

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.BluecapMushroom && (ThirdIngredientAmount > m_BluecapMushroom || m_BluecapMushroom == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.CockatriceEgg && (ThirdIngredientAmount > m_CockatriceEgg || m_CockatriceEgg == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.Creepervine && (ThirdIngredientAmount > m_Creepervine || m_Creepervine == 0))
                validThirdIngredient = false;            

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.FireEssence && (ThirdIngredientAmount > m_FireEssence || m_FireEssence == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.Ghostweed && (ThirdIngredientAmount > m_Ghostweed || m_Ghostweed == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.GhoulHide && (ThirdIngredientAmount > m_GhoulHide || m_GhoulHide == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.LuniteHeart && (ThirdIngredientAmount > m_LuniteHeart || m_LuniteHeart == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.ObsidianShard && (ThirdIngredientAmount > m_ObsidianShard || m_ObsidianShard == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.Quartzstone && (ThirdIngredientAmount > m_Quartzstone || m_Quartzstone == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.ShatteredCrystal && (ThirdIngredientAmount > m_ShatteredCrystal || m_ShatteredCrystal == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.Snakeskin && (ThirdIngredientAmount > m_Snakeskin || m_Snakeskin == 0))
                validThirdIngredient = false;

            if (m_ThirdIngredientType == CraftingComponent.CraftingComponentType.TrollFat && (ThirdIngredientAmount > m_TrollFat || m_TrollFat == 0))
                validThirdIngredient = false;

            #endregion

            if (FirstIngredientAmount == 0)
            {
                from.SendMessage("You have not chosen a first ingredient yet.");
                return;
            }

            if (SecondIngredientAmount == 0)
            {
                from.SendMessage("You have not chosen a second ingredient yet.");
                return;
            }

            if (ThirdIngredientAmount == 0)
            {
                from.SendMessage("You have not chosen a third ingredient yet.");
                return;
            }
            
            if (!validFirstIngredient)
            {
                from.SendMessage("You do not have enough distillation available for your first ingredient.");
                return;
            }

            if (!validSecondIngredient)
            {
                from.SendMessage("You do not have enough distillation available for your second ingredient.");
                return;
            }

            if (!validThirdIngredient)
            {
                from.SendMessage("You do not have enough distillation available for your third ingredient.");
                return;
            }

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is too full to create a new item. Please remove some items to make room.");
                return;
            }

            ConsumeIngredient(m_FirstIngredientType, m_FirstIngredientAmount);
            ConsumeIngredient(m_SecondIngredientType, m_SecondIngredientAmount);
            ConsumeIngredient(m_ThirdIngredientType, m_ThirdIngredientAmount);

            //Potion Success
            if (Utility.RandomDouble() <= successChance)
            {
                CustomAlchemyPotion potion = new CustomAlchemyPotion();

                //Determine Effect Type
                double positiveChance = CustomAlchemy.positivePotionChance;

                if (Utility.RandomDouble() <= positiveChance)
                    potion.PositiveEffect = true;
                else
                    potion.PositiveEffect = false;

                //Add Ingredient to Potion
                List<CraftingComponent.CraftingComponentType> m_Ingredients = new List<CraftingComponent.CraftingComponentType>();

                for (int a = 0; a < m_FirstIngredientAmount; a++)
                {
                    m_Ingredients.Add(m_FirstIngredientType);
                }

                for (int a = 0; a < m_SecondIngredientAmount; a++)
                {
                    m_Ingredients.Add(m_SecondIngredientType);
                }

                for (int a = 0; a < m_ThirdIngredientAmount; a++)
                {
                    m_Ingredients.Add(m_ThirdIngredientType);
                }

                if (m_Ingredients.Count == 0)
                    return;

                //Primary Effect
                CraftingComponent.CraftingComponentType craftingComponent = m_Ingredients[Utility.RandomMinMax(0, m_Ingredients.Count - 1)];
                CustomAlchemyComponentDetail firstIngredientDetail = CustomAlchemy.GetComponentDetail(craftingComponent);

                Queue m_Queue = new Queue();

                foreach (CraftingComponent.CraftingComponentType craftingComponentType in m_Ingredients)
                {
                    if (craftingComponentType == craftingComponent)
                        m_Queue.Enqueue(craftingComponentType);
                }

                while (m_Queue.Count > 0)
                {
                    CraftingComponent.CraftingComponentType craftingComponentType = (CraftingComponent.CraftingComponentType)m_Queue.Dequeue();
                    m_Ingredients.Remove(craftingComponentType);
                }

                if (m_Ingredients.Count == 0)
                    return;
                
                //Secondary Effect
                craftingComponent = m_Ingredients[Utility.RandomMinMax(0, m_Ingredients.Count - 1)];
                CustomAlchemyComponentDetail secondIngredientDetail = CustomAlchemy.GetComponentDetail(craftingComponent);

                m_Queue = new Queue();

                foreach (CraftingComponent.CraftingComponentType craftingComponentType in m_Ingredients)
                {
                    if (craftingComponentType == craftingComponent)
                        m_Queue.Enqueue(craftingComponentType);
                }

                while (m_Queue.Count > 0)
                {
                    CraftingComponent.CraftingComponentType craftingComponentType = (CraftingComponent.CraftingComponentType)m_Queue.Dequeue();
                    m_Ingredients.Remove(craftingComponentType);
                }

                if (m_Ingredients.Count == 0)
                    return;   
             
                //Potion Potency
                craftingComponent = m_Ingredients[0];

                int weakestIngredientCount = 0;

                if (m_FirstIngredientType == craftingComponent)
                    weakestIngredientCount = m_FirstIngredientAmount;

                if (m_SecondIngredientType == craftingComponent)
                    weakestIngredientCount = m_SecondIngredientAmount;

                if (m_ThirdIngredientType == craftingComponent)
                    weakestIngredientCount = m_ThirdIngredientAmount;

                potion.EffectPotency = CustomAlchemy.EffectPotencyType.Target;

                int potionLevel = 0;

                double adjustedUpgradeChance = CustomAlchemy.upgradeChancePerThirdIngredient * (double)weakestIngredientCount;

                int upgradeLevels = Enum.GetNames(typeof(CustomAlchemy.EffectPotencyType)).Length - 1;

                for (int a = 0; a < upgradeLevels; a++)
                {
                    if (Utility.RandomDouble() <= adjustedUpgradeChance)
                        potionLevel++;

                    adjustedUpgradeChance *= CustomAlchemy.upgradeTierScalarPerUpgrade;
                }
                
                potion.EffectPotency = (CustomAlchemy.EffectPotencyType)potionLevel;

                //Determine Effect Area
                double effectAreaResult = Utility.RandomDouble();

                if (effectAreaResult <= .60)
                    potion.EffectPotency = CustomAlchemy.EffectPotencyType.Target;

                else if (effectAreaResult <= .80)
                    potion.EffectPotency = CustomAlchemy.EffectPotencyType.MediumAoE;
                else
                    potion.EffectPotency = CustomAlchemy.EffectPotencyType.LargeAoE;
                
                if (potion.PositiveEffect)
                {
                    potion.PrimaryEffect = firstIngredientDetail.m_PositiveEffectType;
                    potion.SecondaryEffect = secondIngredientDetail.m_PositiveEffectType;

                    potion.Hue = firstIngredientDetail.m_PositivePotionHue;
                }

                else
                {
                    potion.PrimaryEffect = firstIngredientDetail.m_NegativeEffectType;
                    potion.SecondaryEffect = secondIngredientDetail.m_NegativeEffectType;

                    potion.Hue = firstIngredientDetail.m_NegativePotionHue;
                }

                from.Backpack.DropItem(potion);

                from.SendSound(CustomAlchemy.PotionSuccessSound);
                from.SendMessage("You create a potion.");
            }

            else
            {
                from.SendSound(CustomAlchemy.PotionFailureSound);
                from.SendMessage("You fail to create your potion.");
            }
        }

        public void DistillAllCraftingComponentsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            List<CraftingComponent> m_CraftingComponents = from.Backpack.FindItemsByType<CraftingComponent>();

            int totalComponentCount = 0;
            int totalDistillationCount = 0;

            Queue m_Queue = new Queue();

            foreach (CraftingComponent craftingComponent in m_CraftingComponents)
            {
                m_Queue.Enqueue(craftingComponent);
            }

            bool advancedCooking = false;
            bool masterCooking = false;

            int distillationPerCraftingComponent = CustomAlchemy.DistillationBaseChargesPerCraftingComponent;

            if (from.Skills.Cooking.Value >= CustomAlchemy.CookingSkillFirstExtraDistillationChargeThreshold)
            {
                distillationPerCraftingComponent++;
                advancedCooking = true;
            }

            if (from.Skills.Cooking.Value >= CustomAlchemy.CookingSkillSecondExtraDistillationChargeThreshold)
            {
                distillationPerCraftingComponent++;
                masterCooking = true;
            }

            while (m_Queue.Count > 0)
            {
                CraftingComponent craftingComponent = (CraftingComponent)m_Queue.Dequeue();

                #region Crafting Components 

                if (craftingComponent is BluecapMushroom)
                {
                    m_BluecapMushroom += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is Creepervine)
                {
                    m_Creepervine += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is CockatriceEgg)
                {
                    m_CockatriceEgg += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is FireEssence)
                {
                    m_FireEssence += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is Ghostweed)
                {
                    m_Ghostweed += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is GhoulHide)
                {
                    m_GhoulHide += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is LuniteHeart)
                {
                    m_LuniteHeart += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is ObsidianShard)
                {
                    m_ObsidianShard += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is Quartzstone)
                {
                    m_Quartzstone += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is ShatteredCrystal)
                {
                    m_ShatteredCrystal += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is Snakeskin)
                {
                    m_Snakeskin += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                if (craftingComponent is TrollFat)
                {
                    m_TrollFat += craftingComponent.Amount * distillationPerCraftingComponent;
                    totalDistillationCount += craftingComponent.Amount * distillationPerCraftingComponent;
                }

                #endregion

                totalComponentCount += craftingComponent.Amount;
                craftingComponent.Delete();
            }


            if (totalComponentCount >= 1)
            {
                string distillMessage = "With your limited knowledge of cooking ";

                if (advancedCooking)
                    distillMessage = "With your advanced knowledge of cooking ";

                if (masterCooking)
                    distillMessage = "With your mastery of cooking ";
                
                distillMessage += "you distill " + totalComponentCount.ToString() + " crafting components and add " + totalDistillationCount.ToString() + " distillation charges to your alchemy kit.";

                from.SendMessage(distillMessage);
                from.SendSound(CustomAlchemy.DistillIngredientsSound);
            }

            else
                from.SendMessage("You do not have any crafting components in your backpack.");
        }

        public void ConsumeIngredient(CraftingComponent.CraftingComponentType craftingComponent, int amount)
        {
            switch (craftingComponent)
            {
                case CraftingComponent.CraftingComponentType.BluecapMushroom: m_BluecapMushroom -= amount; break;
                case CraftingComponent.CraftingComponentType.CockatriceEgg: m_CockatriceEgg -= amount; break;
                case CraftingComponent.CraftingComponentType.Creepervine: m_Creepervine -= amount; break;
                case CraftingComponent.CraftingComponentType.FireEssence: m_FireEssence -= amount; break;
                case CraftingComponent.CraftingComponentType.Ghostweed: m_Ghostweed -= amount; break;
                case CraftingComponent.CraftingComponentType.GhoulHide: m_GhoulHide -= amount; break;
                case CraftingComponent.CraftingComponentType.LuniteHeart: m_LuniteHeart -= amount; break;
                case CraftingComponent.CraftingComponentType.ObsidianShard: m_ObsidianShard -= amount; break;
                case CraftingComponent.CraftingComponentType.Quartzstone: m_Quartzstone -= amount; break;
                case CraftingComponent.CraftingComponentType.ShatteredCrystal: m_ShatteredCrystal -= amount; break;
                case CraftingComponent.CraftingComponentType.Snakeskin: m_Snakeskin -= amount; break;
                case CraftingComponent.CraftingComponentType.TrollFat: m_TrollFat -= amount; break;
            }
        }

        public void AddRemoveIngredient(Mobile from, CraftingComponent.CraftingComponentType craftingComponent, int amount)
        {
            bool addedIngredient = false;

            //Add Ingredient
            if (amount > 0)
            {
                if (FirstIngredientAmount > 0 && FirstIngredientType == craftingComponent)
                {
                    FirstIngredientAmount++;
                    addedIngredient = true;
                }

                else if (SecondIngredientAmount > 0 && SecondIngredientType == craftingComponent)
                {
                    SecondIngredientAmount++;
                    addedIngredient = true;
                }

                else if (ThirdIngredientAmount > 0 && ThirdIngredientType == craftingComponent)
                {
                    ThirdIngredientAmount++;
                    addedIngredient = true;
                }

                else if (FirstIngredientAmount == 0)
                {
                    FirstIngredientType = craftingComponent;
                    FirstIngredientAmount = 1;

                    addedIngredient = true;
                }

                else if (SecondIngredientAmount == 0)
                {
                    SecondIngredientType = craftingComponent;
                    SecondIngredientAmount = 1;

                    addedIngredient = true;
                }

                else if (ThirdIngredientAmount == 0)
                {
                    ThirdIngredientType = craftingComponent;
                    ThirdIngredientAmount = 1;

                    addedIngredient = true;
                }

                int sound = CustomAlchemy.AddIngredientSound;

                switch (craftingComponent)
                {
                    case CraftingComponent.CraftingComponentType.BluecapMushroom: sound = 0x5D9; break;
                    case CraftingComponent.CraftingComponentType.CockatriceEgg: sound = 0x134; break;
                    case CraftingComponent.CraftingComponentType.Creepervine: sound = 0x33A; break;
                    case CraftingComponent.CraftingComponentType.FireEssence: sound = 0x58E; break;

                    case CraftingComponent.CraftingComponentType.Ghostweed: sound = 0x368; break;
                    case CraftingComponent.CraftingComponentType.GhoulHide: sound = 0x3E3; break;
                    case CraftingComponent.CraftingComponentType.LuniteHeart: sound = 0x2E3; break;
                    case CraftingComponent.CraftingComponentType.ObsidianShard: sound = 0x056; break;

                    case CraftingComponent.CraftingComponentType.Quartzstone: sound = 0x24B; break;
                    case CraftingComponent.CraftingComponentType.ShatteredCrystal: sound = 0x03F; break;
                    case CraftingComponent.CraftingComponentType.Snakeskin: sound = 0x5A2; break;
                    case CraftingComponent.CraftingComponentType.TrollFat: sound = 0x2DD; break;
                }

                if (addedIngredient)
                    from.SendSound(sound);

                return;
            }

            //Remove Ingredient
            else if (amount < 0)
            {
                if (FirstIngredientAmount > 0 && FirstIngredientType == craftingComponent)
                    FirstIngredientAmount--;

                else if (SecondIngredientAmount > 0 && SecondIngredientType == craftingComponent)
                    SecondIngredientAmount--;

                else if (ThirdIngredientAmount > 0 && ThirdIngredientType == craftingComponent)
                    ThirdIngredientAmount--;

                from.SendSound(CustomAlchemy.RemoveIngredientSound);
            }
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   

            writer.Write(m_BluecapMushroom);
            writer.Write(m_CockatriceEgg);
            writer.Write(m_Creepervine);            
            writer.Write(m_FireEssence);
            writer.Write(m_Ghostweed);
            writer.Write(m_GhoulHide);
            writer.Write(m_LuniteHeart);
            writer.Write(m_ObsidianShard);
            writer.Write(m_Quartzstone);
            writer.Write(m_ShatteredCrystal);
            writer.Write(m_Snakeskin);
            writer.Write(m_TrollFat);

            writer.Write((int)m_FirstIngredientType);
            writer.Write(m_FirstIngredientAmount);

            writer.Write((int)m_SecondIngredientType);
            writer.Write(m_SecondIngredientAmount);

            writer.Write((int)m_ThirdIngredientType);
            writer.Write(m_ThirdIngredientAmount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_BluecapMushroom = reader.ReadInt();
                m_CockatriceEgg = reader.ReadInt();
                m_Creepervine = reader.ReadInt();               
                m_FireEssence = reader.ReadInt();
                m_Ghostweed = reader.ReadInt();
                m_GhoulHide = reader.ReadInt();
                m_LuniteHeart = reader.ReadInt();
                m_ObsidianShard = reader.ReadInt();
                m_Quartzstone = reader.ReadInt();
                m_ShatteredCrystal = reader.ReadInt();
                m_Snakeskin = reader.ReadInt();
                m_TrollFat = reader.ReadInt();

                m_FirstIngredientType = (CraftingComponent.CraftingComponentType)reader.ReadInt();
                m_FirstIngredientAmount = reader.ReadInt();

                m_SecondIngredientType = (CraftingComponent.CraftingComponentType)reader.ReadInt();
                m_SecondIngredientAmount = reader.ReadInt();

                m_ThirdIngredientType = (CraftingComponent.CraftingComponentType)reader.ReadInt();
                m_ThirdIngredientAmount = reader.ReadInt();                
            }
        }
    }

    public class CustomAlchemyKitGump : Gump
    {
        private PlayerMobile m_Player;
        private CustomAlchemyKit m_CustomAlchemyKit;       

        public CustomAlchemyKitGump(PlayerMobile player, CustomAlchemyKit CustomAlchemyKit): base(110, 100)
        {
            m_Player = player;
            m_CustomAlchemyKit = CustomAlchemyKit;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2499;

            int RedTextHue = 0x22;           
            int YellowTextHue = 149;
            int BlueTextHue = 2603;
            int GreenTextHue = 0x3F;

            AddPage(0);
            AddBackground(8, 3, 490, 525, 9270);

            AddLabel(186, 23, 2534, "Distillation Available");

            AddLabel(190, 262, WhiteTextHue, "+");          
            AddImage(127, 226, 30081, 2586);
            AddImage(112, 216, 30079, 2586);           
            AddImage(223, 226, 30081, 2586);
            AddImage(208, 216, 30079, 2586);
            AddLabel(287, 262, WhiteTextHue, "+");             
            AddImage(318, 226, 30081, 2586);
            AddImage(304, 216, 30079, 2586);

            int row = 0;
            int column = 0;

            int startX = 45;
            int startY = 60;

            int rowSpacingX = 115;
            int rowSpacingY = 45;
            
            int adjustedAmount = 0;
            int amountHue = WhiteTextHue;
            CraftingComponent.CraftingComponentType craftingComponentType;
            CraftingComponentDetail detail;

            int firstIngredientHue = WhiteTextHue;
            int secondIngredientHue = WhiteTextHue;
            int thirdIngredientHue = WhiteTextHue;

            #region Crafting Component Entries

            //Bluecap Mushroom  
            craftingComponentType = CraftingComponent.CraftingComponentType.BluecapMushroom;
            adjustedAmount = m_CustomAlchemyKit.BluecapMushroom;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(28, 60, 9906, 9907, 10, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(83, 60, amountHue, adjustedAmount.ToString());

            //Cockatrice Egg  
            column++;
 
            craftingComponentType = CraftingComponent.CraftingComponentType.CockatriceEgg;
            adjustedAmount = m_CustomAlchemyKit.CockatriceEgg;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(144, 60, 9906, 9907, 11, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(203, 60, amountHue, adjustedAmount.ToString());

            //Creepervine
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.Creepervine;
            adjustedAmount = m_CustomAlchemyKit.Creepervine;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(262, 60, 9906, 9907, 12, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(315, 60, amountHue, adjustedAmount.ToString());
            
            //Fire Essence
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.FireEssence;
            adjustedAmount = m_CustomAlchemyKit.FireEssence;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(375, 60, 9906, 9907, 13, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(428, 60, amountHue, adjustedAmount.ToString());
          
            //Ghostweed
            row = 1;
            column = 0;

            craftingComponentType = CraftingComponent.CraftingComponentType.Ghostweed;
            adjustedAmount = m_CustomAlchemyKit.Ghostweed;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(28, 105, 9906, 9907, 14, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(83, 105, amountHue, adjustedAmount.ToString());

            //Ghoul Hide
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.GhoulHide;
            adjustedAmount = m_CustomAlchemyKit.GhoulHide;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(144, 105, 9906, 9907, 15, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(203, 105, amountHue, adjustedAmount.ToString());

            //Lunite Heart
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.LuniteHeart;
            adjustedAmount = m_CustomAlchemyKit.LuniteHeart;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(262, 105, 9906, 9907, 16, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(315, 105, amountHue, adjustedAmount.ToString());

            //Obsidian Shard
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.ObsidianShard;
            adjustedAmount = m_CustomAlchemyKit.ObsidianShard;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(375, 105, 9906, 9907, 17, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(428, 105, amountHue, adjustedAmount.ToString());
                                   
            //Quartzstone
            row = 2;
            column = 0;

            craftingComponentType = CraftingComponent.CraftingComponentType.Quartzstone;
            adjustedAmount = m_CustomAlchemyKit.Quartzstone;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(28, 150, 9906, 9907, 18, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(83, 150, amountHue, adjustedAmount.ToString());

            //Shattered Crystal
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.ShatteredCrystal;
            adjustedAmount = m_CustomAlchemyKit.ShatteredCrystal;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(144, 150, 9906, 9907, 19, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(203, 150, amountHue, adjustedAmount.ToString());

            //Snakeskin
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.Snakeskin;
            adjustedAmount = m_CustomAlchemyKit.Snakeskin;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(262, 150, 9906, 9907, 20, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(315, 150, amountHue, adjustedAmount.ToString());
            
            //Troll Fat
            column++;

            craftingComponentType = CraftingComponent.CraftingComponentType.TrollFat;
            adjustedAmount = m_CustomAlchemyKit.TrollFat;
            detail = CraftingComponent.GetCraftingComponentDetail(craftingComponentType);

            amountHue = WhiteTextHue;

            if (m_CustomAlchemyKit.FirstIngredientType == craftingComponentType && m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.FirstIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    firstIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.SecondIngredientType == craftingComponentType && m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.SecondIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    secondIngredientHue = RedTextHue;
                }
            }

            if (m_CustomAlchemyKit.ThirdIngredientType == craftingComponentType && m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                adjustedAmount -= m_CustomAlchemyKit.ThirdIngredientAmount;

                if (adjustedAmount >= 0)
                    amountHue = GreenTextHue;
                else
                {
                    amountHue = RedTextHue;
                    thirdIngredientHue = RedTextHue;
                }
            }

            AddButton(375, 150, 9906, 9907, 21, GumpButtonType.Reply, 0);
            AddItem(startX + (column * rowSpacingX) + detail.m_OffsetX, startY + (row * rowSpacingY) + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
            AddLabel(428, 150, amountHue, adjustedAmount.ToString());
            
            #endregion

            AddLabel(181, 191, 2624, "Current Ingredients");

            AddButton(14, 7, 2095, 2094, 3, GumpButtonType.Reply, 0);
            AddLabel(41, 11, 149, "Wiki Page");

            string amount = "";

            //Ingredient 1
            if (m_CustomAlchemyKit.FirstIngredientAmount > 0)
            {
                detail = CraftingComponent.GetCraftingComponentDetail(m_CustomAlchemyKit.FirstIngredientType);

                amount = m_CustomAlchemyKit.FirstIngredientAmount.ToString();

                AddItem(128 + detail.m_OffsetX, 250 + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
                AddLabel(Utility.CenteredTextOffset(150, amount), 280, WhiteTextHue, amount);
                AddButton(136, 322, 9900, 9900, 4, GumpButtonType.Reply, 0);
            }

            //Ingredient 2
            if (m_CustomAlchemyKit.SecondIngredientAmount > 0)
            {
                detail = CraftingComponent.GetCraftingComponentDetail(m_CustomAlchemyKit.SecondIngredientType);

                amount = m_CustomAlchemyKit.SecondIngredientAmount.ToString();

                AddItem(223 + detail.m_OffsetX, 250 + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
                AddLabel(Utility.CenteredTextOffset(245, amount), 280, WhiteTextHue, amount);
                AddButton(234, 322, 9900, 9900, 5, GumpButtonType.Reply, 0);
            }

            //Ingredient 3
            if (m_CustomAlchemyKit.ThirdIngredientAmount > 0)
            {
                detail = CraftingComponent.GetCraftingComponentDetail(m_CustomAlchemyKit.ThirdIngredientType);

                amount = m_CustomAlchemyKit.ThirdIngredientAmount.ToString();

                AddItem(318 + detail.m_OffsetX, 250 + detail.m_OffsetY, detail.m_ItemId, detail.m_Hue);
                AddLabel(Utility.CenteredTextOffset(340, amount), 280, WhiteTextHue, amount);
                AddButton(329, 322, 9900, 9900, 6, GumpButtonType.Reply, 0);
            }

            double potionSuccessChance = m_CustomAlchemyKit.DeterminePotionSuccessChance(m_Player);

            //Potion Success Chance
            int chanceTextHue = RedTextHue;

            if (potionSuccessChance >= .25)
                chanceTextHue = YellowTextHue;

            if (potionSuccessChance >= .50)
                chanceTextHue = BlueTextHue;

            if (potionSuccessChance >= .75)
                chanceTextHue = GreenTextHue;

            string chanceText = Utility.CreateDecimalPercentageString(potionSuccessChance, 1);

            if (m_CustomAlchemyKit.FirstIngredientAmount == 0 && m_CustomAlchemyKit.SecondIngredientAmount == 0 && m_CustomAlchemyKit.ThirdIngredientAmount == 0)
                chanceText = "";

            AddLabel(46, 360, WhiteTextHue, "Potion Success Chance:");    
            AddLabel(205, 360, chanceTextHue, chanceText);
            //AddLabel(Utility.CenteredTextOffset(365, chanceText), 360, chanceTextHue, chanceText);
            
            //Possible Potions
            string supportPotionName = "";
            string harmfulPotionName = "";

            CraftingComponent.CraftingComponentType primaryCraftingComponent = m_CustomAlchemyKit.FirstIngredientType;
            CraftingComponent.CraftingComponentType secondaryCraftingComponent = m_CustomAlchemyKit.SecondIngredientType;
            CraftingComponent.CraftingComponentType thirdCraftingComponent = m_CustomAlchemyKit.ThirdIngredientType;

            if (potionSuccessChance > 0)
            {
                //Determine Primary Component
                if (m_CustomAlchemyKit.FirstIngredientAmount >= m_CustomAlchemyKit.SecondIngredientAmount && m_CustomAlchemyKit.FirstIngredientAmount >= m_CustomAlchemyKit.ThirdIngredientAmount)
                    primaryCraftingComponent = m_CustomAlchemyKit.FirstIngredientType;

                if (m_CustomAlchemyKit.SecondIngredientAmount >= m_CustomAlchemyKit.FirstIngredientAmount && m_CustomAlchemyKit.SecondIngredientAmount >= m_CustomAlchemyKit.ThirdIngredientAmount)
                    primaryCraftingComponent = m_CustomAlchemyKit.SecondIngredientType;

                if (m_CustomAlchemyKit.ThirdIngredientAmount >= m_CustomAlchemyKit.FirstIngredientAmount && m_CustomAlchemyKit.ThirdIngredientAmount >= m_CustomAlchemyKit.SecondIngredientAmount)
                    primaryCraftingComponent = m_CustomAlchemyKit.ThirdIngredientType;

                //Determine Secondary Component
                if (m_CustomAlchemyKit.FirstIngredientType != primaryCraftingComponent && (m_CustomAlchemyKit.FirstIngredientAmount >= m_CustomAlchemyKit.SecondIngredientAmount || m_CustomAlchemyKit.FirstIngredientAmount >= m_CustomAlchemyKit.ThirdIngredientAmount))
                    secondaryCraftingComponent = m_CustomAlchemyKit.FirstIngredientType;

                if (m_CustomAlchemyKit.SecondIngredientType != primaryCraftingComponent && (m_CustomAlchemyKit.SecondIngredientAmount >= m_CustomAlchemyKit.FirstIngredientAmount || m_CustomAlchemyKit.SecondIngredientAmount >= m_CustomAlchemyKit.ThirdIngredientAmount))
                    secondaryCraftingComponent = m_CustomAlchemyKit.SecondIngredientType;

                if (m_CustomAlchemyKit.ThirdIngredientType != primaryCraftingComponent && (m_CustomAlchemyKit.ThirdIngredientAmount >= m_CustomAlchemyKit.FirstIngredientAmount || m_CustomAlchemyKit.ThirdIngredientAmount >= m_CustomAlchemyKit.SecondIngredientAmount))
                    secondaryCraftingComponent = m_CustomAlchemyKit.ThirdIngredientType;

                //Third Component
                int thirdCraftingComponentCount = 0;

                if (m_CustomAlchemyKit.FirstIngredientType != primaryCraftingComponent && m_CustomAlchemyKit.FirstIngredientType != secondaryCraftingComponent)
                {
                    thirdCraftingComponent = m_CustomAlchemyKit.FirstIngredientType;
                    thirdCraftingComponentCount = m_CustomAlchemyKit.FirstIngredientAmount;
                }

                if (m_CustomAlchemyKit.SecondIngredientType != primaryCraftingComponent && m_CustomAlchemyKit.SecondIngredientType != secondaryCraftingComponent)
                {
                    thirdCraftingComponent = m_CustomAlchemyKit.SecondIngredientType;
                    thirdCraftingComponentCount = m_CustomAlchemyKit.SecondIngredientAmount;
                }

                if (m_CustomAlchemyKit.ThirdIngredientType != primaryCraftingComponent && m_CustomAlchemyKit.ThirdIngredientType != secondaryCraftingComponent)
                {
                    thirdCraftingComponent = m_CustomAlchemyKit.ThirdIngredientType;
                    thirdCraftingComponentCount = m_CustomAlchemyKit.ThirdIngredientAmount;
                }

                CustomAlchemyComponentDetail firstIngredientDetail = CustomAlchemy.GetComponentDetail(primaryCraftingComponent);
                CustomAlchemyComponentDetail secondIngredientDetail = CustomAlchemy.GetComponentDetail(secondaryCraftingComponent);
                CustomAlchemyComponentDetail thirdIngredientDetail = CustomAlchemy.GetComponentDetail(thirdCraftingComponent);

                CustomAlchemy.EffectPotencyType expectedPotency = CustomAlchemy.EffectPotencyType.Target;

                double adjustedUpgradeChance = CustomAlchemy.upgradeChancePerThirdIngredient * (double)thirdCraftingComponentCount;
                
                int upgradeLevels = Enum.GetNames(typeof(CustomAlchemy.EffectPotencyType)).Length - 1;
                int potencyTier = 0;
                
                for (int a = 0; a < upgradeLevels; a++)
                {
                    if (adjustedUpgradeChance >= .5)                    
                        potencyTier++;

                    adjustedUpgradeChance *= CustomAlchemy.upgradeTierScalarPerUpgrade;
                }

                expectedPotency = (CustomAlchemy.EffectPotencyType)potencyTier;

                supportPotionName = CustomAlchemy.GetPotionName(firstIngredientDetail.m_PositiveEffectType, secondIngredientDetail.m_PositiveEffectType, true, expectedPotency);
                harmfulPotionName = CustomAlchemy.GetPotionName(firstIngredientDetail.m_NegativeEffectType, secondIngredientDetail.m_NegativeEffectType, false, expectedPotency); 
            }

            //Expected Support Potion
            AddLabel(33, 380, WhiteTextHue, "Expected Support Potion:");
            AddLabel(205, 380, 2587, supportPotionName);
            //AddLabel(Utility.CenteredTextOffset(365, supportPotionName), 380, 2587, supportPotionName);

            //Expected Harmful Potion
            AddLabel(33, 400, WhiteTextHue, "Expected Harmful Potion:");
            AddLabel(205, 400, 2115, harmfulPotionName);
            //AddLabel(Utility.CenteredTextOffset(365, harmfulPotionName), 400, 2115, harmfulPotionName);

            AddButton(205, 430, 1147, 1148, 1, GumpButtonType.Reply, 0);

            AddButton(25, 483, 2151, 2154, 2, GumpButtonType.Reply, 0);
            AddLabel(60, 477, YellowTextHue, "Distill All Crafting");
            AddLabel(60, 492, YellowTextHue, "Components in Backpack");
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Player == null || m_CustomAlchemyKit == null) return;
            if (m_CustomAlchemyKit.Deleted) return;

            if (!m_CustomAlchemyKit.IsChildOf(m_Player.Backpack))
            {
                m_Player.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Craft
                    m_CustomAlchemyKit.CreatePotion(m_Player);

                    closeGump = false;
                break;

                case 2:
                    //Distill All
                    m_CustomAlchemyKit.DistillAllCraftingComponentsInPack(m_Player);

                    closeGump = false;
                break;

                case 3:
                    //Wiki
                    closeGump = false;
                break;

                case 4:
                    //Remove 1st Ingredient
                    if (m_CustomAlchemyKit.FirstIngredientAmount > 0)
                    {
                        m_CustomAlchemyKit.FirstIngredientAmount--;
                        m_Player.SendSound(CustomAlchemy.RemoveIngredientSound);
                    }

                    closeGump = false;
                break;

                case 5:
                    //Remove 2nd Ingredient
                    if (m_CustomAlchemyKit.SecondIngredientAmount > 0)
                    {
                        m_CustomAlchemyKit.SecondIngredientAmount--;
                        m_Player.SendSound(CustomAlchemy.RemoveIngredientSound);
                    }

                    closeGump = false;
                break;

                case 6:
                    //Remove 3rd Ingredient
                    if (m_CustomAlchemyKit.ThirdIngredientAmount > 0)
                    {
                        m_CustomAlchemyKit.ThirdIngredientAmount--;
                        m_Player.SendSound(CustomAlchemy.RemoveIngredientSound);
                    }

                    closeGump = false;
                break;

                //Row 1
                case 10:
                    //Bluecap Mushroom
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.BluecapMushroom, 1);                    

                    closeGump = false;
                break;

                case 11:
                    //Cockatrice Egg
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.CockatriceEgg, 1);
                    
                    closeGump = false;                   
                break;

                case 12:
                    //Creepervine
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.Creepervine, 1);
                    
                    closeGump = false;
                break;

                case 13:
                    //Fire Essence
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.FireEssence, 1);
                    
                    closeGump = false;
                break;

                //Row 2
                case 14:
                    //Ghostweed
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.Ghostweed, 1);
                    
                    closeGump = false;
                break;

                case 15:
                    //Ghoul Hide
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.GhoulHide, 1);
                    
                    closeGump = false;
                break;

                case 16:
                    //Lunite Heart
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.LuniteHeart, 1);
                   
                    closeGump = false;
                break;

                case 17:
                    //Obsidian Shard
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.ObsidianShard, 1);
                    
                    closeGump = false;
                break;

                //Row 3
                case 18:
                    //Quartzstone
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.Quartzstone, 1);
                    
                    closeGump = false;
                break;

                case 19:
                    //Shattered Crystal
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.ShatteredCrystal, 1);
                    
                    closeGump = false;
                break;

                case 20:
                    //Snakeskin
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.Snakeskin, 1);
                    
                    closeGump = false;
                break;

                case 21:
                    //Troll Fat
                    m_CustomAlchemyKit.AddRemoveIngredient(m_Player, CraftingComponent.CraftingComponentType.TrollFat, 1);
                    
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(CustomAlchemyKitGump));
                m_Player.SendGump(new CustomAlchemyKitGump(m_Player, m_CustomAlchemyKit));
            }
        }
    }
}