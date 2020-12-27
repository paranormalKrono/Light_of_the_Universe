using UnityEngine;
using UnityEngine.UI;

public class PlanetCoordinates : MonoBehaviour
{

    [SerializeField] private string planetKey = "Wasp";

    [SerializeField] private string GalacticLongitude;
    [SerializeField] private string GalacticLatitude;
    [SerializeField] private string Distance;

    [SerializeField] private Button CheckButton;
    [SerializeField] private Button PlanetButton;
    [SerializeField] private InputField inputFieldGLong;
    [SerializeField] private InputField inputFieldGLati;
    [SerializeField] private InputField inputFieldDistance;

    [SerializeField] private Text Nothing;
    [SerializeField] private Text Agree;

    internal delegate void PlanetFindedD();
    internal PlanetFindedD PlanetFinded;

    private void Start()
    {
        if (PlayerPrefs.HasKey(planetKey))
        {
            inputFieldGLong.interactable = false;
            inputFieldGLong.text = GalacticLongitude;
            inputFieldGLati.interactable = false;
            inputFieldGLati.text = GalacticLatitude;
            inputFieldDistance.interactable = false;
            inputFieldDistance.text = Distance;
            CorrectAnswer();
        }
    }

    public void CheckCoordinates()
    {
        bool isAnswered = true;
        if (GalacticLongitude == inputFieldGLong.text)
        {
            inputFieldGLong.interactable = false;
        }
        else
        {
            isAnswered = false;
        }
        if (GalacticLatitude == inputFieldGLati.text)
        {
            inputFieldGLati.interactable = false;
        }
        else
        {
            isAnswered = false;
        }
        if (Distance == inputFieldDistance.text)
        {
            inputFieldDistance.interactable = false;
        }
        else
        {
            isAnswered = false;
        }
        if (isAnswered)
        {
            CorrectAnswer();
        }
    }

    private void CorrectAnswer()
    {
        PlayerPrefs.SetInt(planetKey, 1);
        Agree.enabled = true;
        Nothing.enabled = false;
        CheckButton.gameObject.SetActive(false);
        PlanetButton.gameObject.SetActive(true);
        PlanetFinded?.Invoke();
    }
}