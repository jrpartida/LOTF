// ======================================================================
//    Land Forgotten : Hero Gameplay UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDiedAnnouncement : MonoBehaviour 
{
	[SerializeField] private TMPro.TMP_Text _HeroDeadTxt;
	[SerializeField] private Image _DeadPlayerImage;
	[SerializeField] private Sprite[] _CharacterIcons;

	private void Awake()
	{
		Reset();
	}

	public void Reset()
	{
		_HeroDeadTxt.text = "";
		_DeadPlayerImage.enabled = false;
	}

	public IEnumerator PlayerDeadAnnouncement(CharacterSelectionType character)
    {
		_DeadPlayerImage.enabled = true;
        _HeroDeadTxt.text = character.ToString() + " died!";
		_DeadPlayerImage.sprite = _CharacterIcons[(int)character];
        yield return new WaitForSeconds(2.0f);

        _HeroDeadTxt.text = "";
		_DeadPlayerImage.enabled = false;

    }
}
