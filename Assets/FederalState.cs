using System;
using System.Runtime.Serialization;

public enum FederalState
{
    [EnumMember(Value = "Schleswig-Holstein")]
    SchleswigHolstein,
    [EnumMember(Value = "Hamburg")] Hamburg,
    [EnumMember(Value = "Niedersachsen")] Niedersachsen,
    [EnumMember(Value = "Bremen")] Bremen,

    [EnumMember(Value = "Nordrhein-Westfalen")]
    NordrheinWestfalen,
    [EnumMember(Value = "Hessen")] Hessen,

    [EnumMember(Value = "Rheinland-Pfalz")]
    RheinlandPfalz,

    [EnumMember(Value = "Baden-Württemberg")]
    BadenWürttemberg,
    [EnumMember(Value = "Bayern")] Bayern,
    [EnumMember(Value = "Saarland")] Saarland,
    [EnumMember(Value = "Berlin")] Berlin,
    [EnumMember(Value = "Brandenburg")] Brandenburg,

    [EnumMember(Value = "Mecklenburg-Vorpommern")]
    MecklenburgVorpommern,

    [EnumMember(Value = "Sachsen")] Sachsen,
    [EnumMember(Value = "Sachsen-Anhalt")] SachsenAnhalt,
    [EnumMember(Value = "Thüringen")] Thüringen,
    [EnumMember(Value = "Insgesamt")] Insgesamt,
}

static class FederalStateParser
{
    public static FederalState fromString(string name)
    {
        switch (name)
        {
            case "Schleswig-Holstein": return FederalState.SchleswigHolstein;
            case "Hamburg": return FederalState.Hamburg;
            case "Niedersachsen": return FederalState.Niedersachsen;
            case "Bremen": return FederalState.Bremen;
            case "Nordrhein-Westfalen": return FederalState.NordrheinWestfalen;
            case "Hessen": return FederalState.Hessen;
            case "Rheinland-Pfalz": return FederalState.RheinlandPfalz;
            case "Baden-Wuerttemberg": return FederalState.BadenWürttemberg;
            case "Bayern": return FederalState.Bayern;
            case "Saarland": return FederalState.Saarland;
            case "Berlin": return FederalState.Berlin;
            case "Brandenburg": return FederalState.Brandenburg;
            case "Mecklenburg-Vorpommern": return FederalState.MecklenburgVorpommern;
            case "Sachsen": return FederalState.Sachsen;
            case "Sachsen-Anhalt": return FederalState.SachsenAnhalt;
            case "Thueringen": return FederalState.Thüringen;
            case "Insgesamt": return FederalState.Insgesamt;
        }

        throw new Exception($"unknown state: {name}");
    }

    public static string toString(this FederalState target)
    {
        switch (target)
        {
            case FederalState.SchleswigHolstein: return "Schleswig-Holstein";
            case FederalState.Hamburg: return "Hamburg";
            case FederalState.Niedersachsen: return "Niedersachsen";
            case FederalState.Bremen: return "Bremen";
            case FederalState.NordrheinWestfalen: return "Nordrhein-Westfalen";
            case FederalState.Hessen: return "Hessen";
            case FederalState.RheinlandPfalz: return "Rheinland-Pfalz";
            case FederalState.BadenWürttemberg: return "Baden-Wuerttemberg";
            case FederalState.Bayern: return "Bayern";
            case FederalState.Saarland: return "Saarland";
            case FederalState.Berlin: return "Berlin";
            case FederalState.Brandenburg: return "Brandenburg";
            case FederalState.MecklenburgVorpommern: return "Mecklenburg-Vorpommern";
            case FederalState.Sachsen: return "Sachsen";
            case FederalState.SachsenAnhalt: return "Sachsen-Anhalt";
            case FederalState.Thüringen: return "Thueringen";
            case FederalState.Insgesamt: return "Insgesamt";
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }
}