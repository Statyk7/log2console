using System.Windows.Forms;


namespace Log2Console.Settings
{
  public partial class SettingsForm : Form
  {
    public SettingsForm(UserSettings userSettings)
    {
      InitializeComponent();

      Font = UserSettings.Instance.DefaultFont ?? Font;

      // UI Settings
      UserSettings = userSettings;
    }

    public UserSettings UserSettings
    {
      get { return settingsPropertyGrid.SelectedObject as UserSettings; }
      set
      {
        settingsPropertyGrid.SelectedObject = value;
      }
    }
  }
}