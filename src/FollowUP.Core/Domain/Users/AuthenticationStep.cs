namespace FollowUP.Core.Domain
{
    public enum AuthenticationStep
    {
        NotAuthenticated = 0,
        TwoFactorRequired = 1,
        ChallengeRequired = 2,
        PhoneRequired = 3,
        RequestMethodRequired = 4,
        NeedCodeVerify = 5,
        Authenticated = 6
    }
}
