using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
using Tools;
using System.Threading;

public class TimeReciever : MonoBehaviour
{
    public static Action<DateTime> OnRecievedTime;

    [SerializeField] private string _requestedUrl;
    [SerializeField] private int _requestDelay;
    [SerializeField] private ErrorPopUp _errorPopUp;
    [SerializeField] private GameObject _editBlock; 

    private CancellationTokenSource _sourceToken;
    private CancellationToken _tokenUtilityInterface;
    private bool _timeStored = false;
    private bool _isNetReachible = true;


    void Start()
    {
        CreateNewCancellationToken();
        CheckInternetConnection();
    }

    private async UniTaskVoid GetWebRequest(string url, CancellationToken cancellationToken)
    {
        while (true)
        {
            var request = new UnityWebRequest();
            do
            {
                try
                {
                    request = await UnityWebRequest.Get(url).SendWebRequest().WithCancellation(cancellationToken);
                }
                catch
                {
                    _errorPopUp.ShowErrorAlert();
                    await UniTask.Delay(TimeSpan.FromSeconds(5), ignoreTimeScale: false, cancellationToken: cancellationToken);
                }
            } while (request.result != UnityWebRequest.Result.Success);
            Debug.Log(request.downloadHandler.text);
            if (!_timeStored)
            {
                _timeStored = true;
                _editBlock.SetActive(true);
            }
            _errorPopUp.CloseErrorAlert();
            OnRecievedTime?.Invoke(GetCurrentTime(request.downloadHandler.text));
            await UniTask.Delay(TimeSpan.FromSeconds(_requestDelay), ignoreTimeScale: false, cancellationToken: cancellationToken);
        }
        
        
    }

    private DateTime GetCurrentTime(string timeStr)
    {
        JSONNode node = JSONNode.Parse(timeStr);
        DateTime currentTime = new DateTime(year: node["year"].AsInt, month: node["month"].AsInt, day: node["day"].AsInt,
            hour: node["hour"].AsInt, minute: node["minute"].AsInt, second: node["seconds"].AsInt, millisecond: node["milliSeconds"].AsInt);
        return currentTime;
    }

    private async UniTaskVoid CheckInternetConnection()
    {
        CheckNetReachability();
        do
        {
            if (!_isNetReachible)
            {
                if (_timeStored) _errorPopUp.ShowSubmitBtn();
                _errorPopUp.ShowErrorAlert();
                _sourceToken.Cancel();
                await UniTask.WaitUntil(() => _isNetReachible );
            }
            else
            {
                CreateNewCancellationToken();
                GetWebRequest(_requestedUrl, _tokenUtilityInterface);
                await UniTask.WaitUntil(() => !_isNetReachible);
            }
        } while (true);
    }

    private async UniTaskVoid CheckNetReachability()
    {
        do
        {
            try
            {
                var ping = UnityWebRequest.Get(_requestedUrl);
                ping.timeout = 5;
                ping = await ping.SendWebRequest();
                _isNetReachible = true;
            }
            catch
            {
                _isNetReachible = false;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);
        } while (true);
    }

    private void CreateNewCancellationToken()
    {
        _sourceToken = new CancellationTokenSource();
        _tokenUtilityInterface = _sourceToken.Token;
    }

    
}
