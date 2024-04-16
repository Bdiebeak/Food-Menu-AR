using ARMenu.Scripts.Runtime.UI.General.Mvvm;
using Unity.Properties;

namespace ARMenu.Scripts.Runtime.UI.ScanHint
{
	public class HintViewModel : ViewModel
	{
		[CreateProperty] public string HintText { get; private set; }

		public void SetHint(string text)
		{
			HintText = text;
			CallChangedEvent();
		}

		public override void Initialize()
		{
			CallChangedEvent();
		}

		public override void Clear()
		{
			HintText = string.Empty;
		}
	}
}