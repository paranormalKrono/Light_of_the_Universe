using UnityEngine;

public class SomeShootDialog : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private int inGameDialogId1;
    [SerializeField] private int inGameDialogId2;
    [SerializeField] private int bulletsCountOn1;
    [SerializeField] private int bulletsCountOn2;

    private int bulletsCount;

    private void OnCollisionEnter(Collision collision)
    {
        if (bulletsCount < bulletsCountOn2 && collision.gameObject.CompareTag("Bullet") && collision.gameObject.GetComponent<Bullet>().GetBulletType() == Bullet.BulletType.Player)
        {
            bulletsCount += 1;
            if (bulletsCountOn1 == bulletsCount)
            {
                GameDialogs.ShowInGameDialogEvent(inGameDialogId1);
            }
            if (bulletsCountOn2 == bulletsCount)
            {
                GameDialogs.ShowInGameDialogEvent(inGameDialogId2);
            }
        }
    }
}
