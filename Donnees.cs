using System.Globalization;
using ClosedXML.Excel;

namespace MoulesMachines;

public static class Donnees
{
    // سيتم وضع ملف Excel داخل مجلد Documents في الهاتف.
    public static string Fichier =>
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments),
            "Machines_Moules.xlsx");

    // =====================================================
    // إنشاء ملف Excel إذا لم يكن موجودًا
    // =====================================================
    public static void Preparer()
    {
        string? dossier = Path.GetDirectoryName(Fichier);

        if (!string.IsNullOrEmpty(dossier) &&
            !Directory.Exists(dossier))
        {
            Directory.CreateDirectory(dossier);
        }

        if (File.Exists(Fichier))
            return;

        using var wb = new XLWorkbook();

        // -----------------------------
        // Feuille Moules
        // -----------------------------
        var m = wb.Worksheets.Add("Moules");

        m.Cell(1, 1).Value = "CodeMoule";
        m.Cell(1, 2).Value = "NomMoule";
        m.Cell(1, 3).Value = "W";
        m.Cell(1, 4).Value = "H";
        m.Cell(1, 5).Value = "V";
        m.Cell(1, 6).Value = "Poids";
        m.Cell(1, 7).Value = "Force";

        // -----------------------------
        // Feuille Machines
        // -----------------------------
        var a = wb.Worksheets.Add("Machines");

        a.Cell(1, 1).Value = "TypeMachine";
        a.Cell(1, 2).Value = "ForceMachine";
        a.Cell(1, 3).Value = "MinH";
        a.Cell(1, 4).Value = "MinV";
        a.Cell(1, 5).Value = "MinW";
        a.Cell(1, 6).Value = "MaxW";
        a.Cell(1, 7).Value = "PoidsMax";

        wb.SaveAs(Fichier);
    }

    // =====================================================
    // Lire un nombre depuis Excel
    // =====================================================
    public static double Num(IXLCell cell)
    {
        try
        {
            if (cell.IsEmpty())
                return 0;

            return cell.GetDouble();
        }
        catch
        {
            string valeur = cell.GetString()
                .Trim()
                .Replace(',', '.');

            if (double.TryParse(
                valeur,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out double resultat))
            {
                return resultat;
            }

            return 0;
        }
    }

    // =====================================================
    // Vérifier un nombre saisi
    // =====================================================
    public static bool Nombre(string? s, out double valeur)
    {
        return double.TryParse(
                   (s ?? "")
                       .Trim()
                       .Replace(',', '.'),
                   NumberStyles.Any,
                   CultureInfo.InvariantCulture,
                   out valeur)
               && valeur >= 0;
    }

    // =====================================================
    // Lire les moules
    // =====================================================
    public static List<Moule> LireMoules(XLWorkbook wb)
    {
        var liste = new List<Moule>();

        if (!wb.Worksheets.Contains("Moules"))
            return liste;

        var ws = wb.Worksheet("Moules");

        foreach (var row in ws.RowsUsed())
        {
            if (row.RowNumber() == 1)
                continue;

            string code = row.Cell(1)
                .GetString()
                .Trim();

            if (code == "")
                continue;

            var m = new Moule
            {
                Code = code,
                Nom = row.Cell(2)
                    .GetString()
                    .Trim(),

                W = Num(row.Cell(3)),
                H = Num(row.Cell(4)),
                V = Num(row.Cell(5)),
                Poids = Num(row.Cell(6)),
                Force = Num(row.Cell(7))
            };

            liste.Add(m);
        }

        return liste;
    }

    // =====================================================
    // Lire les machines
    // =====================================================
    public static List<Machine> LireMachines(XLWorkbook wb)
    {
        var liste = new List<Machine>();

        if (!wb.Worksheets.Contains("Machines"))
            return liste;

        var ws = wb.Worksheet("Machines");

        foreach (var row in ws.RowsUsed())
        {
            if (row.RowNumber() == 1)
                continue;

            string type = row.Cell(1)
                .GetString()
                .Trim();

            if (type == "")
                continue;

            bool complet =
                !row.Cell(2).IsEmpty() &&
                !row.Cell(3).IsEmpty() &&
                !row.Cell(4).IsEmpty() &&
                !row.Cell(6).IsEmpty() &&
                !row.Cell(7).IsEmpty();

            var machine = new Machine
            {
                Type = type,

                Force = Num(row.Cell(2)),
                HMin = Num(row.Cell(3)),
                VMin = Num(row.Cell(4)),
                WMax = Num(row.Cell(6)),
                PoidsMax = Num(row.Cell(7)),

                Complet = complet
            };

            liste.Add(machine);
        }

        return liste;
    }

    // =====================================================
    // Recherche d'un moule par code ou nom
    // =====================================================
    public static Moule? Chercher(
        List<Moule> moules,
        string recherche)
    {
        foreach (var m in moules)
        {
            if (m.Code.Equals(
                    recherche,
                    StringComparison.OrdinalIgnoreCase)
                ||
                m.Nom.Equals(
                    recherche,
                    StringComparison.OrdinalIgnoreCase))
            {
                return m;
            }
        }

        return null;
    }

    // =====================================================
    // Vérification de compatibilité
    //
    // Liste vide = compatible
    // =====================================================
    public static List<string> Verifier(
        Moule moule,
        Machine machine,
        double marge)
    {
        var raisons = new List<string>();

        double vMin = machine.VMin - marge;
        double hMin = machine.HMin - marge;

        // W maximum
        if (moule.W > machine.WMax)
        {
            raisons.Add(
                $"W moule {moule.W} > W max {machine.WMax}");
        }

        // V minimum
        if (moule.V < vMin)
        {
            raisons.Add(
                $"V moule {moule.V} < V min {vMin}");
        }

        // H minimum
        if (moule.H < hMin)
        {
            raisons.Add(
                $"H moule {moule.H} < H min {hMin}");
        }

        // Force de verrouillage
        if (moule.Force > machine.Force)
        {
            raisons.Add(
                $"Force moule {moule.Force} > Force machine {machine.Force}");
        }

        // Poids maximum
        if (moule.Poids > machine.PoidsMax)
        {
            raisons.Add(
                $"Poids moule {moule.Poids} > Poids max {machine.PoidsMax}");
        }

        return raisons;
    }
}
