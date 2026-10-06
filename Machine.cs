namespace MoulesMachines;

public class Machine
{
    public string Type { get; set; } = "";

    public double Force { get; set; }

    public double HMin { get; set; }
    public double VMin { get; set; }

    public double WMax { get; set; }

    public double PoidsMax { get; set; }

    public bool Complet { get; set; }
}
