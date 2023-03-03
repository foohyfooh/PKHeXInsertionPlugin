using System.Text.RegularExpressions;
using PKHeX.Core;

namespace InsertionPlugin
{
  public partial class PositionForm : Form
  {

    private readonly Regex notNumberRegex = NotNumberRegex();
    private readonly ISaveFileProvider saveFileEditor;
    public PositionForm(ISaveFileProvider iSaveFileProvider)
    {
      InitializeComponent();
      saveFileEditor = iSaveFileProvider;
    }

    private void boxNumberLabel_Click(object sender, EventArgs e) => boxNumber.Focus();

    private void slotNumberLabel_Click(object sender, EventArgs e) => slotNumber.Focus();

    private void ValidateNumberInput(object sender, EventArgs e)
    {
      TextBox textBox = (TextBox)sender;
      if (notNumberRegex.IsMatch(textBox.Text))
      {
        int selectionStart = textBox.SelectionStart - 1;
        textBox.Text = Regex.Replace(textBox.Text, notNumberRegex.ToString(), string.Empty);
        textBox.Select(selectionStart, 0);
      }
    }

    private void boxNumber_TextChanged(object sender, EventArgs e) => ValidateNumberInput(sender, e);
    private void slotNumber_TextChanged(object sender, EventArgs e) => ValidateNumberInput(sender, e);

    private void ShowErrorMessageBox(string text) => MessageBox.Show(text, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

    private void insertSpotButton_Click(object sender, EventArgs e)
    {
      if (boxNumber.Text.Length == 0 || slotNumber.Text.Length == 0) return;

      int boxNum = int.Parse(boxNumber.Text);
      int slotNum = int.Parse(slotNumber.Text);
      bool hadError = false;
      string errorText = string.Empty;
      if (boxNum < 1 || boxNum > saveFileEditor.SAV.BoxCount)
      {
        errorText = $"Box Number should be between 1 and {saveFileEditor.SAV.BoxCount}";
        hadError = true;
      }
      if (slotNum < 1 || slotNum > saveFileEditor.SAV.BoxSlotCount)
      {
        if (errorText.Length > 0) errorText += "\n";
        errorText += $"Slot Number should be between 1 and {saveFileEditor.SAV.BoxSlotCount}";
        hadError = true;
      }
      if (hadError)
      {
        ShowErrorMessageBox(errorText);
        return;
      }

      int startIndex = (boxNum - 1) * saveFileEditor.SAV.BoxSlotCount + (slotNum - 1);
      PKM currMon = saveFileEditor.SAV.GetBoxSlotAtIndex(startIndex), nextMon;
      if(currMon.Species == (int)Species.None)
      {
        ShowErrorMessageBox($"Box {boxNum} Slot {slotNum} is already empty");
        return;
      }
      int boxIndex = startIndex + 1;
      while (boxIndex < saveFileEditor.SAV.SlotCount)
      {
        currMon = saveFileEditor.SAV.GetBoxSlotAtIndex(boxIndex);
        if (currMon.Species == (int)Species.None) break;
        boxIndex++;
      }
      if(boxIndex == saveFileEditor.SAV.SlotCount)
      {
        ShowErrorMessageBox($"There are no empty slots after Box {boxNum} Slot {slotNum}");
        return;
      }
      currMon = saveFileEditor.SAV.GetBoxSlotAtIndex(startIndex);
      saveFileEditor.SAV.SetBoxSlotAtIndex(saveFileEditor.SAV.BlankPKM, startIndex);
      for (int index = startIndex + 1; index <= boxIndex; index++)
      {
        nextMon = saveFileEditor.SAV.GetBoxSlotAtIndex(index);
        saveFileEditor.SAV.SetBoxSlotAtIndex(currMon, index, PKMImportSetting.UseDefault, PKMImportSetting.Skip);
        currMon = nextMon;
      }
      saveFileEditor.ReloadSlots();
      Close();
    }

    [GeneratedRegex("[^0-9]")]
    private static partial Regex NotNumberRegex();
  }
}
