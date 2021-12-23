using UnityEngine;
using UnityEngine.UI;

public class ShooterView : MonoBehaviour
{
    public ShooterMediator mediator;
    public RectTransform aimer;
    public GameObject pinSpot;
    public Image reloadImg;
    public GameObject bullet;

    private void Start()
    {
        mediator = new ShooterMediator();
        mediator.SetUp(this);
    }

    private void OnDestroy()
    {
        mediator.Dispose();
    }

    public void RotateAimer(float speed)
    {
        aimer.Rotate(new Vector3(0, 0, speed));
    }

    public float GetAimerRotation()
    {
        return aimer.localRotation.eulerAngles.z + 90;
    }

    public void UpdateReloadImg(float progerss)
    {
        reloadImg.fillAmount = progerss;
        reloadImg.transform.parent.gameObject.SetActive(progerss < 1);
    }

    public void OnShoot()
    {

    }

}
