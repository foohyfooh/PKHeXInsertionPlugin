using PKHeX.Core;

namespace InsertionPlugin {
  public class InsertionPlugin : IPlugin {
    public string Name => nameof(InsertionPlugin);
    public int Priority => 1; // Loading order, lowest is first.

    // Initialized on plugin load
    public ISaveFileProvider SaveFileEditor { get; private set; } = null!;
    public IPKMView PKMEditor { get; private set; } = null!;

    public void Initialize(params object[] args) {
      Console.WriteLine($"Loading {Name}...");
      SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider)!;
      PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView)!;
      ToolStrip menu = (ToolStrip)Array.Find(args, z => z is ToolStrip)!;
      if (menu.Items.Find("Menu_Tools", false)[0] is ToolStripDropDownItem tools) {
        ToolStripMenuItem insertionPluginButton = new ToolStripMenuItem("Insertion Plugin");
        insertionPluginButton.Click += (s, e) => new PositionForm(SaveFileEditor).ShowDialog();
        tools.DropDownItems.Add(insertionPluginButton);
      }
    }

    public void NotifySaveLoaded() {
      Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
    }

    public bool TryLoadFile(string filePath) {
      Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
      return false; // no action taken
    }
  }
}
