using UnityEngine;

public class Player_Starship_Mark : MonoBehaviour
{

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float speed = 30;
    [SerializeField] private float angleSpeed = 60;

    private bool isMouseOver;
    private Transform starshipRotTr;
    private Vector3 mouseV3;

    private void Awake()
    {
        Player_Starship_Controller psc = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        psc.OnMouseOver += OnMouseOv;
        psc.OnMouseOverEnd += OnMouseOverEnd;
        starshipRotTr = psc.GetComponent<Starship>().RotationPoint;
    }

    private void Update()
    {
        if (isMouseOver)
        {
            if (!starshipRotTr)
            {
                OnMouseOverEnd();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, starshipRotTr.rotation, angleSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, mouseV3, speed * Time.deltaTime);
            }
        }
    }

    private void OnMouseOv(Vector3 MouseV3)
    {
        mouseV3 = MouseV3;
        if (!isMouseOver)
        {
            transform.rotation = starshipRotTr.rotation;
            transform.position = mouseV3;
            isMouseOver = true;
            sprite.enabled = true;
        }
    }
    private void OnMouseOverEnd()
    {
        isMouseOver = false;
        sprite.enabled = false;
    }
}
