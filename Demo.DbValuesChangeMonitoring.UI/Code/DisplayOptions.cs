namespace Demo.DbValuesChangeMonitoring.UI.Code
{
	public class DisplayOptions
	{
		public FontSize FontSize { get; set; }
		public Theme Theme { get; set; }
		public NavigationBar NavigationBar { get; set; }
	}

	public enum FontSize
	{
		None,
		Small,
		Medium,
		Large
	}

	public enum Theme
	{
		None,
		Dark,
		Light
	}

	public enum NavigationBar
	{
		None,
		Left,
		Right,
		Top,
		Bottom
	}
}
