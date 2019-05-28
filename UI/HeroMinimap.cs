// ======================================================================
//    Land Forgotten : Hero Minimap UI
//    Written by     : Ramon Partida, 2019 
//    Version        : 1.0
//    Program        : Unity 2018.2.18f1
// ======================================================================
using UnityEngine;
using UnityEngine.UI;

public class HeroMinimap : MonoBehaviour 
{

    [SerializeField] private GameObject _CPPrefab;
    [SerializeField] private GameObject _RotationPivot;
    [SerializeField] private Sprite[] _HeroIcons;
    [SerializeField] private float _Radius = 5.0f;

    private HeroesUI _HeroesUI;
    private GameObject[] _CPIcon = new GameObject[4];

    private void OnEnable() 
    {
        _HeroesUI = gameObject.GetComponentInParent<HeroesUI>();
    }

    private void Update() 
    {
        if(_HeroesUI.MyPlayer != null)
            ShowControlPointsDirection(_HeroesUI.MyPlayer);

    }

    private void SendUnderAttackPing(float value)
    {
      //  Debug.Log("Control Point Under Attack");
    }

    private void ShowControlPointsDirection(GameObject player)
    {
        for (int i = 0; i < ControllPointPing.ControlPointList.Count; i++)
        {
            if(ControllPointPing.ControlPointList[i] != null)
            {

                if(_CPIcon[i] == null)
                {
                    _CPIcon[i] = Instantiate(_CPPrefab, Vector3.zero, Quaternion.identity);
                    _CPIcon[i].transform.GetChild(0).GetComponent<Image>().color = ControllPointPing.ControlPointList[i].GetComponent<ControllPointPing>().AreaColor;
                    _CPIcon[i].transform.SetParent(_RotationPivot.transform, false);
                }

                float distance = Vector3.Distance(player.transform.position, ControllPointPing.ControlPointList[i].transform.position);
                if(distance > _Radius)
                {
                    _CPIcon[i].SetActive(true);
                    
                    Vector3 direction = (ControllPointPing.ControlPointList[i].transform.localPosition - player.transform.localPosition);
                    float angle = Vector3.SignedAngle(direction.normalized, player.transform.forward.normalized, Vector3.up);

                    _CPIcon[i].transform.rotation = Quaternion.Euler(0f, 0f, angle);

                    Debug.DrawRay(player.transform.localPosition, direction, Color.red);
                    Debug.DrawRay(player.transform.localPosition, player.transform.forward, Color.blue);
                    
                }
                else
                    _CPIcon[i].SetActive(false);
            }
            else
            {
                _CPIcon[i].SetActive(false);
            }
        }
    }

}
