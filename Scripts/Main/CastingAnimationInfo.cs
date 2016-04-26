using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server
{
	public class CastingAnimationInfo
	{  
        
        public CastingAnimationInfo()
        {
        }

        public static void GetCastAnimationForBody(BaseCreature target)
        {
            if (!(target is BaseCreature))
               return;

            //High Seas Creature Animation Override
            if (target is BaseCreature)
            {
                BaseCreature bc_Target = target as BaseCreature;

                if (bc_Target.IsHighSeasBodyType)
                {
                    target.SpellCastAnimation = 27;
                    target.SpellCastFrameCount = 5;
                }
            }

            //Vampire Thrall
            if (target.Body == 722)
            {
                target.SpellCastAnimation = 5;
                target.SpellCastFrameCount = 8;
            }

            //Ratman Mage
            if (target.Body == 0x8F)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Arcane Daemon
            if (target.Body == 0x310)
            {
                target.SpellCastAnimation = 13;
                target.SpellCastFrameCount = 7;
            }

            //Ruby Dragon
            if (target.Body == 0x3B)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Swamp Dragons
            if (target.Body == 0x31F || target.Body == 0x31A)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Chaos Dragon
            if (target.Body == 0xC)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Gargoyle
            if (target.Body == 0x4)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 7;
            }

            //Daemon
            if (target.Body == 0x9)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 7;
            }

            //Dragon
            if (target.Body == 0xC || target.Body == 0x3B)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Air Elemental
            if (target.Body == 0xD)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }

            //Fire Elemental
            if (target.Body == 0xF)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }

            //Water Elemental
            if (target.Body == 0x10)
            {
                target.SpellCastAnimation = 3;
                target.SpellCastFrameCount = 4;
            }

            //Gazer
            if (target.Body == 0x16)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }

            //Gazer Larva
            if (target.Body == 778)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }

            //Lich
            if (target.Body == 0x18)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 7;
            }

            //Spectre
            if (target.Body == 0x1A)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }

            //Balron
            if (target.Body == 0x28)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 7;
            }

            //Ancient Wyrm
            if (target.Body == 0x2E)
            {
                target.SpellCastAnimation = 5;
                target.SpellCastFrameCount = 5;
            }

            //Reaper
            if (target.Body == 0x2F)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //White Wyrm
            if (target.Body == 0x31)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Wisp
            if (target.Body == 0x3A)
            {
                target.SpellCastAnimation = 3;
                target.SpellCastFrameCount = 4;
            }

            //Terathan Matriarch
            if (target.Body == 0x48)
            {
                target.SpellCastAnimation = 14;
                target.SpellCastFrameCount = 5;
            }

            //Imp
            if (target.Body == 74)
            {
                target.SpellCastAnimation = 0;
                target.SpellCastFrameCount = 10;
            }

            //Titan
            if (target.Body == 0x4C)
            {
                target.SpellCastAnimation = 18;
                target.SpellCastFrameCount = 5;
            }

            //LichLord
            if (target.Body == 0x4F)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 5;
            }

            //Ophidian Archmage
            if (target.Body == 0x55)
            {
                target.SpellCastAnimation = 13;
                target.SpellCastFrameCount = 5;
            }

            //Ophidian Matriarch
            if (target.Body == 0x57)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Fierce Dragon
            if (target.Body == 0x67)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Skeletal Dragon
            if (target.Body == 0x68)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Shadow Wyrm
            if (target.Body == 0x6A)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Efreet
            if (target.Body == 0x83)
            {
                target.SpellCastAnimation = 2;
                target.SpellCastFrameCount = 4;
            }

            //Orcish Mage
            if (target.Body == 0x8C)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Bone Magi
            if (target.Body == 0x94)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Terathan Avenger
            if (target.Body == 0x98)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }                    

            //Abyss Dragon
            if (target.Body == 0x96)
            {
                target.SpellCastAnimation = 11;
                target.SpellCastFrameCount = 5;
            }

            //Bogle
            if (target.Body == 0x99)
            {
                target.SpellCastAnimation = 6;
                target.SpellCastFrameCount = 4;
            }    

            //Blood Elemental
            if (target.Body == 0x9F)
            {
                target.SpellCastAnimation = 3;
                target.SpellCastFrameCount = 6;
            }

            //Poison Elemental
            if (target.Body == 0xA2)
            {
                target.SpellCastAnimation = 3;
                target.SpellCastFrameCount = 4;
            }

            //Chaos Daemon
            if (target.Body == 0x318)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 7;
            }

            //sanguinous
            if (target.Body == 741)
            {
                target.SpellCastAnimation = 12;
                target.SpellCastFrameCount = 15;
            } 

            return;
        }      
    }
}