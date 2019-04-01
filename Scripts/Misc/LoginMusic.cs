
using Server;
using Server.Network;

namespace Felladrin.Automations
{
    public static class PlayMusicOnLogin
    {
        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        static void OnLogin(LoginEventArgs args)
        {
            args.Mobile.Send(PlayMusic.GetInstance(72));
        }

    }
}
