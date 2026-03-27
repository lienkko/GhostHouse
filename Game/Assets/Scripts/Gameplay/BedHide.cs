public class BedHide : HideSpot
{
    private void Awake()
    {
        GetComponent<Interactive>().isInteractive = true;
        GetComponent<Interactive>().SetListener(Hide);
    }
}
