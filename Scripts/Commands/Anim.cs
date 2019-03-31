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
    public class Anim
    {
        public static void Initialize()
        {
            CommandSystem.Register("Anim", AccessLevel.Administrator, OnCommandMusic);
        }

        [Usage("Anim")]
        private static void OnCommandMusic(CommandEventArgs e)
        {
            var animType = e.GetString(0);
            var action = e.GetInt32(1);
            try
            {

                var a = (AnimationType)System.Enum.Parse(typeof(AnimationType), animType);
                e.Mobile.Animate(a, action);
            } catch(Exception ex)
            {
                e.Mobile.SendMessage("Achei a musica nao veih "+ ex.Message);
                
            }
        }
    }
}
