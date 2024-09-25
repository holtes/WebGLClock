using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopUp : MonoBehaviour
{
    [SerializeField] private Button _submitBtn;

    private void Awake()
    {
        _submitBtn.onClick.AddListener(delegate { gameObject.SetActive(false); });
    }

    public void ShowErrorAlert()
    {
        gameObject.SetActive(true);
    }

    public void CloseErrorAlert()
    {
        gameObject.SetActive(false);
    }

    public void ShowSubmitBtn()
    {
        _submitBtn.gameObject.SetActive(true);
    }

    public void CloseSubmitBtn()
    {
        _submitBtn.gameObject.SetActive(false);
    }
}
