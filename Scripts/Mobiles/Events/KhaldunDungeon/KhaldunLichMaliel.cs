using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;
using Server.Gumps;

namespace Server.Mobiles
{
    [CorpseName("maliel's corpse")]
    public class KhaldunLichMaliel : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public int damageIntervalThreshold = 1000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 30;

        public bool AbilityInProgress = false;

        public List<int> m_DiceValues = new List<int>();
        public List<int> m_DiceHues = new List<int>();

        public List<Mobile> m_Creatures = new List<Mobile>();

        public string[] idleSpeech
        {
            get { return new string[] { "*chants*" }; }
        }

        public string[] combatSpeech
        {
            get { return new string[] { "" }; }
        }

        [Constructable]
        public KhaldunLichMaliel(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Maliel the Hand of Fate";

            Body = 830;
            Hue = 2587;

            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(30000);
            SetStam(5000);
            SetMana(30000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 300);

            SetSkill(SkillName.MagicResist, 150);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {                           
            UniqueCreatureDifficultyScalar = 1.5;

            DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 0;
            DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 0;

            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictWanderAction[WanderAction.None] = 0;
            DictWanderAction[WanderAction.SpellHealSelf100] = 0;
            DictWanderAction[WanderAction.SpellCureSelf] = 0;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            SpellHue = Hue - 1;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool AlwaysBoss { get { return true; } }        
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!willKill)
            {
                if (from != null && amount > 10)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() <= .16)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() <= .08)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() <= .24)
                            DamageEffect(from);
                    }
                }

                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    IntervalEffect();
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public void DamageEffect(Mobile from)
        {
            if (Deleted || !Alive) return;
            if (from == null) return;
            if (from.Deleted || !from.Alive) return;           

            if (AbilityInProgress)
                return;

            if (m_DiceValues.Count >= 4)
                return;   

            Mobile target = from;
            Point3D location = Location;
            Map map = Map;  

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int gumpRange = 24;            
            int gumpItemId = 11351;
            
            int diceHue = 0;
            int diceSpecialHue = 2603;

            int effectSound = 0x456;
            int effectHue = Hue - 1;

            Animate(15, 8, 1, true, false, 0); //Staff

            PublicOverheadMessage(MessageType.Regular, 0, false, "*manipulates fate*");
            PlaySound(effectSound);

            Point3D creatureLocation = Location;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 15, 1, 8, true);

            if (m_ValidLocations.Count > 0)
                creatureLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

            OtherworldlyDenizon otherworldlyDenizon = new OtherworldlyDenizon();
            otherworldlyDenizon.MoveToWorld(creatureLocation, map);
            m_Creatures.Add(otherworldlyDenizon);

            Effects.SendLocationParticles(EffectItem.Create(creatureLocation, map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
            Effects.SendLocationParticles(EffectItem.Create(location, map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
                        
            int diceRoll = Utility.RandomMinMax(1, 6);
            m_DiceValues.Add(diceRoll);   

            List<int> m_DiceHues = new List<int>();    
            for (int a = 0; a < m_DiceValues.Count; a++)
            {
                m_DiceHues.Add(diceHue);
            }

            Queue m_Queue = new Queue();

            IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, gumpRange);

            foreach (Mobile mobile in mobilesInRange)
            {
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                m_Queue.Enqueue(player);                
            }

            mobilesInRange.Free();

            while (m_Queue.Count > 0)
            {
                PlayerMobile player = (PlayerMobile)m_Queue.Dequeue();               

                player.CloseGump(typeof(KhaldunLichMalielGump));
                player.SendGump(new KhaldunLichMalielGump(m_DiceValues, m_DiceHues));
                player.SendSound(effectSound);
            }
        }

        public void IntervalEffect()
        {
            if (Deleted || !Alive) return;                  

            if (m_DiceValues.Count < 4)
                return;            

            double spawnPercent = (double)intervalCount / (double)totalIntervals;      

            Point3D location = Location;
            Map map = Map;

            int gumpRange = 24;
            int gumpItemId = 11351;

            int diceHue = 0;
            int diceSpecialHue = 2603;             
    
            //Add Last Die and Determine Results
            int diceRoll = Utility.RandomMinMax(1, 6);
            m_DiceValues.Add(diceRoll);

            int highestSetValue = 1; //Which Type of Ability
            int highestSetCount = 0; //Intensity of Ability

            for (int a = 1; a < 7; a++)
            {
                int setCount = 0;

                foreach (int value in m_DiceValues)
                {
                    if (a == value)
                        setCount++;
                }

                if (setCount >= highestSetCount)
                {
                    highestSetValue = a;
                    highestSetCount = setCount;
                }
            }

            List<int> m_DiceHues = new List<int>();
            for (int a = 0; a < m_DiceValues.Count; a++)
            {
                if (m_DiceValues[a] == highestSetValue)
                    m_DiceHues.Add(diceSpecialHue);

                else
                    m_DiceHues.Add(diceHue);
            }            

            Queue m_Queue = new Queue();

            IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, gumpRange);

            foreach (Mobile mobile in mobilesInRange)
            {
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                m_Queue.Enqueue(player);
            }

            mobilesInRange.Free();

            while (m_Queue.Count > 0)
            {
                PlayerMobile player = (PlayerMobile)m_Queue.Dequeue();

                player.CloseGump(typeof(KhaldunLichMalielGump));
                player.SendGump(new KhaldunLichMalielGump(m_DiceValues, m_DiceHues));                
            }

            m_DiceValues.Clear();

            switch (highestSetValue)
            {
                case 1: Fortune(highestSetCount); break;     
                case 2: Calibration(highestSetCount); break;
                case 3: Futility(highestSetCount); break;               
                case 4: Oblivion(highestSetCount); break;
                case 5: Judgment(highestSetCount); break;
                case 6: Doom(highestSetCount); break;                
            }              
        }

        public void Fortune(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            
            PublicOverheadMessage(MessageType.Regular, 0, false, "*fate is sealed*");
            PlaySound(0x456);      

            AbilityInProgress = true;

            int castingMotions = 5 - (int)(Math.Ceiling(1 * spawnPercent));
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;
            
            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            CantWalk = true;
            Frozen = true;

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Fortune*");
                PlaySound(0x64F);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    mobile.SendMessage(149, "A benevolent force offers you aid and recovery...");

                    TimedStatic fortune = new TimedStatic(4007, 2);
                    fortune.Name = "Fortune";
                    fortune.Hue = 0;

                    Point3D fortuneLocation = mobileLocation;
                    fortuneLocation.X--;
                    fortuneLocation.Y--;
                    fortuneLocation.Z += 14;
                    fortune.MoveToWorld(fortuneLocation, map);

                    //Heal, Refresh, Restore Mana
                    mobile.FixedParticles(0x376A, 9, 32, 5030, 0, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x299);

                    mobile.Hits += (int)Math.Round((double)mobile.HitsMax * intensity / 5);
                    mobile.Stam += (int)Math.Round((double)mobile.StamMax * intensity / 5);
                    mobile.Mana += (int)Math.Round((double)mobile.ManaMax * intensity / 5);

                    //Reactive Armor
                    if (intensity >= 2)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (mobile == null) return;
                            if (mobile.Deleted || !mobile.Alive) return;

                            if (mobile.MeleeDamageAbsorb < 20)
                            {
                                //Reactive Armor
                                mobile.MeleeDamageAbsorb = 20;

                                mobile.FixedParticles(0x376A, 9, 32, 5008, 0, 0, EffectLayer.Waist);
                                mobile.PlaySound(0x1F2);
                            }
                        });
                    }

                    //Reflect      
                    if (intensity >= 3)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                        {
                            if (mobile == null) return;
                            if (mobile.Deleted || !mobile.Alive) return;
                                             
                            if (mobile.MagicDamageAbsorb == 0)
                            {
                                mobile.MagicDamageAbsorb = 1;

                                mobile.FixedParticles(0x375A, 10, 15, 5037, 0, 0, EffectLayer.Waist);
                                mobile.PlaySound(0x1E9);
                            }
                        });
                    }                    
                }
            });
        }

        public void Calibration(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*seals fate*");
            PlaySound(0x456);

            AbilityInProgress = true;

            int castingMotions = 5 - (int)(Math.Ceiling(1 * spawnPercent));
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Calibration*");
                PlaySound(0x64F);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                List<Point3D> m_ValidLocations = new List<Point3D>();

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                    {
                        m_ValidLocations.Add(mobile.Location);
                        m_Queue.Enqueue(mobile);
                    }
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    Point3D newLocation = location;

                    if (m_ValidLocations.Count > 0)
                    {
                        int locationIndex = Utility.RandomMinMax(0, m_ValidLocations.Count - 1);

                        newLocation = m_ValidLocations[locationIndex];
                        m_ValidLocations.RemoveAt(locationIndex);
                    }                    

                    mobile.Location = newLocation;
                    mobile.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(mobileLocation, map, EffectItem.DefaultDuration), 0x3763, 10, 30, effectHue, 0, 5028, 0);
                    Effects.SendLocationParticles(EffectItem.Create(newLocation, map, EffectItem.DefaultDuration), 0x3763, 10, 30, effectHue, 0, 5028, 0);                 

                    mobile.PlaySound(0x1FE);                                        
                    mobile.SendMessage(149, "Reality seems to bend and the world shifts, disorientating you...");

                    int duration = 5 + (intensity * 5);

                    PlayerMobile player = mobile as PlayerMobile;

                    if (player != null)
                        SpecialAbilities.EntangleSpecialAbility(0.33, this, player, 1.0, duration, 0x1FE, false, "", "");

                    BaseCreature bc_Creature = mobile as BaseCreature;

                    if (bc_Creature != null)
                        bc_Creature.Pacify(this, DateTime.UtcNow + TimeSpan.FromSeconds(duration), false);
                    
                    TimedStatic calibration = new TimedStatic(6225, 2);
                    calibration.Name = "Calibration";
                    calibration.Hue = 2587;

                    Point3D calibrationLocation = mobileLocation;
                    calibrationLocation.X--;
                    calibrationLocation.Y--;
                    calibrationLocation.Z += 14;
                    calibration.MoveToWorld(calibrationLocation, map);
                }
            });
        }

        public void Futility(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*seals fate*");
            PlaySound(0x456);

            AbilityInProgress = true;

            int castingMotions = 5 - (int)(Math.Ceiling(1 * spawnPercent));
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            int spellHue = SpellHue;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Futility*");
                PlaySound(0x64F);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    mobile.SendMessage(149, "A sense of futility makes you doubt your abilities...");

                    //Drain Stamina and Mana
                    mobile.FixedParticles(0x374A, 10, 15, 5028, spellHue, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x1FB);
                    
                    mobile.Stam -= (int)Math.Round((double)mobile.StamMax * intensity / 5);
                    mobile.Mana -= (int)Math.Round((double)mobile.ManaMax * intensity / 5);

                    double effect = .10 * intensity;
                    double duration = 60;

                    SpecialAbilities.BacklashSpecialAbility(1.0, null, mobile, effect, duration, -1, false, "", "");
                    SpecialAbilities.StunSpecialAbility(1.0, null, mobile, effect, duration, -1, false, "", "");

                    mobile.FixedParticles(0x374A, 10, 15, 5028, spellHue, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x1FB);                                       
                }
            });
        }

        public void Oblivion(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*seals fate*");
            PlaySound(0x456);

            AbilityInProgress = true;

            int castingMotions = 5 - (int)(Math.Ceiling(1 * spawnPercent));
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Oblivion*");
                PlaySound(0x104);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                List<Point3D> m_MobileLocations = new List<Point3D>();

                double duration = 5 + intensity;

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    mobile.SendMessage(149, "A wave of vast, unending emptiness washes over you...");
                    
                    SpecialAbilities.PetrifySpecialAbility(1.0, null, mobile, 1.0, duration, -1, true, "", "");

                    mobile.PlaySound(0x104);

                    if (!m_MobileLocations.Contains(mobile.Location))
                        m_MobileLocations.Add(mobile.Location);
                }

                foreach (Point3D point in m_MobileLocations)
                {
                    Rabbit nothingness = new Rabbit();

                    nothingness.Name = "Oblivion";
                    nothingness.Blessed = true;
                    nothingness.Frozen = true;
                    nothingness.BodyValue = 260;
                    nothingness.Hue = 2500;

                    nothingness.AngerSound = 0x104;
                    nothingness.IdleSound = 0x104;
                    nothingness.DeathSound = 0x104;

                    nothingness.MoveToWorld(point, map);

                    Timer.DelayCall(TimeSpan.FromSeconds(duration * .25), delegate
                    {
                        if (nothingness == null) return;
                        if (nothingness.Deleted) return;

                        nothingness.Animate(1, 8, 1, true, false, 0);
                        nothingness.PlaySound(0x104);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(duration * .5), delegate
                    {
                        if (nothingness == null) return;
                        if (nothingness.Deleted) return;

                        nothingness.Animate(1, 8, 1, true, false, 0);
                        nothingness.PlaySound(0x104);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(duration * .75), delegate
                    {
                        if (nothingness == null) return;
                        if (nothingness.Deleted) return;

                        nothingness.Animate(1, 8, 1, true, false, 0);
                        nothingness.PlaySound(0x104);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), delegate
                    {
                        if (nothingness == null) return;
                        if (nothingness.Deleted) return;

                        nothingness.Delete();                        
                    });
                }
            });
        }

        public void Judgment(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*seals fate*");
            PlaySound(0x456);

            AbilityInProgress = true;

            int castingMotions = 5 - (int)(Math.Ceiling(1 * spawnPercent));
            double castingDuration = 1;

            int spellHue = SpellHue;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            int minDamage = (int)(Math.Round((double)DamageMin / 2));
            int maxDamage = (int)(Math.Round((double)DamageMin / 2));

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Judgment*");
                PlaySound(0x64F);

                Animate(15, 8, 1, true, false, 0); //Staff
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
                PlaySound(0x456);

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (Deleted || !Alive) return;

                    Animate(15, 8, 1, true, false, 0); //Staff
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
                    PlaySound(0x456);
                });

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (Deleted || !Alive) return;

                    Animate(15, 8, 1, true, false, 0); //Staff
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
                    PlaySound(0x456);
                });

                Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
                {
                    if (Deleted || !Alive) return;

                    Animate(15, 8, 1, true, false, 0); //Staff
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3740, 10, 30, effectHue, 0, 2023, 0);
                    PlaySound(0x456);
                });

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                double damage = 0;

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;

                    mobile.SendMessage(149, "Judgment arrives...");

                    TimedStatic judgment = new TimedStatic(17087, 2);
                    judgment.Name = "Judgment";
                    judgment.Hue = spellHue;

                    Point3D judgmentLocation = mobileLocation;
                    judgmentLocation.X--;
                    judgmentLocation.Y--;
                    judgmentLocation.Z += 14;
                    judgment.MoveToWorld(judgmentLocation, map);

                    //Harm
                    mobile.FixedParticles(0x374A, 10, 15, 5013, spellHue, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x1F1);

                    damage = Utility.RandomMinMax(minDamage, maxDamage) * (1 + (.10 * intensity));

                    if (mobile is BaseCreature)
                        damage *= 2;

                    new Blood().MoveToWorld(mobileLocation, map);
                    AOS.Damage(mobile, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);                       

                    //Lightning
                    Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;
                        
                        mobile.BoltEffect(0);
                        mobile.PlaySound(0x29);

                        damage = Utility.RandomMinMax(minDamage, maxDamage) * (1 + (.10 * intensity));

                        if (mobile is BaseCreature)
                            damage *= 2;

                        new Blood().MoveToWorld(mobileLocation, map);
                        AOS.Damage(mobile, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);
                    }); 

                    //Explosion                   
                    Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;
                        
                        mobile.FixedParticles(0x36BD, 20, 20, 5044, spellHue, 0, EffectLayer.Head);
                        mobile.PlaySound(0x307);

                        damage = Utility.RandomMinMax(minDamage, maxDamage) * (1 + (.10 * intensity));

                        if (mobile is BaseCreature)
                            damage *= 2;

                        new Blood().MoveToWorld(mobileLocation, map);
                        AOS.Damage(mobile, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);
                    });                    

                    //Flamestrike                    
                    Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;

                        mobile.FixedParticles(0x3709, 10, 30, 5052, spellHue, 0, EffectLayer.LeftFoot);
                        mobile.PlaySound(0x208);

                        damage = Utility.RandomMinMax(minDamage, maxDamage) * (1 + (.10 * intensity));

                        if (mobile is BaseCreature)
                            damage *= 2;

                        new Blood().MoveToWorld(mobileLocation, map);
                        AOS.Damage(mobile, (int)(Math.Ceiling(damage)), 0, 100, 0, 0, 0);
                    });                    
                }
            });
        }

        public void Doom(int intensity)
        {
            if (Deleted || !Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*seals fate*");
            PlaySound(0x456);

            AbilityInProgress = true;

            int castingMotions = 3;
            double castingDuration = 1;

            double stationaryDelay = castingMotions * castingDuration + 1;

            Point3D location = Location;
            Map map = Map;

            int range = 12 + (int)(Math.Ceiling(24 * spawnPercent));
            int effectHue = Hue - 1;

            Combatant = null;
            NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(stationaryDelay);

            AIObject.NextMove = AIObject.NextMove + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(stationaryDelay);
            NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(stationaryDelay);
            NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "");

            for (int a = 0; a < castingMotions; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * castingDuration), delegate
                {
                    if (Deleted || !Alive) return;

                    CantWalk = true;
                    Frozen = true;

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Animate(12, 12, 1, true, false, 0);
                });
            }   

            Timer.DelayCall(TimeSpan.FromSeconds((double)castingMotions * castingDuration), delegate
            {
                if (Deleted || !Alive) return;

                CantWalk = false;
                Frozen = false;

                AbilityInProgress = false;

                PublicOverheadMessage(MessageType.Regular, 0, false, "*Fate: Doom*");
                PlaySound(0x246);

                Queue m_Queue = new Queue();

                IPooledEnumerable mobilesInRange = map.GetMobilesInRange(location, range);

                foreach (Mobile mobile in mobilesInRange)
                {
                    if (mobile.Deleted) continue;
                    if (!mobile.Alive) continue;
                    if (mobile == this) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;

                    bool validTarget = false;

                    PlayerMobile pm_Target = mobile as PlayerMobile;
                    BaseCreature bc_Target = mobile as BaseCreature;

                    if (pm_Target != null)
                        validTarget = true;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    if (validTarget)
                        m_Queue.Enqueue(mobile);
                }

                mobilesInRange.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();
                    Point3D mobileLocation = mobile.Location;                    
                    
                    Effects.PlaySound(mobileLocation, map, 0x246);

                    int projectiles = 4 * intensity;
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(mobile.Location, true, false, mobile.Location, mobile.Map, 1, 15, 5, 10, true);

                        if (m_ValidLocations.Count > 0)
                            newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                        else
                            continue;

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(mobileLocation.X, mobileLocation.Y, mobileLocation.Z + 5), map);

                        if (Utility.RandomDouble() <= .5)
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 2053, 0);
                        else
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 8707, particleSpeed, 0, false, false, 0, 0);
                    } 

                    double damage = Utility.RandomMinMax(15, 25) * intensity;                   

                    if (mobile is BaseCreature)
                        damage *= 3.0;

                    mobile.SendMessage(149, "An impending doom fills your soul with dread...");
                    mobile.SendSound(0x246);

                    Timer.DelayCall(TimeSpan.FromSeconds(15), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;

                        mobile.SendMessage(149, "Doom approaches....");
                        mobile.SendSound(0x246);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(25), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;

                        mobile.SendMessage(149, "Doom is nearly upon you....");
                        mobile.SendSound(0x246);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
                    {
                        if (Deleted || !Alive) return;
                        if (mobile == null) return;
                        if (mobile.Deleted || !mobile.Alive) return;

                        mobile.SendMessage(149, "Doom is at hand!");
                        mobile.SendSound(0x246);

                        IEntity doomLocationEntity = new Entity(Serial.Zero, new Point3D(mobile.X, mobile.Y, mobile.Z), map);
                        Effects.SendLocationParticles(doomLocationEntity, 0x3709, 10, 30, 2053, 0, 5052, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x653);

                        projectiles = 4 * intensity;
                        int blood = 5 * intensity;

                        particleSpeed = 4;

                        PublicOverheadMessage(MessageType.Regular, 149, false, "*doom*");

                        for (int a = 0; a < projectiles; a++)
                        {
                            Point3D newLocation;

                            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(mobile.Location, true, false, mobile.Location, mobile.Map, 1, 15, 5, 10, true);

                            if (m_ValidLocations.Count > 0)
                                newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                            else
                                continue;

                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z + 5), map);
                            IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 10), map);

                            if (Utility.RandomDouble() <= .5)
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3728, particleSpeed, 0, false, false, 2053, 0);
                            else
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 8707, particleSpeed, 0, false, false, 0, 0);
                        } 

                        for (int a = 0; a < blood; a++)
                        {
                            Point3D bloodlocation = new Point3D(mobile.Location.X + Utility.RandomList(-2, 2), mobile.Location.Y + Utility.RandomList(-2, 2), mobile.Location.Z);
                            SpellHelper.AdjustField(ref bloodlocation, map, 12, false);

                            new Blood().MoveToWorld(bloodlocation, mobile.Map);
                        }
                        
                        AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                    });
                }
            });
        }

        public override void OnThink()
        {
            base.OnThink();

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            
            if (Utility.RandomDouble() < 0.01 && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*cackles*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*sneers*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*scowls*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*seethes*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*curses*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestResist] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                    break;
                }

                Animate(27, 10, 1, true, false, 0); //Sneer
                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }        

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (!m_Creatures[a].Deleted)
                        m_Creatures[a].Delete();
                }
            }
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return 4; } }
        public override int AttackFrames { get { return 10; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 8; } }

        public override int IdleAnimation { get { return -1; } }
        public override int IdleFrames { get { return 0; } }

        public override int GetAngerSound() { return 0x2BC; }
        public override int GetIdleSound() { return 0x2B9; }
        public override int GetAttackSound() { return 0x2BA; }
        public override int GetHurtSound() { return 0x621; }
        public override int GetDeathSound() { return 0x58D; }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x654);

            return base.OnMove(d);
        }

        public KhaldunLichMaliel(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(damageIntervalThreshold);
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(totalIntervals);

            writer.Write(m_DiceValues.Count);
            for (int a = 0; a < m_DiceValues.Count; a++)
            {
                writer.Write(m_DiceValues[a]);
            }

            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_DiceValues = new List<int>();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            int diceCount = reader.ReadInt();
            for (int a = 0; a < diceCount; a++)
            {
                m_DiceValues.Add(reader.ReadInt());
            }

            m_Creatures = new List<Mobile>();

            int creaturesCount = reader.ReadInt();
            for (int a = 0; a < creaturesCount; a++)
            {
                Mobile creature = reader.ReadMobile();

                if (creature != null)
                    m_Creatures.Add(creature);
            }
        }

        public class KhaldunLichMalielGump : Gump
        {
            public KhaldunLichMalielGump(List<int> m_DiceValues, List<int> m_DiceHues): base(200, 200)
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddImage(3, 2, 103, 2499);

                int textHue = 2036;

                AddLabel(40, 14, textHue, "Maliel's Fate");

                int itemIDBase = 11280;

                List<Point2D> m_Coordinates = new List<Point2D>();

                m_Coordinates.Add(new Point2D(23, 64));
                m_Coordinates.Add(new Point2D(44, 37));
                m_Coordinates.Add(new Point2D(66, 64));
                m_Coordinates.Add(new Point2D(87, 37));
                m_Coordinates.Add(new Point2D(108, 64));

                for (int a = 0; a < m_DiceValues.Count; a++)
                {
                    AddImage(m_Coordinates[a].X, m_Coordinates[a].Y, itemIDBase + m_DiceValues[a] - 1, m_DiceHues[a]);                    
                }            
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
            }
        }
    }
}



