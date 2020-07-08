﻿using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Globalization;

//#pragma warning disable CS1998
namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Summary("Admin Commands")]
    [RequireAdminRole(Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class AdminCommands : ModuleBase
    {
        [Command("mute")]
        [Summary("Allow you to mute an user, every of his messages will be deleted.")]
        public async Task MuteCommand([Summary("The user to mute")] SocketGuildUser user = null, [Summary("Duration in minutes")]int minutes = 5)
        {
            if (!Utils.GetSettings(Context.Guild.Id).admin_mute)
                return;

            EmbedBuilder embedMessage;
            if (user == null)
            {
                embedMessage = new EmbedBuilder()
                    .WithDescription("User not found.")
                    .WithColor(255, 0, 0);
                await ReplyAsync("", false, embedMessage.Build());
                return;
            }
            var dateEnd = DateTime.Now.AddMinutes(minutes);
            var embedDesc = "";
            if (Utils.GetSettings(Context.Guild.Id).IsMuted(user.Id))
                embedDesc = user.Mention + " is now muted until " + dateEnd.ToString("F", DateTimeFormatInfo.InvariantInfo) + " (was " + Utils.GetSettings(Context.Guild.Id).muted[user.Id].ToString("F", DateTimeFormatInfo.InvariantInfo) + ")";
            else
                embedDesc = user.Mention + " is muted until " + dateEnd.ToString("F", DateTimeFormatInfo.InvariantInfo);
            Utils.GetSettings(Context.Guild.Id).muted[user.Id] = dateEnd;
            Utils.SaveData();
            embedMessage = new EmbedBuilder()
                .WithDescription(embedDesc)
                .WithColor(0, 255, 0);
            await ReplyAsync("", false, embedMessage.Build());


        }

        [Command("unmute")]
        [Summary("Allow you to unmute an user")]
        public async Task UnMuteCommand([Summary("The user to unmute")] SocketGuildUser user = null)
        {
            if (!Utils.GetSettings(Context.Guild.Id).admin_mute)
                return;

            if (user == null)
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("User not found.")
                    .WithColor(255, 0, 0);
                await ReplyAsync("", false, embedMessage.Build());
                return;
            }

            if (!Utils.GetSettings(Context.Guild.Id).IsMuted(user.Id))
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription(user + " is not muted.")
                    .WithColor(255, 0, 0);
                await ReplyAsync("", false, embedMessage.Build());
            } else {
                Utils.GetSettings(Context.Guild.Id).muted.Remove(user.Id);
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription(user + " is now unmuted.")
                    .WithColor(0, 255, 0);
                await ReplyAsync("", false, embedMessage.Build());

            }
        }
    }
}