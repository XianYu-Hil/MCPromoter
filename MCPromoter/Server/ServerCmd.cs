﻿using System.IO;
using System.Text.RegularExpressions;
using CSR;

namespace MCPromoter
{
    partial class MCPromoter
    {
        public static bool ServerCmdPlugin(Events x)
        {
            var e = BaseEvent.getFrom(x) as ServerCmdEvent;
            if (e == null) return true;

            string cmd = e.cmd;

            if (cmd == "mcp setting reload")
            {
                LoadPlugin();
                if (config.Logging.Plugin) LogsWriter("MCP", "配置文件已重新载入。");
                if (config.ConsoleOutput.Plugin) ConsoleOutputter("MCP", "配置文件已重新载入。");
                return false;
            }

            if (cmd == "stop")
            {
                foreach (var playerData in playerDatas)
                {
                    if (playerData.Value.IsOnline)
                    {
                        playerDatas[playerData.Key].IsOnline = false;
                        _mapi.runcmd($"kick {playerData.Value.Name}");
                    }
                }

                string savedPlayerDatas = javaScriptSerializer.Serialize(playerDatas);
                File.WriteAllText(PluginPath.PlayerDatasPath, savedPlayerDatas);
                return true;
            }

            return true;
        }
        
        public static bool ServerCmdOutputPlugin(Events x)
        {
            var e = BaseEvent.getFrom(x) as ServerCmdOutputEvent;
                if (e == null) return true;

                string output = e.output;
                if (output.StartsWith("Day is "))
                {
                    GameDatas.GameDay = Regex.Replace(output, @"[^0-9]+", "");
                }
                else if (output.StartsWith("Daytime is"))
                {
                    GameDatas.GameTime = Regex.Replace(output, @"[^0-9]+", "");
                }
                else if (output.StartsWith("randomtickspeed = "))
                {
                    GameDatas.TickStatus = Regex.Replace(output, @"[^0-9]+", "");
                }
                else if (output.StartsWith("keepInventory = "))
                {
                    if (output.Contains("keepInventory = true"))
                    {
                        GameDatas.KiStatus = "已开启";
                    }
                    else if (output.Contains("keepInventory = false"))

                    {
                        GameDatas.KiStatus = "已关闭";
                    }
                }
                else if (output.StartsWith("mobGriefing = "))
                {
                    if (output.Contains("mobGriefing = true"))
                    {
                        GameDatas.MgStatus = "已开启";
                    }
                    else if (output.Contains("mobGriefing = false"))

                    {
                        GameDatas.MgStatus = "已关闭";
                    }
                }
                else if (output.Contains("entityCounter"))
                {
                    GameDatas.EntityCounter = Regex.Replace(output, @"[^0-9]+", "");
                }
                else if (output.Contains("itemCounter"))
                {
                    GameDatas.ItemCounter = Regex.Replace(output, @"[^0-9]+", "");
                }

                string[] blockWords =
                {
                    "Killed", "Dead", "Dig", "Placed", "Health", "_CounterCache", "Tasks", "OnlineMinutes",
                    "No targets matched selector", "game mode to Default"
                };
                foreach (var blockWord in blockWords)
                {
                    if (output.Contains(blockWord)) return false;
                }

                return true;
        }
    }
}