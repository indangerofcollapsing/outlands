using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;
using Server;
using Server.Misc;
using Server.Mobiles;

namespace Server.ArenaSystem
{
	public class ArenaSpectator : BaseCreature
	{
		public static List<ArenaSpectator> AllSpectators = new List<ArenaSpectator>();

		static string[] OnMatchStartShouts = new string[]{
			"Look everyone, it is {0}!",
			"{0}, I LOVE THEE!",
			"My money is on {0}!",
			"LOOK IT IS {0}",
			"Nice outfit {0}",
			"{0} looks sharp today",
			"{0}! {0}! {0}!",
			"{0}! {0}! {0}!",
			"wohooo",
			"oh yeah, fight!!"
		};

		static string[] OnMatchEndShoutWinner = new string[]{
			"Great fight {0}!",
			"{0} really nailed that one",
			"{0} should be doing well this arena season",
			"GREAT FIGHT {0}",
			"{0}! {0}! {0}!",
			"You are the best {0}!",
			"NICE!",
			"told you so guys",
			"one more {0}!"
		};

		static string[] OnMatchEndShoutLoser = new string[]{
			"Get back in there {0}",
			"GUARDS! Rigged match, {0} is the superior fighter",
			"Come on {0}, you can do better",
			"REMATCH!",
			"GIVE US A REMATCH!",
			"one more guys, come on"
		};

		static string[] OnMatchDuringShouts = new string[]{
			"KILL HIM",
			"KILL HIM",
			"FINISH HIM {0}",
			"FINISH HIM {0}",
			"ZzzZZzzz",
			"ZzzZZzzz",
			"looking good {0}",
			"looking good {0}",
			"{0} will lose this fight, just watch",
			"{0} will lose this fight, just watch",
			"There is no way {0} will lose this fight",
			"{0} will win",
			"{0} will win",
			"I'm hungry",
			"Where art thou from?",
			"I must consider my sins",
			"can you heal through poison?",
			"Nobody wants to buy my fish steaks anymore...",
			"Why is this taking so long?",
			"My brother saw a murderer outside shame yesterday",
			"Who's up next?",
			"ever been to fire island?",
			"Did anyone see that crazy fight earlier today?",
			"I wish I was a treasure hunter",
			"come on...",
			"nice move {0}",
			"Nice reflexes {0}!"
		};

		public void DoMatchStartShout(string contestant)
		{
			Say(String.Format(OnMatchStartShouts[Utility.Random(OnMatchStartShouts.Length)], contestant));
		}

		public void DoMatchDuringShout(string contestant)
		{
			Say(String.Format(OnMatchDuringShouts[Utility.Random(OnMatchDuringShouts.Length)], contestant));
		}

		public void DoMatchEndShoutWinner(string contestant)
		{
			Say(String.Format(OnMatchEndShoutWinner[Utility.Random(OnMatchEndShoutWinner.Length)], contestant));
		}

		public void DoMatchEndShoutLoser(string contestant)
		{
			Say(String.Format(OnMatchEndShoutLoser[Utility.Random(OnMatchEndShoutLoser.Length)], contestant));
		}

		public static void DoPostMatchShouts(Point3D origin, string winner, string loser)
		{
			List<ArenaSpectator> spectators = ArenaSpectator.GetNearbySpectators(origin);
			if (spectators.Count == 0)
				return;
			spectators[Utility.Random(spectators.Count)].DoMatchEndShoutWinner(winner);
			spectators[Utility.Random(spectators.Count)].DoMatchEndShoutLoser(loser);
			if (Utility.RandomBool())
				spectators[Utility.Random(spectators.Count)].DoMatchEndShoutWinner(winner);
			else
				spectators[Utility.Random(spectators.Count)].DoMatchEndShoutLoser(loser);
		}

		public static void DoPreMatchShouts(Point3D origin, string t1, string t2)
		{
			List<ArenaSpectator> spectators = ArenaSpectator.GetNearbySpectators(origin);
			if (spectators.Count == 0)
				return;

			spectators[Utility.Random(spectators.Count)].DoMatchStartShout(t1);
			spectators[Utility.Random(spectators.Count)].DoMatchStartShout(t2);
			if (Utility.RandomBool())
				spectators[Utility.Random(spectators.Count)].DoMatchStartShout(t1);
			else
				spectators[Utility.Random(spectators.Count)].DoMatchStartShout(t2);
		}

		public static void DoDuringMatchShout(Point3D origin, string t)
		{
			List<ArenaSpectator> spectators = ArenaSpectator.GetNearbySpectators(origin);
			if (spectators.Count == 0)
				return;

			spectators[Utility.Random(spectators.Count)].DoMatchDuringShout(t);
		}
		
		private static List<ArenaSpectator> GetNearbySpectators(Point3D origin)
		{
			double sq_max = 226; // 15 units + rounding error margin

			List<ArenaSpectator> result = new List<ArenaSpectator>();
			foreach (ArenaSpectator spec in AllSpectators)
			{
				double sqdist = Math.Pow(origin.X - spec.Location.X, 2) + Math.Pow(origin.Y - spec.Location.Y, 2);
				if (sq_max >= sqdist)
				{
					result.Add(spec);
				}
			}
			return result;
		}


		[Constructable]
		public ArenaSpectator()
			: base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
		{
			AccessLevel = AccessLevel.Player;
			InitStats(31, 41, 51);

			InitBody();
			InitOutfit();

			Title = "the spectator";

			Blessed = true;
			Frozen = false;
			CantWalk = true;

			AllSpectators.Add(this);
		}

		public ArenaSpectator(Serial s)
			: base(s)
		{
		}

		public void InitOutfit()
		{
			switch (Utility.Random(10))
			{
				case 0:
					AddItem(Immovable(Rehued(new BodySash(), 1645)));
					AddItem(Immovable(Rehued(new Kilt(), 1645)));
					AddItem(Immovable(Rehued(new Sandals(), 1645)));
					AddItem(Newbied(new DoubleAxe()));
					break;
				case 1:
					AddItem(new StuddedChest());
					AddItem(new StuddedLegs());
					AddItem(new StuddedArms());
					AddItem(new StuddedGloves());
					AddItem(new StuddedGorget());
					AddItem(new Boots());
					AddItem(Newbied(new Spear()));
					break;
				case 2:
					AddItem(Immovable(Rehued(new ChainChest(), 2125)));
					AddItem(Immovable(Rehued(new ChainLegs(), 2125)));
					AddItem(Immovable(Rehued(new ChainCoif(), 2125)));
					AddItem(Immovable(Rehued(new PlateArms(), 2125)));
					AddItem(Immovable(Rehued(new PlateGloves(), 2125)));

					AddItem(Immovable(Rehued(new BodySash(), 1254)));
					AddItem(Immovable(Rehued(new Kilt(), 1254)));
					AddItem(Immovable(Rehued(new Sandals(), 1254)));
					break;
				case 3:
					if (Utility.RandomBool())
						AddItem(Immovable(Rehued(new WizardsHat(), 1325)));
					AddItem(Immovable(Rehued(new Sandals(), 1325)));
					AddItem(Immovable(Rehued(new LeatherGorget(), 1325)));
					AddItem(Immovable(Rehued(new LeatherGloves(), 1325)));
					AddItem(Immovable(Rehued(new LeatherLegs(), 1325)));
					AddItem(Immovable(Rehued(new Skirt(), 1325)));
					AddItem(Immovable(Rehued(new FemaleLeatherChest(), 1325)));
					AddItem(Newbied(Rehued(new Halberd(), 1310)));
					break;
				case 4:
					AddItem(new ChainChest());
					AddItem(new ChainLegs());
					AddItem(new RingmailArms());
					AddItem(new RingmailGloves());
					AddItem(new ChainCoif());
					AddItem(new Boots());
					AddItem(Newbied(new ShortSpear()));
					break;
				default:
					// "civilian"
					AssignRandomCivilianClothing();
					break;
			}
		}

		public override bool IsInvulnerable { get { return true; } }


		public override void OnDelete()
		{
			AllSpectators.Remove(this);
		}

		public void InitBody()
		{
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if (Female = Utility.RandomBool())
			{
				Body = 0x191;
				Name = NameList.RandomName("female");
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName("male");
			}
		}

		public Item Immovable(Item item)
		{
			item.Movable = false;
			return item;
		}

		public Item Newbied(Item item)
		{
			item.LootType = LootType.Newbied;
			return item;
		}

		public Item Rehued(Item item, int hue)
		{
			item.Hue = hue;
			return item;
		}

		public void AssignRandomCivilianClothing()
		{
			switch (Utility.Random(3))
			{
				case 0: AddItem(new FancyShirt(GetRandomHue())); break;
				case 1: AddItem(new Doublet(GetRandomHue())); break;
				case 2: AddItem(new Shirt(GetRandomHue())); break;
			}

			switch (Utility.Random(4))
			{
				case 0: AddItem(new Shoes(GetShoeHue())); break;
				case 1: AddItem(new Boots(GetShoeHue())); break;
				case 2: AddItem(new Sandals(GetShoeHue())); break;
				case 3: AddItem(new ThighBoots(GetShoeHue())); break;
			}

			int hairHue = Utility.RandomHairHue();

			Utility.AssignRandomHair(this, hairHue);
			Utility.AssignRandomFacialHair(this, hairHue);

			if (Utility.RandomBool())
				AddItem(Loot.RandomHat());

			if (Female)
			{
				switch (Utility.Random(6))
				{
					case 0: AddItem(new ShortPants(GetRandomHue())); break;
					case 1:
					case 2: AddItem(new Kilt(GetRandomHue())); break;
					case 3:
					case 4:
					case 5: AddItem(new Skirt(GetRandomHue())); break;
				}
			}
			else
			{
				switch (Utility.Random(2))
				{
					case 0: AddItem(new LongPants(GetRandomHue())); break;
					case 1: AddItem(new ShortPants(GetRandomHue())); break;
				}
			}
		}
		public virtual int GetRandomHue()
		{
			switch (Utility.Random(5))
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if (Utility.RandomBool())
				return Utility.RandomNeutralHue();
			else
				return Utility.RandomMetalHue();
		}

		public override bool HandlesOnSpeech(Mobile from)
		{
			return from is ArenaAnnouncer;
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			base.OnSpeech(e);
		}

		public override bool CanTeach { get { return false; } }
		public override bool ClickTitle { get { return false; } }


		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version 
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			AllSpectators.Add(this);
		}
	}
}
