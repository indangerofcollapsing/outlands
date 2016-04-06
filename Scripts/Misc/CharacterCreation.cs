using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;


namespace Server.Misc
{
    public class CharacterCreation
    {
        public static readonly bool FreezeNewbieItems = true;

        public static int MinStartingStatValue = 10;
        public static int MaxStartingStatValue = 60;
        public static double MaxStartingSkillValue = 50.0;

        public static void Initialize()
        {
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(EventSink_CharacterCreated);
        }

        private static void AddBackpack(Mobile m)
        {
            Container pack = m.Backpack;

            if (pack == null)
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem(pack);
            }

            PackItem(new RedBook("a book", m.Name, 20, true));
            PackItem(new Candle());

            PackItem(MakeNewbie(new Hatchet()));
            PackItem(MakeNewbie(new Shovel()));

            PackItem(MakeNewbie(new Dagger()));
            PackItem(MakeNewbie(new Scissors()));            

            PackItem(new Gold(50));
            PackItem(new TrainingCreditDeed());
            PackItem(new NewbieDungeonRune());

            m.StatCap = 225;
        }

        private static Item MakeNewbie(Item item)
        {           
            item.LootType = LootType.Newbied;

            return item;
        }

        private static void PlaceItemIn(Container parent, int x, int y, Item item)
        {
            parent.AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }
                
        private static void AddShirt(Mobile m, int shirtHue)
        {
            int hue = Utility.ClipDyedHue(shirtHue & 0x3FFF);
            
            switch (Utility.Random(3))
            {
                case 0: EquipItem(new Shirt(hue), true); break;
                case 1: EquipItem(new FancyShirt(hue), true); break;
                case 2: EquipItem(new Doublet(hue), true); break;
            }            
        }

        private static void AddPants(Mobile m, int pantsHue)
        {
            int hue = Utility.ClipDyedHue(pantsHue & 0x3FFF);
            
            if (m.Female)
            {
                switch (Utility.Random(2))
                {
                    case 0: EquipItem(new Skirt(hue), true); break;
                    case 1: EquipItem(new Kilt(hue), true); break;
                }
            }

            else
            {
                switch (Utility.Random(2))
                {
                    case 0: EquipItem(new LongPants(hue), true); break;
                    case 1: EquipItem(new ShortPants(hue), true); break;
                }
            }            
        }

        private static void AddShoes(Mobile m)
        {           
            EquipItem(new Shoes(Utility.RandomYellowHue()), true);
        }

        private static Mobile CreateMobile(Account a)
        {
            if (a.Count >= a.Limit)
                return null;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] == null)
                {
                    PlayerMobile pm = new PlayerMobile();
                    pm.CreatedOn = DateTime.UtcNow;
                    a[i] = pm;
                    return a[i];
                }
            }

            return null;
        }

        private static void EventSink_CharacterCreated(CharacterCreatedEventArgs args)
        {
            NetState state = args.State;

            if (state == null)
                return;

            Mobile newChar = CreateMobile(args.Account as Account);

            if (newChar == null)
            {
                Console.WriteLine("Login: {0}: Character creation failed, account full", state);
                return;
            }

            args.Mobile = newChar;
            m_Mobile = newChar;

            newChar.Player = true;
            newChar.AccessLevel = args.Account.AccessLevel;
            newChar.Female = args.Female;

            if (Core.Expansion >= args.Race.RequiredExpansion)
                newChar.Race = args.Race;	//Sets body
            else
                newChar.Race = Race.DefaultRace;
           
            newChar.Hue = newChar.Race.ClipSkinHue(args.Hue & 0x3FFF) | 0x8000;

            newChar.Hunger = Food.CharacterCreationHunger;

            bool young = true;

            if (newChar is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)newChar;

                pm.Profession = args.Profession;

                Account account = pm.Account as Account;

                if (pm.AccessLevel == AccessLevel.Player && account.Young)
                    young = pm.Young = true;
            }   

            SetName(newChar, args.Name);

            AddBackpack(newChar);
            
            SetStats(newChar, state, args.Str, args.Dex, args.Int);
            SetSkills(newChar, args.Skills, args.Profession);

            Race race = newChar.Race;

            if (race.ValidateHair(newChar, args.HairID))
            {
                newChar.HairItemID = args.HairID;
                newChar.HairHue = race.ClipHairHue(args.HairHue & 0x3FFF);
            }

            if (race.ValidateFacialHair(newChar, args.BeardID))
            {
                newChar.FacialHairItemID = args.BeardID;
                newChar.FacialHairHue = race.ClipHairHue(args.BeardHue & 0x3FFF);
            }
                       
            AddShirt(newChar, args.ShirtHue);
            AddPants(newChar, args.PantsHue);
            AddShoes(newChar);            
            
            if (young)
            {
                NewPlayerTicket ticket = new NewPlayerTicket();
                ticket.Owner = newChar;
                newChar.BankBox.DropItem(ticket);
            }
            
            CityInfo[] ci = new CityInfo[12];

            ci[0] = new CityInfo("", "", 1503, 1621, 21, Map.Felucca);
            ci[1] = new CityInfo("", "", 1503, 1613, 21, Map.Felucca);
            ci[2] = new CityInfo("", "", 1431, 1720, 20, Map.Felucca);
            ci[3] = new CityInfo("", "", 1495, 1623, 20, Map.Felucca);
            ci[4] = new CityInfo("", "", 1587, 1596, 20, Map.Felucca);
            ci[5] = new CityInfo("", "", 1587, 1586, 20, Map.Felucca);
            ci[6] = new CityInfo("", "", 1579, 1596, 20, Map.Felucca);
            ci[7] = new CityInfo("", "", 1579, 1586, 20, Map.Felucca);
            ci[8] = new CityInfo("", "", 1495, 1687, 20, Map.Felucca);
            ci[9] = new CityInfo("", "", 1503, 1620, 21, Map.Felucca);
            ci[10] = new CityInfo("", "", 1503, 1614, 21, Map.Felucca);
            ci[11] = new CityInfo("", "", 1494, 1608, 21, Map.Felucca);

            Random rand = new Random();
            int cityIndex = rand.Next(0, 11);

            CityInfo city = ci[cityIndex];

            newChar.MoveToWorld(city.Location, city.Map);
            
            new WelcomeTimer(newChar).Start();
        }

        public static bool VerifyProfession(int profession)
        {
            if (profession < 0)
                return false;

            else if (profession < 4)
                return true;

            else if (Core.AOS && profession < 6)
                return true;

            else if (Core.SE && profession < 8)
                return true;

            else
                return false;
        }

        private class BadStartMessage : Timer
        {
            Mobile m_Mobile;
            int m_Message;

            public BadStartMessage(Mobile m, int message) : base(TimeSpan.FromSeconds(3.5))
            {
                m_Mobile = m;
                m_Message = message;

                Start();
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage(m_Message);
            }
        }

        private static CityInfo GetStartLocation(CharacterCreatedEventArgs args, bool isYoung)
        {
            bool useHaven = isYoung;

            ClientFlags flags = args.State == null ? ClientFlags.None : args.State.Flags;
            Mobile m = args.Mobile;

            return args.City;
        }

        private static void FixStats(ref int str, ref int dex, ref int intel, int max)
        {
            int vMax = max - 30;

            int vStr = str - 10;
            int vDex = dex - 10;
            int vInt = intel - 10;

            if (vStr < 0)
                vStr = 0;

            if (vDex < 0)
                vDex = 0;

            if (vInt < 0)
                vInt = 0;

            int total = vStr + vDex + vInt;

            if (total == 0 || total == vMax)
                return;

            double scalar = vMax / (double)total;

            vStr = (int)(vStr * scalar);
            vDex = (int)(vDex * scalar);
            vInt = (int)(vInt * scalar);

            FixStat(ref vStr, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vDex, (vStr + vDex + vInt) - vMax, vMax);
            FixStat(ref vInt, (vStr + vDex + vInt) - vMax, vMax);

            str = vStr + 10;
            dex = vDex + 10;
            intel = vInt + 10;
        }

        private static void FixStat(ref int stat, int diff, int max)
        {
            stat += diff;

            if (stat < 0)
                stat = 0;

            else if (stat > max)
                stat = max;
        }

        private static void SetStats(Mobile m, NetState state, int str, int dex, int intel)
        {
            int max = state.NewCharacterCreation ? 90 : 80;

            FixStats(ref str, ref dex, ref intel, max);

            if (str < MinStartingStatValue || str > MaxStartingStatValue || dex < MinStartingStatValue || dex > MaxStartingStatValue || intel < MinStartingStatValue || intel > MaxStartingStatValue || (str + dex + intel) != max)
            {
                str = MaxStartingStatValue;
                dex = 10;
                intel = 10;
            }

            m.InitStats(str, dex, intel);
        }

        private static void SetName(Mobile m, string name)
        {
            name = name.Trim();

            if (!NameVerification.Validate(name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
                name = "Generic Player";

            m.Name = name;
        }

        private static bool ValidSkills(SkillNameValue[] skills)
        {
            int total = 0;

            for (int i = 0; i < skills.Length; ++i)
            {
                if (skills[i].Value < 0 || skills[i].Value > (int)MaxStartingSkillValue)
                    return false;

                total += skills[i].Value;

                for (int j = i + 1; j < skills.Length; ++j)
                {
                    if (skills[j].Value > 0 && skills[j].Name == skills[i].Name)
                        return false;
                }
            }

            return (total == 100 || total == 120);
        }

        private static Mobile m_Mobile;

        private static void SetSkills(Mobile mobile, SkillNameValue[] skills, int prof)
        {
            switch (prof)
            {
                case 1: // Warrior
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.Anatomy, 30 ),
						new SkillNameValue( SkillName.Healing, 45 ),
						new SkillNameValue( SkillName.Swords, 35 ),
						new SkillNameValue( SkillName.Tactics, 50 )
					};

                    break;
                }

                case 2: // Magician
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.EvalInt, 30 ),
						new SkillNameValue( SkillName.Wrestling, 30 ),
						new SkillNameValue( SkillName.Magery, 50 ),
						new SkillNameValue( SkillName.Meditation, 50 )
					};

                    break;
                }

                case 3: // Blacksmith
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.Mining, 30 ),
						new SkillNameValue( SkillName.ArmsLore, 30 ),
						new SkillNameValue( SkillName.Blacksmith, 50 ),
						new SkillNameValue( SkillName.Tinkering, 50 )
					};

                    break;
                }

                case 4: // Necromancer
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.SpiritSpeak, 30 ),
						new SkillNameValue( SkillName.Swords, 30 ),
						new SkillNameValue( SkillName.Tactics, 20 )
					};

                    break;
                }

                case 5: // Paladin
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.Swords, 49 ),
						new SkillNameValue( SkillName.Tactics, 30 )
					};

                    break;
                }

                case 6:	//Samurai
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.Swords, 50 ),
						new SkillNameValue( SkillName.Anatomy, 30 ),
						new SkillNameValue( SkillName.Healing, 30 )
				    };

                    break;
                }

                case 7:	//Ninja
                {
                    skills = new SkillNameValue[]
					{
						new SkillNameValue( SkillName.Hiding, 50 ),
						new SkillNameValue( SkillName.Fencing, 30 ),
						new SkillNameValue( SkillName.Stealth, 30 )
					};
                    break;
                }

                default:
                {
                    if (!ValidSkills(skills))                    
                        return;                    

                    break;
                }
            }

            bool addSkillItems = true;

            //Overrides
            int bowcraftValueToAssign = 0;

            bool hasCarpentry = false;
            bool hasLumberjacking = false;

            bool increasedCarpentry = false;
            bool increasedLumberjacking = false;            

            for (int i = 0; i < skills.Length; ++i)
            {
                SkillNameValue skillNameValue = skills[i];
                
                if (skillNameValue.Value > 0)
                {
                    if (skillNameValue.Name == SkillName.Carpentry)
                        hasCarpentry = true;

                    if (skillNameValue.Name == SkillName.Lumberjacking)
                        hasLumberjacking = true;

                    if (skillNameValue.Name == SkillName.Bowcraft)
                    {
                        bowcraftValueToAssign += skillNameValue.Value * 10;
                        continue;
                    }

                    Skill skill = mobile.Skills[skillNameValue.Name];
                    
                    if (skill != null)
                    {
                        skill.BaseFixedPoint = skillNameValue.Value * 10;

                        if (addSkillItems)
                            AddSkillItems(skillNameValue.Name, mobile);
                    }
                }
            }

            int MaxStartingSkillBaseFixedPoint = (int)MaxStartingSkillValue * 10;

            //Convert Bowcraft to Carpentry
            if (bowcraftValueToAssign > 0)
            {
                if (mobile.Skills.Carpentry.BaseFixedPoint < MaxStartingSkillBaseFixedPoint)
                {
                    int availableCarpentry = MaxStartingSkillBaseFixedPoint - mobile.Skills.Carpentry.BaseFixedPoint;
                    int bowcraftToCarpentry = 0;

                    if (bowcraftValueToAssign > availableCarpentry)
                    {
                        bowcraftToCarpentry = availableCarpentry;
                        bowcraftValueToAssign -= availableCarpentry;

                        mobile.Skills.Carpentry.BaseFixedPoint += bowcraftToCarpentry;
                    }

                    else
                    {
                        mobile.Skills.Carpentry.BaseFixedPoint += bowcraftValueToAssign;
                        bowcraftValueToAssign = 0;
                    }
                         
                    increasedCarpentry = true;

                    AddSkillItems(SkillName.Carpentry, mobile);
                }
            }

            //Convert Remaining Bowcraft to Lumberjacking
            if (bowcraftValueToAssign > 0)
            {
                if (mobile.Skills.Lumberjacking.BaseFixedPoint < MaxStartingSkillBaseFixedPoint)
                {
                    int availableLumberjacking = MaxStartingSkillBaseFixedPoint - mobile.Skills.Lumberjacking.BaseFixedPoint;
                    int bowcraftToLumberjacking = 0;

                    if (bowcraftValueToAssign > availableLumberjacking)
                    {
                        bowcraftToLumberjacking = availableLumberjacking;
                        bowcraftValueToAssign -= availableLumberjacking;

                        mobile.Skills.Lumberjacking.BaseFixedPoint += bowcraftToLumberjacking;
                    }

                    else
                    {
                        mobile.Skills.Lumberjacking.BaseFixedPoint += bowcraftValueToAssign;
                        bowcraftValueToAssign = 0;
                    }

                    increasedLumberjacking = true;

                    AddSkillItems(SkillName.Lumberjacking, mobile);
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
            {
                if (mobile == null)
                    return;

                if (increasedCarpentry && !increasedLumberjacking)
                    mobile.SendMessage(0x3F, "The Bowcraft skill has been merged into the Carpentry skill and you have received an increase in Carpentry as compensation.");

                else if (!increasedCarpentry && increasedLumberjacking)
                    mobile.SendMessage(0x3F, "The Bowcraft skill has been merged into the Carpentry skill and you have received an increase in Lumberjacking as compensation.");

                else if (increasedCarpentry && increasedLumberjacking)
                    mobile.SendMessage(0x3F, "The Bowcraft skill has been merged into the Carpentry skill and you have received increases in Carpentry and Lumberjacking as compensation.");
            });
        }

        private static void EquipItem(Item item)
        {
            EquipItem(item, false);
        }

        private static void EquipItem(Item item, bool mustEquip)
        {
            item.LootType = LootType.Newbied;

            if (m_Mobile != null && m_Mobile.EquipItem(item))
                return;

            Container pack = m_Mobile.Backpack;

            if (!mustEquip && pack != null)
                pack.DropItem(item);

            else
                item.Delete();
        }

        private static void PackItem(Item item)
        {
            item.LootType = LootType.Newbied;

            Container pack = m_Mobile.Backpack;

            if (pack != null)
                pack.DropItem(item);

            else
                item.Delete();
        }

        private static void PackInstrument()
        {
            switch (Utility.Random(6))
            {
                case 0: PackItem(new Drums()); break;
                case 1: PackItem(new Harp()); break;
                case 2: PackItem(new LapHarp()); break;
                case 3: PackItem(new Lute()); break;
                case 4: PackItem(new Tambourine()); break;
                case 5: PackItem(new TambourineTassel()); break;
            }
        }

        private static void PackScroll(int circle)
        {
            switch (Utility.Random(8) * (circle + 1))
            {
                case 0: PackItem(new ClumsyScroll()); break;
                case 1: PackItem(new CreateFoodScroll()); break;
                case 2: PackItem(new FeeblemindScroll()); break;
                case 3: PackItem(new HealScroll()); break;
                case 4: PackItem(new MagicArrowScroll()); break;
                case 5: PackItem(new NightSightScroll()); break;
                case 6: PackItem(new ReactiveArmorScroll()); break;
                case 7: PackItem(new WeakenScroll()); break;
                case 8: PackItem(new AgilityScroll()); break;
                case 9: PackItem(new CunningScroll()); break;
                case 10: PackItem(new CureScroll()); break;
                case 11: PackItem(new HarmScroll()); break;
                case 12: PackItem(new MagicTrapScroll()); break;
                case 13: PackItem(new MagicUnTrapScroll()); break;
                case 14: PackItem(new ProtectionScroll()); break;
                case 15: PackItem(new StrengthScroll()); break;
                case 16: PackItem(new BlessScroll()); break;
                case 17: PackItem(new FireballScroll()); break;
                case 18: PackItem(new MagicLockScroll()); break;
                case 19: PackItem(new PoisonScroll()); break;
                case 20: PackItem(new TelekinisisScroll()); break;
                case 21: PackItem(new TeleportScroll()); break;
                case 22: PackItem(new UnlockScroll()); break;
                case 23: PackItem(new WallOfStoneScroll()); break;
            }
        }

        private static void AddSkillItems(SkillName skill, Mobile m)
        {
            switch (skill)
            {
                case SkillName.Alchemy:
                {
                    PackItem(new Bottle(4));
                    PackItem(new MortarPestle());

                    int hue = 1867;                    
                    EquipItem(new Robe(hue));
                    break;
                }

                case SkillName.Anatomy:
                {
                    int hue = 1867;                    
                    EquipItem(new Robe(hue));
                    break;
                }

                case SkillName.AnimalLore:
                {
                    int hue = 1867;

                    EquipItem(MakeNewbie(new ShepherdsCrook()));
                    EquipItem(new Robe(hue));
                    break;
                }

                case SkillName.Archery:
                {
                    PackItem(MakeNewbie(new Arrow(125)));                    
                    EquipItem(MakeNewbie(new Bow()));
                    break;
                }

                case SkillName.ArmsLore:
                {                        
                    switch (Utility.Random(3))
                    {
                        case 0: EquipItem(MakeNewbie(new Kryss())); break;
                        case 1: EquipItem(MakeNewbie(new Katana())); break;
                        case 2: EquipItem(MakeNewbie(new Club())); break;
                    }                        

                    break;
                }

                case SkillName.Begging:
                {                    
                    EquipItem(MakeNewbie(new GnarledStaff()));
                    break;
                }

                case SkillName.Blacksmith:
                {
                    PackItem(MakeNewbie(new Tongs()));
                    PackItem(MakeNewbie(new Pickaxe()));
                    PackItem(MakeNewbie(new Pickaxe()));                   
                    EquipItem(new HalfApron(Utility.RandomYellowHue()));
                    break;
                }

                case SkillName.Tinkering:
                {
                    PackItem(MakeNewbie(new TinkerTools()));
                    PackItem(MakeNewbie(new TinkerTools()));                    
                    EquipItem(new HalfApron(Utility.RandomYellowHue()));
                    break;
                }

                case SkillName.Camping:
                {
                    PackItem(MakeNewbie(new Bedroll()));
                    PackItem(new Kindling(20));
                    break;
                }

                case SkillName.Carpentry:
                {                   
                    PackItem(new Saw());
                    EquipItem(new HalfApron(Utility.RandomYellowHue()));
                    break;
                }

                case SkillName.Cartography:
                {
                    PackItem(new BlankMap());
                    PackItem(new BlankMap());
                    PackItem(new BlankMap());
                    PackItem(new BlankMap());
                    PackItem(new Sextant());
                    break;
                }

                case SkillName.Cooking:
                {
                    PackItem(new Kindling(2));
                    PackItem(new SackOfFlour(3));
                    PackItem(new Pitcher(BeverageType.Water));
                    break;
                }

                case SkillName.DetectHidden:
                {
                    EquipItem(new Cloak());
                    break;
                }

                case SkillName.Discordance:
                {
                    PackInstrument();
                    break;
                }

                case SkillName.Fencing:
                {
                        
                    EquipItem(MakeNewbie(new Kryss()));
                    break;
                }

                case SkillName.Fishing:
                {
                    EquipItem(MakeNewbie(new FishingPole()));

                    int hue = Utility.RandomYellowHue();
                    
                    EquipItem(new FloppyHat(Utility.RandomYellowHue()));  
                    break;
                }

                case SkillName.Healing:
                {                    
                    PackItem(new Scissors());
                    break;
                }

                case SkillName.Herding:
                {
                    
                    EquipItem(MakeNewbie(new ShepherdsCrook()));
                    break;
                }

                case SkillName.Hiding:
                {
                    EquipItem(new Cloak());
                    break;
                }

                case SkillName.Inscribe:
                {
                    PackItem(new BlankScroll(2));
                    PackItem(new BlueBook());
                    break;
                }

                case SkillName.ItemID:
                {                    
                    EquipItem(MakeNewbie(new GnarledStaff()));
                    break;
                }

                case SkillName.Lockpicking:
                {
                    PackItem(MakeNewbie(new Lockpick(20)));
                    break;
                }

                case SkillName.Lumberjacking:
                {
                    EquipItem(MakeNewbie(new Hatchet()));
                    break;
                }

                case SkillName.Macing:
                {                   
                    EquipItem(MakeNewbie(new Club()));
                    break;
                }

                case SkillName.Magery:
                {
                    PackItem(MakeNewbie(new BlackPearl(30)));
                    PackItem(MakeNewbie(new Bloodmoss(30)));
                    PackItem(MakeNewbie(new Garlic(30)));
                    PackItem(MakeNewbie(new Ginseng(30)));
                    PackItem(MakeNewbie(new MandrakeRoot(30)));
                    PackItem(MakeNewbie(new Nightshade(30)));
                    PackItem(MakeNewbie(new SulfurousAsh(30)));
                    PackItem(MakeNewbie(new SpidersSilk(30)));

                    PackScroll(0);
                    PackScroll(1);
                    PackScroll(2);

                    Spellbook book = new Spellbook((ulong)0x382A8C38);

                    EquipItem(book);

                    int hue = 1867;

                    book.LootType = LootType.Blessed;
                   
                    EquipItem(new WizardsHat());
                    EquipItem(new Robe(hue));
                    break;
                }

                case SkillName.Mining:
                {
                    PackItem(MakeNewbie(new Pickaxe()));
                    PackItem(MakeNewbie(new Pickaxe()));
                    break;
                }

                case SkillName.Musicianship:
                {
                    PackInstrument();
                    PackInstrument();
                    break;
                }

                case SkillName.Parry:
                {
                    EquipItem(MakeNewbie(new WoodenShield()));
                    break;
                }

                case SkillName.Peacemaking:
                {
                    PackInstrument();
                    break;
                }

                case SkillName.Poisoning:
                {
                    PackItem(new LesserPoisonPotion());
                    PackItem(new LesserPoisonPotion());
                    break;
                }

                case SkillName.Provocation:
                {
                    PackInstrument();
                    break;
                }

                case SkillName.Snooping:
                {
                    PackItem(MakeNewbie(new Lockpick(20)));
                    break;
                }

                case SkillName.SpiritSpeak:
                {
                    EquipItem(new Cloak());
                    break;
                }

                case SkillName.Stealing:
                {
                    PackItem(MakeNewbie(new Lockpick(20)));
                    break;
                }

                case SkillName.Swords:
                {                   
                    EquipItem(MakeNewbie(new Katana()));
                    break;
                }

                case SkillName.Tactics:
                {                    
                    EquipItem(MakeNewbie(new Katana()));
                    EquipItem(MakeNewbie(new StuddedChest()));
                    break;
                }

                case SkillName.Tailoring:
                {                  
                    PackItem(MakeNewbie(new SewingKit()));
                    break;
                }

                case SkillName.Tracking:
                {
                    if (m_Mobile != null)
                    {
                        Item shoes = m_Mobile.FindItemOnLayer(Layer.Shoes);

                        if (shoes != null)
                            shoes.Delete();
                    }

                    int hue = Utility.RandomYellowHue();

                    EquipItem(new Boots(hue));
                    EquipItem(new SkinningKnife());
                    break;
                }

                case SkillName.Veterinary:
                {
                    PackItem(new Scissors());
                    break;
                }

                case SkillName.Wrestling:
                {
                    EquipItem(MakeNewbie(new LeatherGloves()));
                    break;
                }
            }
        }
    }
}