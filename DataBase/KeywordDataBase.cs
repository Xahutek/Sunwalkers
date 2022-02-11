using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/KeywordDataBase")]
public class KeywordDataBase : ScriptableObject
{
    public Sprite
        NoIcon,
        RockThrowerIcon,
        RangerIcon,
        BallistaIcon,
        LancerIcon,
        AlchemistIcon,
        ShamanIcon,
        MenderIcon,
        GuardianIcon,
        NomadIcon;

    public Color
        NoColor,
        RockThrowerColor,
        RangerColor,
        BallistaColor,
        LancerColor,
        AlchemistColor,
        ShamanColor,
        MenderColor,
        GuardianColor,
        NomadColor;

    public static State GetAttackState(Keyword keyword)
    {
        switch (keyword)
        {
            case Keyword.Ranger:
                return new ArrowAttackState();
            case Keyword.Ballista:
                return new ArrowAttackState();
            case Keyword.Lancer:
                return new AreaAttackState();
            case Keyword.Alchemist:
                return new SplashAttackState();
            case Keyword.Shaman:
                return new AreaAttackState();
            default:
                return new ArrowAttackState();
        }
    }

    public Sprite GetIcon(Keyword keyword)
    {
        switch (keyword)
        {
            case Keyword.Ranger:
                return RangerIcon;
            case Keyword.Ballista:
                return BallistaIcon;
            case Keyword.Lancer:
                return LancerIcon;
            case Keyword.Alchemist:
                return AlchemistIcon;
            case Keyword.Shaman:
                return ShamanIcon;
            case Keyword.Mender:
                return MenderIcon;
            case Keyword.Guardian:
                return GuardianIcon;
            case Keyword.Nomad:
                return NomadIcon;
            case Keyword.RockThrower:
                return RockThrowerIcon;
            default:
                return NoIcon;
        }
    }
    public Color GetColor(Keyword keyword)
    {
        switch (keyword)
        {
            case Keyword.Ranger:
                return RangerColor;
            case Keyword.Ballista:
                return BallistaColor;
            case Keyword.Lancer:
                return LancerColor;
            case Keyword.Alchemist:
                return AlchemistColor;
            case Keyword.Shaman:
                return ShamanColor;
            case Keyword.Mender:
                return MenderColor;
            case Keyword.Guardian:
                return GuardianColor;
            case Keyword.Nomad:
                return NomadColor;
            case Keyword.RockThrower:
                return RockThrowerColor;
            default:
                return NoColor;
        }
    }
}
public enum Keyword
{
    Ranger, Ballista, Lancer, Alchemist, Shaman,
    Mender, Guardian, Nomad, Trapper, RockThrower
}