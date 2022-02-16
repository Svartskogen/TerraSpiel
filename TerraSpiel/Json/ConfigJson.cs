using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace TerraSpiel.Bot
{
    public struct ConfigJson
    {
        //Server config:
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("connstring")]
        public string ConnectionString { get; private set; }
        [JsonProperty("enviroment")]
        public string Enviroment { get; private set; }

        //Gameplay parameters:
        [JsonProperty("harvest_cd")]
        public int HarvestCd { get; private set; }
        [JsonProperty("train_cd")]
        public int TrainCd { get; private set; }
        [JsonProperty("build_cd")]
        public int BuildCd { get; private set; }
        [JsonProperty("starting_money")]
        public int StartingMoney { get; private set; }
    }
}

