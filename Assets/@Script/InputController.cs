using System;
using UnityEngine;

[CreateAssetMenu]
public class InputController : ScriptableObject
{
	private PlayerInputAction action;
	public PlayerInputAction.PlayerActions Player => action.Player;
	public PlayerInputAction.UIActions UI => action.UI;

	public PlayerInputAction.GameUIActions GameUI
	{
		get
		{
			if (action == null)
				Init();
			
			return action.GameUI;
		}
	}

	public void Init()
	{
		action ??= new PlayerInputAction();
	}

	public void PlayerInputSwitch(bool _enable)
	{
		if (action == null) Init(); // ui 매니저가 없는 테스트 신에서 플레이어만 있을 때 발생하는 에러 방지용 코드
		if (_enable)
			Player.Enable();
		else
			Player.Disable();
	}

	public void UIInputSwitch(bool _enable)
	{
		if (_enable)
			UI.Enable();
		else
			UI.Disable();
	}

	public void GameUISwitch(bool _enable)
	{
		if (action == null) Init();
		if (_enable)
			GameUI.Enable();
		else
			GameUI.Disable();
	}

	internal bool GetKeyDown(KeyCode p)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Player 맵의 개별 액션을 카테고리별로 Enable/Disable.
	/// PlayerInputSwitch()는 전체 맵을 일괄 온오프하지만,
	/// 이 메서드는 "이동은 되지만 상호작용은 안 됨" 같은 세밀한 제어가 가능.
	///
	/// ── 파라미터별 제어 범위 ──
	///   move:     Movement, Run, Crouch
	///   interact: Interaction(E키), Throw(좌클릭), TimerAdjust(스크롤)
	///   flash:    Flash(F키)
	///   look:     Look(마우스 이동)
	///
	/// ── interact 그룹에 3개 액션이 묶인 이유 ──
	///   Interaction(E키 줍기), Throw(좌클릭 던지기), TimerAdjust(스크롤 조절)는
	///   모두 "투척체 상호작용" 관련 입력이므로 항상 같이 켜지고 꺼져야 함.
	///
	///   만약 분리되면 발생하는 문제:
	///     - interact=false인데 Throw만 켜짐 → 대화 중 좌클릭으로 던지기 발동
	///     - interact=false인데 TimerAdjust만 켜짐 → 대화 중 스크롤로 딜레이 변경
	///     - interact=true인데 TimerAdjust만 꺼짐 → 타이머 들고 있는데 조절 불가
	///
	///   셋 다 같이 켜고 끄면 이런 불일치가 원천 차단됨.
	///
	/// ── 호출 예시 ──
	///   대화 시작: PlayerInputSetting(false, false, false, false) → 모든 입력 차단
	///   대화 종료: PlayerInputSetting(true, true, true, true) → 모든 입력 복원
	///   컷씬 중:  PlayerInputSetting(false, false, false, true) → 시선만 가능
	/// </summary>
	public void PlayerInputSetting(bool move, bool interact, bool flash, bool look)
	{
		if (move)
		{
			Player.Movement.Enable();
			Player.Run.Enable();
			Player.Crouch.Enable();
		}
		else
		{
			Player.Movement.Disable();
			Player.Run.Disable();
			Player.Crouch.Disable();
		}

		// ── 투척 상호작용 3종: Interaction + Throw + TimerAdjust ──
		// 셋 다 interact 파라미터로 일괄 제어.
		// E키(줍기), 좌클릭(던지기), 스크롤(타이머 딜레이 조절) 모두 같은 상황에서
		// 차단/허용되어야 하므로 하나의 그룹으로 묶음.
		if (interact)
		{
			Player.Interaction.Enable();   // E키 줍기
			Player.Throw.Enable();         // 좌클릭 던지기
			Player.TimerAdjust.Enable();   // 마우스 스크롤 타이머 조절
		}
		else
		{
			Player.Interaction.Disable();
			Player.Throw.Disable();
			Player.TimerAdjust.Disable();
		}

		if (flash) Player.Flash.Enable();
		else Player.Flash.Disable();

		if (look) Player.Look.Enable();
		else Player.Look.Disable();
	}

	//public void PlayerSkillInputSetting(bool isUse)
	//{
	//	if (isUse) Player.TimeStop.Enable();
 //       else Player.TimeStop.Disable();
 //   }
}