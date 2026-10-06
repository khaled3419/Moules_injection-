using System.Text;
using ClosedXML.Excel;
using Microsoft.Maui.Controls;

namespace MoulesMachines;

public class MoulesPage : ContentPage
{
    private Entry entRecherche = null!;
    private Entry entMarge = null!;
    private Switch swIncomp = null!;
    private Label lblResultat = null!;

    private VerticalStackLayout formAjout = null!;

    private Entry fCode = null!;
    private Entry fNom = null!;
    private Entry fW = null!;
    private Entry fH = null!;
    private Entry fV = null!;
    private Entry fPoids = null!;
    private Entry fForce = null!;

    // =====================================================
    // CONSTRUCTEUR
    // =====================================================
    public MoulesPage()
    {
        Title = "Moule / Machine";

        BackgroundColor = Colors.White;

        // =================================================
        // RECHERCHE
        // =================================================
        entRecherche = Champ(
            "Code ou nom du moule",
            false);

        entRecherche.ClearButtonVisibility =
            ClearButtonVisibility.WhileEditing;

        // =================================================
        // MARGE
        // =================================================
        entMarge = Champ(
            "Marge H / V (vide = 0)",
            true);

        // =================================================
        // SWITCH
        // =================================================
        swIncomp = new Switch();

        var ligneSwitch =
            new HorizontalStackLayout();

        ligneSwitch.Spacing = 8;

        ligneSwitch.Children.Add(swIncomp);

        ligneSwitch.Children.Add(
            new Label
            {
                Text =
                    "Afficher aussi les machines non compatibles",
                VerticalOptions =
                    LayoutOptions.Center
            });

        // =================================================
        // RESULTAT
        // =================================================
        lblResultat = new Label
        {
            FontSize = 15,
            TextColor = Colors.Black
        };

        // =================================================
        // FORMULAIRE AJOUT
        // =================================================
        fCode = Champ("Code du moule", false);
        fNom = Champ("Nom du moule", false);

        fW = Champ("W", true);
        fH = Champ("H", true);
        fV = Champ("V", true);

        fPoids = Champ(
            "Poids (kg)",
            true);

        fForce = Champ(
            "Force de verrouillage (T)",
            true);

        formAjout =
            new VerticalStackLayout
            {
                Spacing = 6,
                IsVisible = false
            };

        formAjout.Children.Add(fCode);
        formAjout.Children.Add(fNom);
        formAjout.Children.Add(fW);
        formAjout.Children.Add(fH);
        formAjout.Children.Add(fV);
        formAjout.Children.Add(fPoids);
        formAjout.Children.Add(fForce);

        formAjout.Children.Add(
            Bouton(
                "💾 Enregistrer le moule",
                OnEnregistrer));

        // =================================================
        // LAYOUT PRINCIPAL
        // =================================================
        var pile =
            new VerticalStackLayout();

        pile.Padding = 16;
        pile.Spacing = 10;

        pile.Children.Add(
            new Label
            {
                Text = "GESTION MOULES / MACHINES",
                FontSize = 22,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions =
                    LayoutOptions.Center
            });

        pile.Children.Add(
            new Label
            {
                Text =
                    "Recherche et vérification de compatibilité",
                FontSize = 14,
                HorizontalOptions =
                    LayoutOptions.Center
            });

        pile.Children.Add(
            entRecherche);

        pile.Children.Add(
            entMarge);

        pile.Children.Add(
            ligneSwitch);

        // =================================================
        // BOUTONS PRINCIPAUX
        // =================================================
        pile.Children.Add(
            DeuxBoutons(
                Bouton(
                    "✅ Vérifier",
                    OnVerifier),

                Bouton(
                    "🗑 Supprimer",
                    OnSupprimer)
            ));

        pile.Children.Add(
            DeuxBoutons(
                Bouton(
                    "📋 Moules",
                    OnMoules),

                Bouton(
                    "🏭 Machines",
                    OnMachines)
            ));

        pile.Children.Add(
            Bouton(
                "➕ Ajouter un moule",
                OnToggleForm));

        pile.Children.Add(
            formAjout);

        // =================================================
        // RESULTAT
        // =================================================
        pile.Children.Add(
            new BoxView
            {
                HeightRequest = 1,
                Color = Colors.LightGray
            });

        pile.Children.Add(
            new Label
            {
                Text = "Résultat",
                FontSize = 18,
                FontAttributes =
                    FontAttributes.Bold
            });

        pile.Children.Add(
            lblResultat);

        Content =
            new ScrollView
            {
                Content = pile
            };
    }

    // =====================================================
    // CREATION BOUTON
    // =====================================================
    private static Button Bouton(
        string texte,
        EventHandler action)
    {
        var bouton = new Button
        {
            Text = texte,
            FontSize = 15,
            CornerRadius = 8,
            Padding = 10
        };

        bouton.Clicked += action;

        return bouton;
    }

    // =====================================================
    // CREATION ENTRY
    // =====================================================
    private static Entry Champ(
        string placeholder,
        bool numerique)
    {
        var entry = new Entry
        {
            Placeholder = placeholder,
            FontSize = 15
        };

        if (numerique)
        {
            entry.Keyboard =
                Keyboard.Numeric;
        }

        return entry;
    }

    // =====================================================
    // DEUX BOUTONS
    // =====================================================
    private static Grid DeuxBoutons(
        Button a,
        Button b)
    {
        var grid = new Grid
        {
            ColumnSpacing = 8
        };

        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = GridLength.Star
            });

        grid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = GridLength.Star
            });

        Grid.SetColumn(a, 0);
        Grid.SetColumn(b, 1);

        grid.Children.Add(a);
        grid.Children.Add(b);

        return grid;
    }

    // =====================================================
    // AFFICHER RESULTAT
    // =====================================================
    private void Afficher(string texte)
    {
        lblResultat.Text = texte;
    }

    // =====================================================
    // LIRE MARGE
    // =====================================================
    private bool LireMarge(
        out double marge)
    {
        marge = 0;

        string texte =
            (entMarge.Text ?? "")
            .Trim();

        if (texte == "")
            return true;

        return Donnees.Nombre(
            texte,
            out marge);
    }

    // =====================================================
    // VERIFIER COMPATIBILITE
    // =====================================================
    private void OnVerifier(
        object? sender,
        EventArgs e)
    {
        try
        {
            string recherche =
                (entRecherche.Text ?? "")
                .Trim();

            if (recherche == "")
            {
                Afficher(
                    "❌ Entrez un code ou un nom de moule.");

                return;
            }

            if (!LireMarge(out double marge))
            {
                Afficher(
                    "❌ Marge incorrecte.");

                return;
            }

            Donnees.Preparer();

            using var wb =
                new XLWorkbook(
                    Donnees.Fichier);

            var moules =
                Donnees.LireMoules(wb);

            var machines =
                Donnees.LireMachines(wb);

            Moule? moule =
                Donnees.Chercher(
                    moules,
                    recherche);

            if (moule == null)
            {
                Afficher(
                    "❌ Moule introuvable.");

                return;
            }

            var sb =
                new StringBuilder();

            sb.AppendLine(
                "════════════════════════");

            sb.AppendLine(
                "      DONNÉES DU MOULE");

            sb.AppendLine(
                "════════════════════════");

            sb.AppendLine(
                $"Code : {moule.Code}");

            sb.AppendLine(
                $"Nom : {moule.Nom}");

            sb.AppendLine(
                $"W : {moule.W}");

            sb.AppendLine(
                $"H : {moule.H}");

            sb.AppendLine(
                $"V : {moule.V}");

            sb.AppendLine(
                $"Poids : {moule.Poids} kg");

            sb.AppendLine(
                $"Force : {moule.Force} T");

            sb.AppendLine(
                $"Marge H/V : {marge}");

            sb.AppendLine();

            sb.AppendLine(
                "════════════════════════");

            sb.AppendLine(
                "   MACHINES COMPATIBLES");

            sb.AppendLine(
                "════════════════════════");

            int nbCompatibles = 0;

            var incompatibles =
                new StringBuilder();

            var incompletes =
                new List<string>();

            foreach (var machine in machines)
            {
                if (!machine.Complet)
                {
                    incompletes.Add(
                        machine.Type);

                    continue;
                }

                var raisons =
                    Donnees.Verifier(
                        moule,
                        machine,
                        marge);

                // -----------------------------------------
                // COMPATIBLE
                // -----------------------------------------
                if (raisons.Count == 0)
                {
                    nbCompatibles++;

                    sb.AppendLine();

                    sb.AppendLine(
                        $"✅ {machine.Type}");

                    sb.AppendLine(
                        $"   Force : {machine.Force} T");

                    sb.AppendLine(
                        $"   W max : {machine.WMax}");

                    sb.AppendLine(
                        $"   H min : {machine.HMin}");

                    sb.AppendLine(
                        $"   V min : {machine.VMin}");

                    sb.AppendLine(
                        $"   Poids max : {machine.PoidsMax} kg");
                }

                // -----------------------------------------
                // NON COMPATIBLE
                // -----------------------------------------
                else
                {
                    incompatibles.AppendLine();

                    incompatibles.AppendLine(
                        $"❌ {machine.Type}");

                    foreach (var raison in raisons)
                    {
                        incompatibles.AppendLine(
                            $"   - {raison}");
                    }
                }
            }

            if (nbCompatibles == 0)
            {
                sb.AppendLine();

                sb.AppendLine(
                    "❌ Aucune machine compatible.");
            }

            // =================================================
            // MACHINES NON COMPATIBLES
            // =================================================
            if (swIncomp.IsToggled &&
                incompatibles.Length > 0)
            {
                sb.AppendLine();

                sb.AppendLine(
                    "════════════════════════");

                sb.AppendLine(
                    "   MACHINES NON COMPATIBLES");

                sb.AppendLine(
                    "════════════════════════");

                sb.Append(
                    incompatibles.ToString());
            }

            // =================================================
            // MACHINES AVEC DONNEES MANQUANTES
            // =================================================
            if (incompletes.Count > 0)
            {
                sb.AppendLine();

                sb.AppendLine(
                    "════════════════════════");

                sb.AppendLine(
                    "⚠️ DONNÉES INCOMPLÈTES");

                sb.AppendLine(
                    "════════════════════════");

                sb.AppendLine(
                    string.Join(
                        ", ",
                        incompletes));
            }

            Afficher(
                sb.ToString());
        }
        catch (Exception ex)
        {
            Afficher(
                "❌ ERREUR : " +
                ex.Message);
        }
    }

    // =====================================================
    // AFFICHER / MASQUER FORMULAIRE
    // =====================================================
    private void OnToggleForm(
        object? sender,
        EventArgs e)
    {
        formAjout.IsVisible =
            !formAjout.IsVisible;
    }

    // =====================================================
    // ENREGISTRER MOULE
    // =====================================================
    private async void OnEnregistrer(
        object? sender,
        EventArgs e)
    {
        try
        {
            string code =
                (fCode.Text ?? "")
                .Trim();

            string nom =
                (fNom.Text ?? "")
                .Trim();

            if (code == "")
            {
                Afficher(
                    "❌ Le code du moule est obligatoire.");

                return;
            }

            if (!Donnees.Nombre(
                    fW.Text,
                    out double w) ||

                !Donnees.Nombre(
                    fH.Text,
                    out double h) ||

                !Donnees.Nombre(
                    fV.Text,
                    out double v) ||

                !Donnees.Nombre(
                    fPoids.Text,
                    out double poids) ||

                !Donnees.Nombre(
                    fForce.Text,
                    out double force))
            {
                Afficher(
                    "❌ Valeurs numériques incorrectes.");

                return;
            }

            Donnees.Preparer();

            using var wb =
                new XLWorkbook(
                    Donnees.Fichier);

            var moules =
                Donnees.LireMoules(wb);

            if (Donnees.Chercher(
                    moules,
                    code) != null)
            {
                Afficher(
                    "❌ Ce code existe déjà.");

                return;
            }

            var ws =
                wb.Worksheet("Moules");

            var derniere =
                ws.LastRowUsed();

            int ligne =
                derniere == null
                    ? 2
                    : derniere.RowNumber() + 1;

            ws.Cell(ligne, 1)
                .Value = code;

            ws.Cell(ligne, 2)
                .Value = nom;

            ws.Cell(ligne, 3)
                .Value = w;

            ws.Cell(ligne, 4)
                .Value = h;

            ws.Cell(ligne, 5)
                .Value = v;

            ws.Cell(ligne, 6)
                .Value = poids;

            ws.Cell(ligne, 7)
                .Value = force;

            wb.Save();

            fCode.Text = "";
            fNom.Text = "";
            fW.Text = "";
            fH.Text = "";
            fV.Text = "";
            fPoids.Text = "";
            fForce.Text = "";

            formAjout.IsVisible =
                false;

            Afficher(
                $"✅ Moule {code} ajouté.");
        }
        catch (Exception ex)
        {
            Afficher(
                "❌ ERREUR : " +
                ex.Message);
        }
    }

    // =====================================================
    // SUPPRIMER MOULE
    // =====================================================
    private async void OnSupprimer(
        object? sender,
        EventArgs e)
    {
        try
        {
            string recherche =
                (entRecherche.Text ?? "")
                .Trim();

            if (recherche == "")
            {
                Afficher(
                    "❌ Entrez le code ou le nom du moule à supprimer.");

                return;
            }

            Donnees.Preparer();

            int ligneTrouvee = -1;

            string codeTrouve = "";

            using (var wb =
                   new XLWorkbook(
                       Donnees.Fichier))
            {
                var ws =
                    wb.Worksheet("Moules");

                foreach (var row in ws.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string code =
                        row.Cell(1)
                           .GetString()
                           .Trim();

                    string nom =
                        row.Cell(2)
                           .GetString()
                           .Trim();

                    if (
                        code.Equals(
                            recherche,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        nom.Equals(
                            recherche,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        ligneTrouvee =
                            row.RowNumber();

                        codeTrouve =
                            code;

                        break;
                    }
                }
            }

            if (ligneTrouvee == -1)
            {
                Afficher(
                    "❌ Moule introuvable.");

                return;
            }

            bool confirmation =
                await DisplayAlert(
                    "Supprimer",
                    $"Supprimer le moule {codeTrouve} ?",
                    "Oui",
                    "Non");

            if (!confirmation)
            {
                Afficher(
                    "Annulé.");

                return;
            }

            using (var wb =
                   new XLWorkbook(
                       Donnees.Fichier))
            {
                wb.Worksheet("Moules")
                    .Row(ligneTrouvee)
                    .Delete();

                wb.Save();
            }

            Afficher(
                $"✅ Moule {codeTrouve} supprimé.");
        }
        catch (Exception ex)
        {
            Afficher(
                "❌ ERREUR : " +
                ex.Message);
        }
    }

    // =====================================================
    // AFFICHER LES MOULES
    // =====================================================
    private void OnMoules(
        object? sender,
        EventArgs e)
    {
        try
        {
            Donnees.Preparer();

            using var wb =
                new XLWorkbook(
                    Donnees.Fichier);

            var moules =
                Donnees.LireMoules(wb);

            var sb =
                new StringBuilder();

            sb.AppendLine(
                $"════════ MOULES ({moules.Count}) ════════");

            foreach (var moule in moules)
            {
                sb.AppendLine();

                sb.AppendLine(
                    $"• {moule.Code} - {moule.Nom}");

                sb.AppendLine(
                    $"  W : {moule.W}");

                sb.AppendLine(
                    $"  H : {moule.H}");

                sb.AppendLine(
                    $"  V : {moule.V}");

                sb.AppendLine(
                    $"  Poids : {moule.Poids} kg");

                sb.AppendLine(
                    $"  Force : {moule.Force} T");
            }

            if (moules.Count == 0)
            {
                sb.AppendLine();

                sb.AppendLine(
                    "Aucun moule enregistré.");
            }

            Afficher(
                sb.ToString());
        }
        catch (Exception ex)
        {
            Afficher(
                "❌ ERREUR : " +
                ex.Message);
        }
    }

    // =====================================================
    // AFFICHER LES MACHINES
    // =====================================================
    private void OnMachines(
        object? sender,
        EventArgs e)
    {
        try
        {
            Donnees.Preparer();

            using var wb =
                new XLWorkbook(
                    Donnees.Fichier);

            var machines =
                Donnees.LireMachines(wb);

            var sb =
                new StringBuilder();

            sb.AppendLine(
                $"════════ MACHINES ({machines.Count}) ════════");

            foreach (var machine in machines)
            {
                sb.AppendLine();

                sb.AppendLine(
                    $"• {machine.Type}");

                if (!machine.Complet)
                {
                    sb.AppendLine(
                        "  ⚠️ Données incomplètes dans Excel");

                    continue;
                }

                sb.AppendLine(
                    $"  Force : {machine.Force} T");

                sb.AppendLine(
                    $"  W max : {machine.WMax}");

                sb.AppendLine(
                    $"  H min : {machine.HMin}");

                sb.AppendLine(
                    $"  V min : {machine.VMin}");

                sb.AppendLine(
                    $"  Poids max : {machine.PoidsMax} kg");
            }

            if (machines.Count == 0)
            {
                sb.AppendLine();

                sb.AppendLine(
                    "Aucune machine enregistrée.");
            }

            Afficher(
                sb.ToString());
        }
        catch (Exception ex)
        {
            Afficher(
                "❌ ERREUR : " +
                ex.Message);
        }
    }
}
