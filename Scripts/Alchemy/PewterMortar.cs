using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server.Gumps;
using Server.Engines.Craft;
using Server.Targeting;
using Server.Network;

namespace Server.Items
{
    public class PewterMortar : Container
    {
        [Constructable]
        public PewterMortar()
            : base(0xE9B) 
        {
            Weight = 1.0;
            Name = "a pewter mortar";
            Hue = 1900;
            GumpID = 0x3D;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendMessage("You have to use it in your backpack");
            else
            {
                DisplayTo(from);
                from.CloseGump(typeof(AlchemyPotGump));
                from.SendGump(new AlchemyPotGump(from, this, user_input));
            }
        }

        [Constructable]
        public PewterMortar(Serial serial)
            : base(serial)
        {
        }

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

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item dropped, Point3D p)
        {
            return OnDragDrop(from, dropped);
        }

        public class Ingredient
        {
            public Type ReagentType;
            public ushort Amount;

            public Ingredient(Type T, ushort num)
            {
                this.ReagentType = T;
                this.Amount = num;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Ingredient rhs = (Ingredient)obj;

                return (Amount == rhs.Amount && ReagentType == rhs.ReagentType);
            }

            public override int GetHashCode()
            {
                return ReagentType.GetHashCode()^Amount;
            }
        }

        public class Recipe
        {
            public List<Ingredient> Ingredients = new List<Ingredient>();
            public Type ResultType;
            public float min_Skill;
            public float max_Skill;
            public Ingredient[] IngArray;

            public Recipe()
            {
            }

            public Recipe(Type potiontype, float min_skill, float max_skill, Ingredient [] I)
            {
                IngArray = I;

                foreach (Ingredient Ing in I)
                {
                    Ingredient Ingred = new Ingredient(Ing.ReagentType, Ing.Amount);
                    this.Ingredients.Add(Ingred);
                }

                ResultType = potiontype;
                min_Skill = min_skill;
                max_Skill = max_skill;
            }  

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Recipe rhs = (Recipe)obj;

                if (rhs.Ingredients == null)
                    return false;

                if (Ingredients.Count != rhs.Ingredients.Count)
                    return false;

                foreach (Ingredient i in Ingredients)
                {
                    bool contains = false;
                    foreach (Ingredient j in rhs.Ingredients)
                    {
                        if (i.Equals(j))
                            contains = true;
                    }

                    if (!contains)
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                return Ingredients.GetHashCode();
            }
        }

        public class RecipeManager
        {
            public List<Recipe> known_Recipies = new List<Recipe>();

            /*
            Summon Slime -  2 pearl, 4 bloodmoss, 5 ash, 3 silk
            Incog -         3 shade 3 garlic 1 pearl, 5 mandrake
            Hunger -        2 ash 2 moss, 3 garlic
            RabbitBomb -    3 mandrake 2 pearl 3 garlic
            Silver -        4 pearl 6 garlic 2 mandrake, 3 silk
            Invis -         4 pearl 2 garlic 3 nightshade, 2 silk
            Sparkle -       2 pearl, 1 ash, 2 spiders silk, 4 ginseng
            Polymorph -     6 pearl 3 garlic 2 ginseng 4 mandrake 2nightshade
            RemovePara -    1 pearl 4 garlic 1 ginseng 1 silk 4 root
            Slimebomb -     1 pearl 3 garlic 5 bloodmoss 3 nightshade
            LesserRepel -   1 pearl 6 garlic 6 ginseng, 4 root
            GreaterRepel -  2 pearl 7 garlic 7 ginseng 2 silk 4 mandrake root
            */
            

            public RecipeManager()
            {      
                known_Recipies.Add(new Recipe(( typeof(SummonSlimePotion) ), 50, 100, (new Ingredient[]
                {       
                    new Ingredient(typeof(BlackPearl),2),new Ingredient(typeof(Bloodmoss),4),new Ingredient(typeof(SulfurousAsh),5),new Ingredient(typeof(SpidersSilk),3)
                })));

                known_Recipies.Add(new Recipe(( typeof(IncognitoPotion) ), 75, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(Nightshade),2),new Ingredient(typeof(Garlic), 3), new Ingredient(typeof(BlackPearl), 1),new Ingredient(typeof(MandrakeRoot), 5)
                })));

                known_Recipies.Add(new Recipe((typeof(HungerPotion)), 50, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(SulfurousAsh),2),new Ingredient(typeof(Bloodmoss), 2),new Ingredient(typeof(Garlic), 3)
                })));

                known_Recipies.Add(new Recipe((typeof(RabbitBomb)), 80, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(MandrakeRoot),3),new Ingredient(typeof(BlackPearl), 2), new Ingredient(typeof(Garlic), 3)
                })));

                known_Recipies.Add(new Recipe((typeof(SilverPotion)), 75, 110, (new Ingredient[]
                {
                    new Ingredient(typeof(Garlic),6),new Ingredient(typeof(BlackPearl), 4),new Ingredient(typeof(MandrakeRoot), 2),new Ingredient(typeof(SpidersSilk), 3)
                })));

                known_Recipies.Add(new Recipe((typeof(InvisPotion)), 65, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),4),new Ingredient(typeof(Garlic), 2), new Ingredient(typeof(Nightshade), 3),new Ingredient(typeof(SpidersSilk), 2)
                })));

                known_Recipies.Add(new Recipe((typeof(SparklePotion)), 70, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),2),new Ingredient(typeof(SulfurousAsh), 1), new Ingredient(typeof(SpidersSilk), 2),new Ingredient(typeof(Ginseng), 4)
                })));

                known_Recipies.Add(new Recipe((typeof(PolymorphPotion)), 70, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),6),new Ingredient(typeof(Garlic), 3), new Ingredient(typeof(Ginseng), 2), new Ingredient(typeof(MandrakeRoot), 4),new Ingredient(typeof(Nightshade), 3),
                })));

                known_Recipies.Add(new Recipe((typeof(RemoveParalyzePotion)), 70, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),1),new Ingredient(typeof(Garlic), 4), new Ingredient(typeof(Ginseng), 1), new Ingredient(typeof(SpidersSilk), 1),new Ingredient(typeof(MandrakeRoot), 4)
                })));

                known_Recipies.Add(new Recipe((typeof(SlimeBomb)), 70, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),1),new Ingredient(typeof(Garlic), 3), new Ingredient(typeof(Bloodmoss), 5), new Ingredient(typeof(Nightshade), 3)
                })));

                known_Recipies.Add(new Recipe((typeof(LesserRepelUndeadPotion)), 70, 100, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),1),new Ingredient(typeof(Garlic), 6), new Ingredient(typeof(Ginseng), 6), new Ingredient(typeof(MandrakeRoot), 4)
                })));
                known_Recipies.Add(new Recipe((typeof(GreaterRepelUndeadPotion)), 70, 110, (new Ingredient[]
                {
                    new Ingredient(typeof(BlackPearl),2),new Ingredient(typeof(Garlic), 7),new Ingredient(typeof(BlackPearl), 7), new Ingredient(typeof(SpidersSilk), 2),new Ingredient(typeof(MandrakeRoot), 4)
                })));
            }   
        }

        public void PrintRecipe(Mobile from, Recipe R)
        {
            for (int i = 0; i < R.Ingredients.Count; i++)
            {
                from.SendMessage("Print User Recipe Element");
                from.SendMessage(R.Ingredients[i].ReagentType.ToString());
                from.SendMessage(R.Ingredients[i].Amount.ToString());
            }
        }

        public void AddToRecipe(Recipe R, Item item)
        {
            R.Ingredients.Add(new Ingredient((item.GetType()), (ushort)((BaseReagent)item).Amount));
        }

        public void PrintKnownRecipies(Mobile from)
        {
            foreach (Recipe r in r_manager.known_Recipies)
            {
                foreach (Ingredient i in r.Ingredients)
                {
                    from.SendMessage("Printing Known Recipe element: ");
                    from.SendMessage(i.ReagentType.ToString());
                    from.SendMessage(i.Amount.ToString());
                }
            }
        }

        public void CraftPotion(Mobile from, Type T)
        {
            Type t = T;
            Assembly a = Assembly.GetAssembly(t);
            Object newObject = a.CreateInstance(t.FullName);
            from.PlaySound(0x240); // sound of filling bottle
            from.SendMessage("Your pour the potion into a bottle");
            from.AddToBackpack((Item)newObject); 
        }

        private void GrindMsg(Mobile from)
        {
            from.SendMessage("You grind and grind but come up with a useless concoction, and your ingredients have been destroyed.");
        }

        private static RecipeManager r_manager = new RecipeManager();
        public Recipe user_input = new Recipe();

        public class AlchemyPotGump : Gump
        {
            private double ChanceOfSuccess;
            private PewterMortar m_Owner;
            private Recipe m_user_input;
            private Queue m_Queue = new Queue();
            public AlchemyPotGump(Mobile from, PewterMortar ap, Recipe user_input)
                : base(200, 100)
            {
                m_Owner = ap;
                m_user_input = user_input;
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddPage(0);
                AddBackground(0, 0, 100, 60, 5054);
                this.AddButton(10, 10, 4005, 4005, -1, GumpButtonType.Reply, 0);
                this.AddLabel(45, 10, 0x480, "Grind");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;
                if (info.ButtonID == -1)
                {
                    if (m_Owner == null || !m_Owner.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("That must be in your pack for you to use it.");
                        return;
                    }

                    CheckUserRecipe(from, this.m_user_input);
                    from.CloseGump(typeof(AlchemyPotGump));
                    from.SendGump(new AlchemyPotGump(from, m_Owner, m_user_input));
                }
            }
            
            private bool CheckUserRecipe(Mobile from, Recipe m_user_input)
            {
                Item bottle = from.Backpack.FindItemByType(typeof(Bottle));
                if (bottle == null)
                {
                    from.SendMessage("You need an empty bottle.");
                    m_user_input.Ingredients.Clear();
                    return false;
                }

                foreach (Item item in m_Owner.Items)
                {
                    if (item is BaseReagent)
                    {
                        m_Owner.AddToRecipe(m_user_input, item);
                        //m_Owner.PrintRecipe(from, m_user_input);    //accepts user_input recipe and the player mobile
                        //m_Owner.OnDragDrop(from, item);
                        m_Queue.Enqueue(item);
                    }
                }

                if (m_user_input.Ingredients.Count == 0)
                {
                    from.SendMessage("There are no reagents to grind.");
                    return false;
                }

                from.PlaySound(0x242); // sound of grinding mortar

               while (m_Queue.Count > 0)
                    ((Item)m_Queue.Dequeue()).Delete();

                foreach (Recipe R in r_manager.known_Recipies)
                {
                    if (R.Equals(m_user_input))
                    {
                        ChanceOfSuccess = ((from.Skills.Alchemy.Value - R.min_Skill) / (R.max_Skill - R.min_Skill));
                        if (ChanceOfSuccess > Utility.RandomDouble())
                        {
                            from.CheckSkill(SkillName.Alchemy, 0, 100);
                            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate { m_Owner.CraftPotion(from, R.ResultType); });
                            bottle.Consume(1);
                            m_user_input.Ingredients.Clear();
                            return true;
                        }
                    }
                }

                if (Utility.RandomDouble() < 0.33)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(3), delegate { CheckFailure(from, m_user_input); });
                    return false;
                }
                else
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(3), delegate { m_Owner.GrindMsg(from); });
                    m_user_input.Ingredients.Clear();
                    return false;
                }
            }

            private void CheckFailure(Mobile from, Recipe m_user_input)
            {
                int x = Utility.Random(m_user_input.Ingredients.Count);
                int y = 0;
                foreach (Ingredient I in m_user_input.Ingredients)
                {
                    if (x == y)
                    {
                        switch (I.ReagentType.ToString())
                        {
                            case "Server.Items.BlackPearl":
                                if (I.Amount > 5 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a total Refresh Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(TotalRefreshPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a refresh Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(RefreshPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.Garlic":
                                if (I.Amount > 6 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater cure potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(GreaterCurePotion));
                                    break;
                                }
                                if (I.Amount > 3 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a cure potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(CurePotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a lesser cure potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(LesserCurePotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.SpidersSilk":
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a nightsight potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(AgilityPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.MandrakeRoot":
                                if (I.Amount > 5 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater strength potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(RefreshPotion));
                                    break;
                                }
                                if (I.Amount > 2 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a strength Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(RefreshPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.Nightshade":
                                if (Utility.RandomDouble() < 0.10)
                                {
                                    double z = Utility.RandomDouble();
                                    if (z < .20)
                                    {
                                        from.Poison = Poison.Deadly;
                                    }
                                    if (z < .50)
                                    {
                                        from.Poison = Poison.Greater;
                                    }
                                    if (z < .70)
                                    {
                                        from.Poison = Poison.Regular;
                                    }
                                    else
                                    {
                                        from.Poison = Poison.Lesser;
                                    }
                                    break;
                                }
                                if (I.Amount > 8 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a deadly poison potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(DeadlyPoisonPotion));
                                    break;
                                }
                                if (I.Amount > 4 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater poison potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(GreaterPoisonPotion));
                                    break;
                                }
                                if (I.Amount > 2 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a poison Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(PoisonPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a lesser poison Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(LesserPoisonPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.Bloodmoss":
                                if (I.Amount > 3 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater agility potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(GreaterAgilityPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged an agility Potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(AgilityPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.SulfurousAsh":
                                if (I.Amount > 3 && Utility.RandomDouble() < 0.90)
                                {
                                    from.Hits -= (Utility.Random(10, 20));
                                    from.PlaySound(0x307); // explosion sound
                                    Effects.SendLocationEffect(from.Location, from.Map, 0x36CB, 10);
                                    break;
                                }
                                if (I.Amount > 3 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater explosion potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(GreaterExplosionPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged an explosion potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(ExplosionPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a lesser explosion potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(LesserExplosionPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;

                            case "Server.Items.Ginseng":
                                if (I.Amount > 7 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a greater heal potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(GreaterHealPotion));
                                    break;
                                }
                                if (I.Amount > 3 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a heal potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(HealPotion));
                                    break;
                                }
                                if (I.Amount > 1 && Utility.RandomDouble() < 0.33)
                                {
                                    from.SendMessage("You salvaged a lesser heal potion from the grinder.");
                                    m_Owner.CraftPotion(from, typeof(LesserHealPotion));
                                    break;
                                }
                                m_Owner.GrindMsg(from);
                                break;
                        }
                    }
                    y += 1;
                }
                m_user_input.Ingredients.Clear();
            }
        }
    }
}
