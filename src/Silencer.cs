using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace ToasterSilenceQuickChat;

public static class Silencer
{
    // TODO fix the formatting on this list
    private static string[] quickChatsArray = new string[]
    {
        "I got it!",
        "Need stamina!",
        "Take the shot!",
        "Defending...",
        "Nice shot!",
        "Great pass!",
        "Thanks!",
        "What a save!",
        "OMG!",
        "Nooo!",
        "Wow!",
        "Close one!",
        "$#@%!",
        "No problem.",
        "Whoops...",
        "Sorry!",
        "vpas",
        "I love Toaster's mods!",
        "Join the Puck Modding Discord: http://discord.puckstats.io",
        "Calculated.",
        "Faking...",
        "Nice moves!",
        "Please don't hurt me.",
        "I'm open!",
        "In position.",
        "No way!",
        "Great clear!",
        "Savage!",
        "Holy cow!",
        "All yours.",
        "Okay.",
        "Siiick!",
        "This is Puck!",
        "This is Puch!",
        "I love jumping!",
        "Watch this!",
        "Oh well...",
        "Uh oh...",
        "Good try!",
        "Nice block!",
        "Spread out!",
        "Stay back!",
        "Be careful.",
        "Drop...?",
        "Watch out!",
        "Incoming!",
        "Centering...",
        "Two on one!",
        "Backchecking...",
        "Checking...",
        "Get open!",
        "What a banan...",
        "Bang!",
        "I'm dead!",
        "What a play!",
        "Crossing!",
        "I love GAFURIX!",
        "Who wants toast?",
        "Please vote.",
        "Oh my god...",
        "My puch!",
        "Help me!",
        // "gg",
        "Good game!",
        "Well played.",
        "Rematch!",
        "That was fun!",
        "Oops!",
        "Good luck, have fun!",
        "I love toast!",
        "Nice try!",
        "I love modding Puck!",
        "What the puck?!",
        "How...?",
        "Yes!",
        "I quit!",
        "Holy moly!",
        "Nice hit!",
        "Opachki!",
        "Puck!",
        "Let's go!",
        "My fault.",
        "Nice one!",
        "Just kidding!",
        "Woohoo!",
        "I know.",
        "I'm laughing so hard right now!",
        "Thanks for playing!",
        "Bruh.",
        "On your left.",
        "On your right.",
        "Pressure them!",
        //"One-timer!",
        "Bumping!",
        "Unlucky.",
        "Clean.",
        "Team play!",
        "No worries.",
        "Oof.",
        "On defense.",
        "My bad...",
        "I love puckstats.io!",
        "Please be nice.",
        "Wowza!"
    };
    
    // build once, use forever
    private static readonly HashSet<string> quickChats =
        new HashSet<string>(quickChatsArray);
    
    // simple regex to strip out <…> tags
    private static readonly Regex rStripTags = new Regex(@"<.*?>",
        RegexOptions.Compiled);

    [HarmonyPatch(typeof(UIChat), nameof(UIChat.AddChatMessage))]
    public static class UIChatAddChatMessage
    {
        [HarmonyPrefix]
        public static bool Prefix(UIChat __instance, string message)
        {
            if (!Plugin.modSettings.NoQuickChatsEnabled)
                return true;

            // find the *last* ": "
            int sep = message.LastIndexOf(": ", StringComparison.Ordinal);
            if (sep <= 0)
                return true;   // can't split → not a quick-chat

            // everything after the last ": "
            string tail = message.Substring(sep + 2)
                    .Trim()                // trim whitespace
                ;                     

            // strip out any <color> or <b> tags
            tail = rStripTags.Replace(tail, string.Empty);

            // finally check the HashSet
            if (quickChats.Contains(tail))
            {
                Plugin.Log($"Hiding quick-chat: \"{tail}\"");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIChat), nameof(UIChat.Client_SendClientChatMessage))]
    public static class UIChatClientSendClientChatMessage
    {
        [HarmonyPrefix]
        public static bool Prefix(UIChat __instance, string message, bool useTeamChat)
        {
            if (message.Equals("/togglequickchat", StringComparison.OrdinalIgnoreCase))
            {
                Plugin.modSettings.NoQuickChatsEnabled = !Plugin.modSettings.NoQuickChatsEnabled;
                Plugin.modSettings.Save();
                __instance.AddChatMessage($"Quick chats have been <b>toggled" +
                                          $" {(!Plugin.modSettings.NoQuickChatsEnabled ? "on" : "off")}</b>.");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(UIChat), nameof(UIChat.OnQuickChat))]
    public static class UIChatOnQuickChat
    {
        [HarmonyPrefix]
        public static bool Prefix(UIChat __instance, int index)
        {
            // If no quick chat is enabled, return false so that user cannot use quick chats
            return !Plugin.modSettings.NoQuickChatsEnabled;
        }
    }
}