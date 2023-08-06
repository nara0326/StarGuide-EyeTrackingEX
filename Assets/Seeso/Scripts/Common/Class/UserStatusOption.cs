public class UserStatusOption
{
    private bool modeAttention;
    private bool modeBlink;
    private bool modeDrowsiness;


    public UserStatusOption()
    {
        modeAttention = false;
        modeBlink = false;
        modeDrowsiness = false;
    }

    public void useAttention()
    {
        modeAttention = true;
    }

    public void useBlink()
    {
        modeBlink = true;
    }

    public void useDrowsiness()
    {
        modeDrowsiness = true;
    }

    public void useAll()
    {
        modeAttention = true;
        modeBlink = true;
        modeDrowsiness = true;
    }

    public bool isUseAttention()
    {
        return modeAttention;
    }

    public bool isUseBlink()
    {
        return modeBlink;
    }

    public bool isUseDrowsiness()
    {
        return modeDrowsiness;
    }
}