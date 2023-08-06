public class UserStatusDelegate
{
    public delegate void onAttention(long timestampBegin, long timestamEnd, float score);
    public delegate void onBlink(long timestamp, bool isBlinkLeft, bool isBlinkRight, bool isBlink, float eyeOpenness);
    public delegate void onDrowsiness(long timestamp, bool isDrowsiness);
}