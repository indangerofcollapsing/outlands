using System;
using System.Collections.Generic;
using Server.Mobiles;

using Server.Spells;

namespace Server.Items
{
    public class BlackZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a black brain"; } }
        public static Dictionary<Mobile, int> ZombieCounts = new Dictionary<Mobile, int>();

        [Constructable]
        public BlackZombieBrains()
            : base()
        {
            Hue = 1908;
        }

        public static void ResetZombieCounts(Mobile m)
        {
            ZombieCounts.Remove(m);
        }

        public static void AddZombieSpawn(Mobile m)
        {
            if (ZombieCounts.ContainsKey(m))
            {
                ZombieCounts[m]++;
            }
            else
            {
                ZombieCounts.Add(m, 1);
            }
        }

        public static bool CanSpawnZombie(Mobile m)
        {
            return !ZombieCounts.ContainsKey(m) || ZombieCounts[m] < 21;
        }

        public override void Eat(Mobile from)
        {
            if (from.BeginAction(typeof(BlackZombieBrains)))
			{
                ResetZombieCounts(from);

				from.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
				from.PlaySound(0x03C);
				from.SendMessage("You devour the blackish brains and begin to feel your flesh ripping as it stretches.");
				
				// Abomination form 
				from.BodyMod = 256;

				// Adjust Str
				int strAmount = 850 - from.Str;
				SpellHelper.AddStatBonus(from, from, StatType.Str, strAmount, TimeSpan.FromMinutes(20));
				BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Strength, 1075845, TimeSpan.FromMinutes(20), from, strAmount));

				// Adjust Dex
				int dexAmount = 70 - from.Dex;
				if (dexAmount == 0)
				{
					// Do nothing
				}
				else if (dexAmount > 0)
				{
					// Positive Effect
					SpellHelper.AddStatBonus(from, from, StatType.Dex, dexAmount, TimeSpan.FromMinutes(20));
					BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Agility, 1075845, TimeSpan.FromMinutes(20), from, dexAmount));
				}
				else if (dexAmount < 0)
				{
					// Negative Effect
					SpellHelper.AddStatCurse(from, from, StatType.Dex, -dexAmount, TimeSpan.FromMinutes(20));
					BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Clumsy, 1075845, TimeSpan.FromMinutes(20), from, -dexAmount));
				}

				// Adjust Int
				int intAmount = 70 - from.Int;
				if (intAmount == 0)
				{
					// Do nothing
				}
				else if (intAmount > 0)
				{
					// Positive Effect
					SpellHelper.AddStatBonus(from, from, StatType.Int, intAmount, TimeSpan.FromMinutes(20));
					BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Cunning, 1075845, TimeSpan.FromMinutes(20), from, intAmount));
				}
				else if (intAmount < 0)
				{
					// Negative Effect
					SpellHelper.AddStatCurse(from, from, StatType.Int, -intAmount, TimeSpan.FromMinutes(20));
					BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.FeebleMind, 1075845, TimeSpan.FromMinutes(20), from, -intAmount));
				}
				from.Hits = 500; // Doesn't work for player access (overwritten by Str calc) 
                Timer.DelayCall(TimeSpan.FromMinutes(15), delegate
                {
                    from.EndAction(typeof(BlackZombieBrains));

                    if (from.BodyMod == 256)
                        from.BodyMod = 3;
                });
				Delete();
			}
			else
			{
				from.SendMessage("You can't eat another one of these so soon!");
			}
        }

        public BlackZombieBrains(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
