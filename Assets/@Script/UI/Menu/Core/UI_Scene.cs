public class UI_Scene : UI_Base
{
	protected override void Awake()
	{
		base.Awake();

		PManagers.DarkUI.SetCanvas(gameObject, false);
		//PManagers.DarkUI. = this;
	}
}