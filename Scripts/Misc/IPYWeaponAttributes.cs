using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;

namespace Server
{
    public enum IPYWeaponAttribute
    {
        None,
        HitPoison,
        HitFireball,
        HitHarm,
        HitMagicArrow,
        HitLightning,
        HitCurse,
        HitParalyze,
        HitManaDrain,
        AreaPoison,
        AreaFireball,
        AreaHarm,
        AreaMagicArrow,
        AreaLightning,
        AreaCurse,
        AreaParalyze,
        AreaManaDrain
    }

    public class IPYWeaponAttributes
    {
        public IPYWeaponAttribute Attribute { get; set; }

        public int Proc { get; set; }

        public static string GetIPYWeaponAttributeName(BaseWeapon weapon)
        {
            var attr = weapon.IPYWeaponAttributes;
            return attr == null ? String.Empty : attr.GetAttributeName();
        }

        public string GetAttributeName()
        {
            switch (this.Attribute)
            {
                case IPYWeaponAttribute.HitFireball: return "Daemon's Breath";
                case IPYWeaponAttribute.HitHarm: return "Wounding";
                case IPYWeaponAttribute.HitLightning: return "Thunder";
                case IPYWeaponAttribute.HitMagicArrow: return "Burning";
                case IPYWeaponAttribute.HitCurse: return "Evil";
                case IPYWeaponAttribute.HitPoison: return "Corruption";
                case IPYWeaponAttribute.HitParalyze: return "Ghoul's Touch";
                case IPYWeaponAttribute.HitManaDrain: return "Mage's Bane";

                case IPYWeaponAttribute.AreaFireball: return "Dragon's Breath";
                case IPYWeaponAttribute.AreaHarm: return "Torture";
                case IPYWeaponAttribute.AreaLightning: return "Mana Storm";
                case IPYWeaponAttribute.AreaMagicArrow: return "Faerie Fire";
                case IPYWeaponAttribute.AreaCurse: return "Breath of the Dead";
                case IPYWeaponAttribute.AreaPoison: return "Wyvern's Breath";
                case IPYWeaponAttribute.AreaParalyze: return "Lich's Grasp";
                case IPYWeaponAttribute.AreaManaDrain: return "Cyclone of Torment";
                default: return String.Empty;
            }
        }

        public IPYWeaponAttributes(IPYWeaponAttribute attr, int proc)
        {
            Attribute = attr;
            Proc = proc;
        }

        public static bool IsActive(BaseWeapon weapon)
        {
            return weapon.IPYWeaponAttributes != null;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); //version

            writer.Write((int)Attribute);
            writer.Write((int)Proc);
        }

        public static IPYWeaponAttributes Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            var ipyattr = (IPYWeaponAttribute)reader.ReadInt();
            var proc = reader.ReadInt();
            IPYWeaponAttributes attr = new IPYWeaponAttributes(ipyattr, proc);

            return attr;
        }
    }
}
