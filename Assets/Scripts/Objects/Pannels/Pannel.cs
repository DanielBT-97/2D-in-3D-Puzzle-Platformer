using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pannel : MonoBehaviour
{
    [SerializeField]
    private GameObject _perimeter, _platforms;

    public void DisablePannel() {
        _perimeter.SetActive(false);
        _platforms.SetActive(false);
    }

    public void EnablePannel() {
        _perimeter.SetActive(true);
        _platforms.SetActive(true);
    }
}