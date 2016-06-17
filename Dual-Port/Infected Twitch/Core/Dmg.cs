﻿#region

using EloBuddy;
using LeagueSharp.SDK;

#endregion

namespace Infected_Twitch.Core
{
    internal class Dmg
    {
        public static int IgniteDmg = 50 + 20 * GameObjects.Player.Level;

        public static float Damage(Obj_AI_Base target)
        {
            if (target == null) return 0;

            float dmg = 0;
            if (Spells.W.IsReady()) dmg = dmg + Spells.W.GetDamage(target);
            if (Spells.E.IsReady()) dmg = dmg + (float)EDamage(target);

            if (Spells.Ignite != SpellSlot.Unknown && GameObjects.Player.Spellbook.CanUseSpell(Spells.Ignite) == SpellState.Ready)
            {
                dmg = dmg + IgniteDmg;
            }


            dmg = dmg + (float)GameObjects.Player.GetAutoAttackDamage(target);

            return dmg;
        }

        public static float EDamage(Obj_AI_Base target)
        {
            if (target == null || !target.LSIsValidTarget(1200) || !target.HasBuff("TwitchDeadlyVenom")) return 0;
            if (target.IsInvulnerable || target.HasBuff("KindredRNoDeathBuff") || target.HasBuffOfType(BuffType.SpellShield)) return 0;

            float eDmg = 0;

            if (Spells.E.IsReady())
            {
                eDmg = eDmg + Spells.E.GetDamage(target);
                eDmg = eDmg + (float)GameObjects.Player.CalculateDamage(target, DamageType.True, Passive(target) * GameObjects.Player.FlatMagicDamageMod + GameObjects.Player.FlatPhysicalDamageMod);
            }
            

            if (GameObjects.Player.HasBuff("SummonerExhaust")) eDmg = eDmg *= (float) 0.6;

            return eDmg;
        }

        public static float Passive(Obj_AI_Base target)
        {
            float dmg = 6;

            if (GameObjects.Player.Level > 16) dmg = 6;
            if (GameObjects.Player.Level > 12) dmg = 5;
            if (GameObjects.Player.Level > 8) dmg = 4;
            if (GameObjects.Player.Level > 4) dmg = 3;
            
            return dmg * Stacks(target) - target.HPRegenRate;
        }

        public static float Stacks(Obj_AI_Base target)
        {
            return target.GetBuffCount("TwitchDeadlyVenom");
        }

        public static bool Executable(Obj_AI_Base target)
        {
            return target.Health < EDamage(target);
        }

        public static bool Lethal(Obj_AI_Base target)
        {
            return Damage(target) / 1.70 >= target.Health;
        }
    }
}
