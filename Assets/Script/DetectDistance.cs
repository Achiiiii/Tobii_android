using System.Collections;
using System.Collections.Generic;
using Tobii;
using UnityEngine;
using UnityEngine.UI;

public class DetectDistance : MonoBehaviour
{
    public GameObject headGO;
    public GameObject displayGO;
    public GameObject canvasTrackBox;
    public float moverScale;
    public AudioSource audioSource;
    public AudioClip closerAudio;
    public AudioClip farerAudio;
    public GameObject pointer;
    public GazeCalibrationManager gazeCalibrationManager;
    public bool isMobile = true;

    private Color _colorMoverGood;
    private Color _colorMoverBad;
    private Color _colorEyeGood;
    private Color _colorEyeBad;

    private Rect _box;
    private Image _mover;
    private Image _colorPanel;
    private float _time = 0;
    private float _validateTime;
    private bool _locker = true;

    private float _min = 0.05f;
    private float _max = 0.95f;


    void Start()
    {
        var box = canvasTrackBox.transform.Find("ImageBox");
        _mover = box.Find("PanelMover").GetComponent<Image>();
        _colorPanel = box.Find("ImagePanel").GetComponent<Image>();
        _colorMoverGood = new Color32(23, 66, 57, 255);
        _colorMoverBad = new Color32(72, 26, 37, 255);
    }
    void Update()
    {
        if (_locker)
        {
            float headZ = headGO.transform.position.z;
            float displayZ = displayGO.transform.position.z;

            // Debug.Log($"headZ: {headZ}, displayZ: {displayZ}\ndistance: {headZ - displayZ}");

            moverScale = PositionMover(headZ, displayZ);
            _time += Time.deltaTime;
            if (moverScale < _min || moverScale > _max)
            {
                if (_time >= 5)
                {
                    if (moverScale < _min)
                    {
                        _time = 0;
                        PlayTTS("頭部請再靠近一點");
                    }
                    if (moverScale > _max)
                    {
                        _time = 0;
                        PlayTTS("頭部請再遠離一點");
                    }
                }
            }
            else _time = 0;

            if (moverScale > _min && moverScale < _max) _validateTime += Time.deltaTime;
            else _validateTime = 0;
            if (_validateTime >= 2)
            {
                _validateTime = 0;
                _locker = false;

                canvasTrackBox.SetActive(false);
                gazeCalibrationManager.SetTrialCountDown();
                Debug.Log("validate");
            }
        }

    }
    private void PlayTTS(string text)
    {
        if (text == "")
            return;
        Nuwa.stopTTS();
        Nuwa.startTTS(text);
    }
    private void AudioPlay(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
    private float PositionMover(float z1, float z2)
    {
        // Debug.Log(z1 - z2);
        var scale = isMobile ? 1.0f - Mathf.Abs(z1 - z2) : 1.5f - Mathf.Abs(z1 - z2);

        // Set the scale.
        _mover.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1f, scale);

        // Set the color.
        _colorPanel.color = Color.Lerp(_colorMoverGood, _colorMoverBad, Mathf.Abs(0.5f - scale) * 2f);
        return scale;
    }
}
