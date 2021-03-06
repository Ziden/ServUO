#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Server.Commands.Generic;
using Server.Engines.BulkOrders;
using Server.Items;
using Server.Network;
#endregion

namespace Server.Commands
{
    public class Music
    {
        public static void Initialize()
        {
            CommandSystem.Register("Music", AccessLevel.Administrator, OnCommandMusic);
        }

        [Usage("Music")]
        private static void OnCommandMusic(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Tocando musiquinha :B");
            var musicId = e.GetInt32(0);

            try
            {
                e.Mobile.NetState.Send(PlayMusic.GetInstance(musicId));
            } catch(Exception ex)
            {
                e.Mobile.SendMessage("Achei a musica nao veih "+ ex.Message);
                
            }
        }
    }
}
