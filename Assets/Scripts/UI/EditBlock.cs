using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class EditBlock : MonoBehaviour
{

    [SerializeField] private Button _editBtn;
    [SerializeField] private TMP_Text _editBtnTxt;
    [SerializeField] private string _editString;
    [SerializeField] private string _confirmString;
    [SerializeField] private List<EditToggle> _editToggles;
    [SerializeField] private UnityEvent<bool, EditToggle.EditType> _onEditBtnClick;
    private bool _isEditMode = false;
    private EditToggle.EditType _curEditType;
    private void Awake()
    {
        foreach (EditToggle editToggle in _editToggles)
        {
            if (editToggle.Toggle.isOn) _curEditType = editToggle.Type;
            editToggle.Toggle.onValueChanged.AddListener(delegate { _curEditType = editToggle.Type; });
        }
        _editBtn.onClick.AddListener(delegate {
            _isEditMode = !_isEditMode;
            foreach (EditToggle editToggle in _editToggles) editToggle.gameObject.SetActive(!_isEditMode);
            _editBtnTxt.text = _isEditMode ? _confirmString : _editString;
            _onEditBtnClick?.Invoke(_isEditMode, _curEditType);
        });
    }
}
