using System;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using System.Collections.Specialized;
using Server.Spells.Third;
using System.Net;


namespace Server.Custom
{
    public class ZombiePlayerState
    {
        public DateTime Start { get; set; }
        public int KillCount { get; set; }
        public Point3D ReturnLocation { get; set; }

        public ZombiePlayerState(Point3D returnLoc)
        {
            Start = DateTime.UtcNow;
            KillCount = 0;
            ReturnLocation = returnLoc;
        }
    }

    // Holds the information for ranking
    public class Rank
    {
        public Mobile Player;
        public TimeSpan Duration;

        public Rank()
        {
        }

        public Rank(Mobile player, TimeSpan duration)
        {
            Player = player;
            Duration = duration;
        }
    }

    public class ZombieQuestRegion : Region
    {

        public static readonly Rectangle2D ZombieRegionRectagle = new Rectangle2D(new Point2D(1000, 1300), new Point2D(1800, 2000));
        public static readonly Point3D SweetDreamsInn = new Point3D(1495, 1629, 10);
        public static readonly Map Facet = Map.Trammel;

        public static readonly Point3D[] EntranceLocations = new Point3D[] {
            new Point3D(1496,1629,10), new Point3D(1513,1614,10), new Point3D(1440,1631,20), 
            new Point3D(1411,1615,30), new Point3D(1433,1731,20)
        };

        private static Dictionary<IPAddress, DateTime> IPMarkers = new Dictionary<IPAddress, DateTime>();
        private static Dictionary<Mobile, ZombiePlayerState> Registry = new Dictionary<Mobile, ZombiePlayerState>();

        public static List<Rank> TopTen = new List<Rank>();

        public static void Initialize()
        {
            ZombieQuestRegion zqRegion = new ZombieQuestRegion("ZombieQuest", Map.Trammel, 10, new Rectangle2D[] { ZombieRegionRectagle });
            zqRegion.Register();
        }

        public ZombieQuestRegion(string name, Map map, int priority, Rectangle2D[] area)
            : base(name, map, priority, area)
        {
        }

        public static bool CanJoin(Mobile m)
        {
            if (m == null || m.NetState == null)
                return false;

            IPAddress ip = m.NetState.Address;

            DateTime nextJoinTime;

            return !IPMarkers.TryGetValue(ip, out nextJoinTime) || DateTime.UtcNow > nextJoinTime;
        }

        public static void Join(Mobile m)
        {
            if (m.NetState != null)
            {
                IPAddress ip = m.NetState.Address;

                IPMarkers.Remove(ip);
                IPMarkers.Add(ip, DateTime.UtcNow + TimeSpan.FromMinutes(10));
            }

            m.Hits = m.HitsMax;
            m.Stam = m.StamMax;
            m.Mana = m.ManaMax;

            // Check to see if player is in Zombie Quest
            if (!Registry.ContainsKey(m))
            {
                Registry.Add(m, new ZombiePlayerState(m.Location));
            }

            m.MoveToWorld(EntranceLocations[Utility.Random(EntranceLocations.Length)], Facet);
        }

        public static bool ContainsPlayer(Mobile m)
        {
            return m.Map == Facet && ZombieRegionRectagle.Contains(m);
        }

		public override void OnEnter(Mobile m)
		{
            if (!(m is PlayerMobile))
                return;

            RemoveAllStatMods(m);

            m.BodyMod = 0;
            m.HueMod = -1;

            // Joining the Zombie Quest message.
            m.SendMessage("You are joining the Zombie Arena!  Type\"[zombie\" at any time to see the rankings.");

            m.RevealingAction();

            // Check to see if player is in Zombie Quest
            if (!Registry.ContainsKey(m))
            {
                Registry.Add(m, new ZombiePlayerState(m.Location));
            }
		}

        public override void OnExit(Mobile m)
        {
            //logged out
            if (m.Map == Map.Internal)
            {
                m.Kill();
                m.LogoutMap = Map.Felucca;
                m.LogoutLocation = SweetDreamsInn;
                Timer.DelayCall(TimeSpan.FromTicks(1), delegate { m.Resurrect(); m.Hits = m.HitsMax; m.Stam = m.StamMax; });
            }

            if (Registry.ContainsKey(m))
                Registry.Remove(m);

            RemoveAllStatMods(m);

            m.BodyMod = 0;
            m.HueMod = -1;

            base.OnExit(m);
        }

		// Prevent: Recall, Gate, Mark, Invisibility from being started
		public override bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			// Check to see if a player is trying to cast
			if (m.AccessLevel == AccessLevel.Player)
			{
				// Check for the banned spells
				if (s is RecallSpell || s is GateTravelSpell || s is MarkSpell || s is InvisibilitySpell || s is PolymorphSpell ||s is TeleportSpell)
				{
					// Do not allow spell and inform player
					m.SendMessage("You may not use that spell in the Zombie Arena.");
					return false;
				}
				return base.OnBeginSpellCast(m, s);
			}
			return base.OnBeginSpellCast(m, s);
		}

        public override bool CanUseStuckMenu(Mobile m)
        {
            return false;
        }

		// Prevent Hiding
		public override bool OnSkillUse(Mobile from, int Skill)
		{
			// Check to see if a player is trying to use a skill
			if (from.AccessLevel == AccessLevel.Player)
			{
				// Check for the banned skills
				if (Skill == 21) // Hiding
				{
					// Do not allow skill and inform player
					from.SendMessage("You may not hide in the Zombie Arena.");
					return false;
				}
			}
			return true;
		}

        public static bool IsZombieBody(Mobile m)
        {
            return m is PlayerMobile && (m.Body == 3 || m.Body == 83 || m.Body == 256);
        }

        public static bool IsAbomination(Mobile m)
        {
            return m is PlayerMobile && (m.Body == 256);
        }

		//Modify damage for high level zombies here
        public override bool OnDamage(Mobile m, ref int Damage)
        {
			// Get the attacker
			Mobile attacker = m.Combatant;

            if (Damage > 5 && IsAbomination(m) && attacker is PlayerMobile && BlackZombieBrains.CanSpawnZombie(m) && Utility.RandomDouble() < 0.05)
            {
                BlackZombieBrains.AddZombieSpawn(m);
                new Zombie().MoveToWorld(m.Location, m.Map);
                m.SendMessage("A zombie comes to your aid!");
            }
            else if (attacker != null && IsZombieBody(m)) // Check to see if attacker is a Zombie Player
            {
                ZombiePlayerState state;

                // Check to see if Zombie Player has a damage bonus
                if (Registry.TryGetValue(attacker, out state))
                {
                    int l_count = state.KillCount;
                    // Apply Damage bonus
                    if (l_count >= 61) { Damage = Convert.ToInt32(1.35 * Damage); }
                    else if (l_count >= 51 && l_count <= 60) { Damage = Convert.ToInt32(1.30 * Damage); }
                    else if (l_count >= 41 && l_count <= 50) { Damage = Convert.ToInt32(1.25 * Damage); }
                    else if (l_count >= 31 && l_count <= 40) { Damage = Convert.ToInt32(1.20 * Damage); }
                    else if (l_count >= 21 && l_count <= 30) { Damage = Convert.ToInt32(1.15 * Damage); }
                    else if (l_count >= 11 && l_count <= 20) { Damage = Convert.ToInt32(1.10 * Damage); }
                    else if (l_count >= 5 && l_count <= 100) { Damage = Convert.ToInt32(1.05 * Damage); }
                }
            }

            return base.OnDamage(m, ref Damage);
        }

		//Handle deaths from players here
		public override bool OnBeforeDeath(Mobile m)
		{
            if (!(m is PlayerMobile))
              return base.OnBeforeDeath(m);

			// Only humans spew brains
			spewBrains(m);

			Mobile killer = m.LastKiller;

			// Check to see if killer was a Zombie Player
			if ( killer != null && killer is PlayerMobile && IsZombieBody(killer) )
			{
				int killCount = 0;
                ZombiePlayerState zombieState;
                
				// Change the Zombie Player hue based on kills
                if (Registry.TryGetValue(killer, out zombieState))
				{
                    killer.SendMessage("You have slain an enemy!");
                    killCount = ++zombieState.KillCount;
					//		Zombie Hue Levels
					//
					//	Rank	Kills	Hue		Color
					//	1		0-4		0		None    
					//	2		5-10	920		Grey
					//	3		11-20	6		Blue
					//	4		21-30	21		Purple
					//	5		31-40	36		Red
					//	6		41-50	71		Green
					//	7		51-60	56		Yellow
					//	8		61+		1161	Fire
					//
					if (killCount >= 61) { killer.HueMod = 1161; }
					else if (killCount >= 51 && killCount <= 60) { killer.HueMod = 56; }
					else if (killCount >= 41 && killCount <= 50) { killer.HueMod = 71; }
					else if (killCount >= 31 && killCount <= 40) { killer.HueMod = 36; }
					else if (killCount >= 21 && killCount <= 30) { killer.HueMod = 21; }
					else if (killCount >= 11 && killCount <= 20) { killer.HueMod = 6; }
					else if (killCount >= 5 && killCount <= 100) { killer.HueMod = 920; }
				}
			}


            //during beta a ghost was in the arena? No idea how this happened.
            if (!Registry.ContainsKey(m))
            {
                Point3D point = new Point3D(1545, 1600, 20);
                m.MoveToWorld(point, Map.Felucca);
            }
            else
            {
                DateTime now = DateTime.UtcNow;
                // Calculate the duration player was alive in the Zombie Arena
                TimeSpan duration = now.Subtract((DateTime)Registry[m].Start);

                // Is a current Human killed?
                if (!(m.BodyValue == 3 || m.BodyValue == 83))
                {
                    AddRanking(m, duration);

                    // Play a death sequence?
                    int sound = m.GetDeathSound();

                    if (sound >= 0)
                        Effects.PlaySound(m, m.Map, sound);

                    m.SendMessage("You have become a zombie!");
                    m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                    m.PlaySound(0x1E1);


                    m.Warmode = false;

                    //Easiest way to have all monsters lose aggression on you
                    m.Hidden = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(1), delegate { m.RevealingAction(); });


                    //m.Send(DeathStatus.Instantiate(true));
                    //EventSink.InvokePlayerDeath(new PlayerDeathEventArgs(m));
                    //m.Send(DeathStatus.Instantiate(false));

                    // Make a zombie
                    // Adjust Str

                    RemoveAllStatMods(m);

                    int strAmount = 100 - m.Str;
                    if (strAmount == 0)
                    {
                        // Do nothing
                    }
                    else if (strAmount > 0)
                    {
                        // Positive Effect
                        SpellHelper.AddStatBonus(m, m, StatType.Str, strAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Strength, 1075845, TimeSpan.FromHours(24), m, strAmount));
                    }
                    else if (strAmount < 0)
                    {
                        // Negative Effect
                        SpellHelper.AddStatCurse(m, m, StatType.Str, -strAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Weaken, 1075845, TimeSpan.FromHours(24), m, -strAmount));
                    }

                    // Adjust Dex
                    int dexAmount = 30 - m.Dex;
                    if (dexAmount == 0)
                    {
                        // Do nothing
                    }
                    else if (dexAmount > 0)
                    {
                        // Positive Effect
                        SpellHelper.AddStatBonus(m, m, StatType.Dex, dexAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Agility, 1075845, TimeSpan.FromHours(24), m, dexAmount));
                    }
                    else if (dexAmount < 0)
                    {
                        // Negative Effect
                        SpellHelper.AddStatCurse(m, m, StatType.Dex, -dexAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Clumsy, 1075845, TimeSpan.FromHours(24), m, -dexAmount));
                    }

                    // Adjust Int
                    int intAmount = 30 - m.Int;
                    if (intAmount == 0)
                    {
                        // Do nothing
                    }
                    else if (intAmount > 0)
                    {
                        // Positive Effect
                        SpellHelper.AddStatBonus(m, m, StatType.Int, intAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Cunning, 1075845, TimeSpan.FromHours(24), m, intAmount));
                    }
                    else if (intAmount < 0)
                    {
                        // Negative Effect
                        SpellHelper.AddStatCurse(m, m, StatType.Int, -intAmount, TimeSpan.FromHours(24));
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.FeebleMind, 1075845, TimeSpan.FromHours(24), m, -intAmount));
                    }
                    m.HueMod = 0;
                    m.BodyMod = 3;

                    // Start the decay
                    decayZombie((PlayerMobile)m);

                    m.Hits = m.HitsMax;
                    m.Stam = m.StamMax;
                    m.Mana = m.ManaMax;

                    // Format the time displayed
                    string l_time = string.Format("{0:00}:{1:00}:{2:00}", duration.TotalHours, duration.Minutes, duration.Seconds);

                    m.SendMessage("You lasted " + l_time + " in the Zombie Arena and have now been turned into a Zombie!");

                    return false;
                }
                // Zombie just got killed
                else
                {
                    RemoveAllStatMods(m);

                    var state = Registry[m];
                    // Coordinates of the Return Location in Brit
                    Point3D point = state.ReturnLocation == Point3D.Zero ? SweetDreamsInn : state.ReturnLocation;

                    // Leaving the Zombie Quest message.
                    m.SendMessage("You have left the Zombie Arena.");

                    // Remove the player from the HashTable
                    Registry.Remove(m);

                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate 
                    { 
                        m.MoveToWorld(point, Map.Felucca);  
                        m.Resurrect();
                        m.Hits = m.HitsMax;
                        m.Stam = m.StamMax;
                    });
                }
            }

            return base.OnBeforeDeath(m);
		}

        public static void RemoveAllStatMods(Mobile m)
        {
            if (m.AccessLevel == AccessLevel.Player)
            {
                StatMod mod;

                mod = m.GetStatMod("[Magic] Str Offset");
                if (mod != null)
                    m.RemoveStatMod("[Magic] Str Offset");

                mod = m.GetStatMod("[Magic] Dex Offset");
                if (mod != null)
                    m.RemoveStatMod("[Magic] Dex Offset");

                mod = m.GetStatMod("[Magic] Int Offset");
                if (mod != null)
                    m.RemoveStatMod("[Magic] Int Offset");

                BuffInfo.RemoveBuff(m, BuffIcon.Strength);
                BuffInfo.RemoveBuff(m, BuffIcon.Weaken);
                BuffInfo.RemoveBuff(m, BuffIcon.Agility);
                BuffInfo.RemoveBuff(m, BuffIcon.Clumsy);
                BuffInfo.RemoveBuff(m, BuffIcon.Cunning);
                BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);

                m.Poison = null;

                m.Blessed = false;
            }
        }

        public static void AddRanking(Mobile m, TimeSpan duration)
        {
            // Does the Player already have a Duration?
            Rank found = TopTen.Find(delegate(Rank rank) { return rank.Player == m; });

            if (found != null)
            {
                if (duration > found.Duration)
                {
                    TopTen.Remove(found);
                }
                else
                {
                    return;
                }
            }

            if (TopTen.Count < 10 || duration > TopTen[TopTen.Count - 1].Duration)
            {
                bool added = false;

                for (int i = 0; i < TopTen.Count; i++)
                {
                    if (duration > TopTen[i].Duration)
                    {
                        TopTen.Insert(i, new Rank(m, duration));
                        added = true;
                        break;
                    }
                }

                if (!added)
                    TopTen.Add(new Rank(m, duration));

                if (TopTen.Count > 10)
                    TopTen.RemoveAt(10);
            }
        }

		// Determine the Zombie Brains to create
		public void spewBrains(Mobile m)
		{
			// How many brains to create (between 1 to 3)
			int numberOfBrains = Utility.Random(3) + 1;

			for (int i = 0; i < numberOfBrains; i++)
			{
				int chance = Utility.Random(100);

				// The chance of getting a type of brain
				if (chance >= 97) { GoldZombieBrains b = new GoldZombieBrains(); sendBrains(m,b); } // 3%
				else if (chance >= 91 && chance <= 96) { RedZombieBrains b = new RedZombieBrains(); sendBrains(m,b); } // 3%
				else if (chance >= 88 && chance <= 90) { BlackZombieBrains b = new BlackZombieBrains(); sendBrains(m,b); } // 3%
				else if (chance >= 78 && chance <= 87) { SilverZombieBrains b = new SilverZombieBrains(); sendBrains(m,b); } // 10%
				else if (chance >= 40 && chance <= 77) { BlueZombieBrains b = new BlueZombieBrains(); sendBrains(m,b); } // 40%
				else if (chance >= 00 && chance <= 37) { YellowZombieBrains b = new YellowZombieBrains(); sendBrains(m,b); } // 41%
			}
		}

		// Create the brains and play an effect (Currently Explosion)
		public void sendBrains(Mobile m, BaseZombieBrains b)
		{
            b.Owner = m;
			b.MoveToWorld(new Point3D(m.X, m.Y, m.Z), m.Map);
			Effects.SendLocationParticles(EffectItem.Create(b.Location, b.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
		}

		// Do damage to Zombie Players every 5 Seconds
		public void decayZombie(PlayerMobile m)
		{
			Timer.DelayCall(TimeSpan.FromSeconds(6), new TimerStateCallback(decayZombieDamage), m);
		}

		// Handle Zombie Player Damage
		private static void decayZombieDamage(object state)
		{
			PlayerMobile pm = ((Mobile)state) as PlayerMobile;
			// Only do this in ZombieQuest Region
			if (!pm.Region.IsPartOf(typeof(ZombieQuestRegion)))
			{
				return;
			}
			// Check to see if it would kill Player Zombie
			if (pm.Hits <= 2)
			{
				// Added additional damage to insure death
				pm.Damage(5);
			}
			else
			{
				// Do 5 damage and recursivly call
				pm.Damage(2);
                pm.LocalOverheadMessage(MessageType.Emote, 0x0, true, "*You are bleeding*");
				Timer.DelayCall(TimeSpan.FromSeconds(6), new TimerStateCallback(decayZombieDamage), pm);
			}
		}



        public static void End()
        {
            Queue trammelQueue = new Queue(100);

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (!(m is PlayerMobile))
                    continue;

                if (m.Map != Map.Trammel && m.LogoutMap != Map.Trammel)
                    continue;

                trammelQueue.Enqueue(m);
            }

            Mobile toMove;
            while (trammelQueue.Count > 0)
            {
                toMove = trammelQueue.Dequeue() as Mobile;

                if (toMove != null)
                {
                    RemoveAllStatMods(toMove);

                    if (toMove.Map == Map.Internal)
                        toMove.LogoutMap = Map.Felucca;
                    else
                        toMove.Map = Map.Felucca;
                }
            }
        }

        public static bool Saved = false;
        public static void Serialize(GenericWriter writer)
        {
            Saved = true;
            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate { Saved = false; });

            writer.WriteEncodedInt(0);

            writer.Write(TopTen.Count);
            foreach (Rank r in TopTen)
            {
                writer.Write(r.Player);
                writer.Write(r.Duration);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Mobile m = reader.ReadMobile();
                            TimeSpan duration = reader.ReadTimeSpan();

                            if (m != null)
                                AddRanking(m, duration);
                        }
                        break;
                    }

            }
        }
	}
}