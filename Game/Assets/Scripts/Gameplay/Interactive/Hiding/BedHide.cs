public class BedHide : HideSpot
{
    private void Awake()
    {
        GetComponent<IInteractive>().isInteractive = true;
        GetComponent<IInteractive>().SetListener(Hide);
    }
}
